using UnityEngine;
using System.Collections;

public class SteeringVehicle : MonoBehaviour {
	[Header("Force constants")]
	public float maxSpeed = 1.5f;
	public float maxForce = 3.0f;
	public float mass = 2.0f;
	public float radius = 0.0f;
	[Header("stage constants")]
	public float stageWidth = 5.0f;
	public float stageHeight = 5.0f;
	public Vector3 stageCenter;
	[Header("wander constants")]
	public float wanderCircleDist = 0.5f;
	public float wanderCircleRadius = 0.25f;
	public float wanderRange = 15.0f; //range at which the target will wander around the circle

	//variables used for movement
	private Vector3 velocity;
	private Vector3 _steeringForce;
	private bool fwdOverride = false; //used if we want to ignore where it looks
	//used in specific instances of movement (not strictly necisary for all movements)
	private float wanderAngle = Mathf.PI/2; //angle around circle to be wandered in radians;
	//debug constant and variables
	private bool debugging = false;
	private GameObject sphere;
	private GameObject circle;

	public Vector3 steeringForce
	{
		get
		{
			return _steeringForce;
		}
		set
		{
			_steeringForce = value;
		}
	}

	public Vector3 right()
	{
		return this.transform.right;
	}

	public void turn(float ang)
	{
		this.transform.RotateAround(this.transform.position,this.transform.up,ang);
	}

	public void stop()
	{
		this.velocity = Vector3.zero;
	}

	public Vector3 seek(Vector3 target)
	{
		Vector3 desVel;
		Vector3 steeringForce;

		desVel = target - this.transform.position;
		desVel = desVel.normalized * maxSpeed;

		steeringForce = desVel - velocity;

		return steeringForce;
	}

	public Vector3 flee(Vector3 target)
	{
		Vector3 desVel;
		Vector3 steeringForce;

		desVel = this.transform.position - target;
		desVel = desVel.normalized * maxSpeed;

		steeringForce = desVel - velocity;

		return steeringForce;
	}

	public Vector3 approach(Vector3 target)
	{
		Vector3 desVel = target - this.transform.position;
		float distance = desVel.magnitude;
		desVel.Normalize();
		float m = (distance/100) * (maxSpeed/Time.deltaTime);
		desVel *= m;

		Vector3 steeringVector = desVel - velocity;


		return steeringVector;
	}

	public Vector3 avoid(Vector3 obstacle, float radius, float safeDist)
	{
		Vector3 desVel;
		Vector3 steeringForce;

		Vector3 vectorToObstacle = obstacle - this.transform.position;
		float distance = vectorToObstacle.magnitude;

		//if object is still outside the safe distance we don't have to avoid it
		if(((distance-radius)-this.radius) > safeDist)
		{
			return Vector3.zero;
		}
		// if object is behind
		if(Vector3.Dot(vectorToObstacle,right()) < 0)
		{
			return Vector3.zero;
		}

		float rightDotVTO = Vector3.Dot(vectorToObstacle,right ());
		//if sum of radii < dot of vectorToObject with right return zero vector
		if((radius+this.radius) < Mathf.Abs(rightDotVTO))
		{
			return Vector3.zero;
		}

		//desired velocity is to right or left depending on sign of dot with right
		if(rightDotVTO < 0)
		{
			desVel = right() * maxSpeed;
		}
		else
		{
			desVel = right() * (maxSpeed*-1);
		}

		steeringForce = desVel-velocity;

		return steeringForce;
	}

	public Vector3 stayOnStage()
	{
		Vector3 steeringForce = Vector3.zero;
		Vector3 pos = this.transform.position;
		if(pos.x > ((this.stageWidth/2)+this.stageCenter.x))
		{
			steeringForce += flee(new Vector3(pos.x+15,0,pos.z));
		}
		else if(pos.x < (((-1*this.stageWidth)/2)+this.stageCenter.x))
		{
			steeringForce += flee(new Vector3(pos.x-15,0,pos.z));
		}
		if(pos.z > ((this.stageHeight/2)+this.stageCenter.z))
		{
			steeringForce += flee (new Vector3(pos.x,0,pos.z+15));
		}
		else if(pos.z < (((-1*this.stageHeight)/2)+this.stageCenter.z))
		{
			steeringForce += flee (new Vector3(pos.x,0,pos.z-15));
		}

		return steeringForce;
	}

	public Vector3 wander()
	{
		Vector3 steeringForce = Vector3.zero;

		wanderAngle += Random.Range(-wanderRange,wanderRange)*Time.deltaTime;
		//Vector3 angleDir = Quaternion.Euler(0,wanderAngle,0).eulerAngles+this.transform.forward;
		//angleDir *= wanderCircleRadius;

		var seekPoint = new Vector3(Mathf.Cos(wanderAngle),0,Mathf.Sin (wanderAngle));
		seekPoint *= wanderCircleRadius;
		seekPoint = seekPoint + (this.transform.position +(this.transform.forward*wanderCircleDist));

		steeringForce += seek(seekPoint);

		return steeringForce;
	}

	public Vector3 separation()
	{
		return Vector3.zero;
	}

	public Vector3 cohesion()
	{
		return Vector3.zero;
	}

	public Vector3 fullSpeedAhead()
	{
		Vector3 steeringForce;
		steeringForce = (this.transform.forward*maxSpeed)-velocity;
		return steeringForce;
	}

	public void forwardOverride(bool ignoreForward)
	{
		fwdOverride = ignoreForward;
	}

	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
		if(debugging)
		{
			sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			circle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			sphere.GetComponent<SphereCollider>().enabled = false;
			circle.GetComponent<CapsuleCollider>().enabled = false;

			//set positions and scale
			sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			circle.transform.localScale = new Vector3(wanderCircleRadius*2, 0.01f, wanderCircleRadius*2);
		}
	}
	
	// Update is called once per frame last to make sure all forces are added
	void LateUpdate () {
		steeringForce = ClampVector(steeringForce,maxForce);
		Vector3 acceleration = steeringForce/mass;
		velocity += acceleration*Time.deltaTime;

		velocity = ClampVector(velocity,maxSpeed);

		this.transform.position += velocity*Time.deltaTime;
		velocity = new Vector3(velocity.x,0,velocity.z);
		if(!fwdOverride)
		{
			this.transform.LookAt(this.transform.position+velocity);
		}
		if(debugging)
		{
			wander();
			DebugWander();
		}
	}

	private void DebugWander()
	{
		Vector3 pos = this.transform.position;
		Vector3 fwd = this.transform.forward;
		Vector3 spherePos;
		Vector3 circlePos;

		circlePos = pos + (fwd*wanderCircleDist);
		circle.transform.position = circlePos;

		spherePos = new Vector3(Mathf.Cos(wanderAngle),0,Mathf.Sin (wanderAngle));
		spherePos *= wanderCircleRadius;

		sphere.transform.position = spherePos + circlePos;
	}

	//useful class functions

	public static Vector3 ClampVector(Vector3 target,float mag)
	{
		if(target.magnitude > mag)
		{
			target = target.normalized*mag;
		}
		return target;
	}

	public static float ClampNumber(float target, float min, float max)
	{
		if(target > max)
		{
			target = max;
		}
		else if(target < min)
		{
			target = min;
		}
		return target;
	}
	
}
