using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteeringVehicle))]
public class Player : MonoBehaviour {

	static PlantGameController gameController;

	public static void getGameController(PlantGameController game)
	{
		gameController = game;
	}

	[Header("Movement Weights")]
	[Range(0.0f,10.0f)]
	public float wanderWeight = 5.0f;
	[Range(0.0f,10.0f)]
	public float forwardWeight = 0.01f;
	[Range(0.0f,10.0f)]
	public float stayOnStageWeight = 2.0f;
	[Range(0.0f,10.0f)]
	public float seekWeight = 4.0f;

	[Header("Text Variables")]
	public float offset = 0.5f;//0.5
	public int fontSize = 60;//60
	public float textScale = 0.05f;//0.05

	private string _playerName;
	private Color _playerColor;
	private GameObject target;

	[Header("Dead Player Attributes")]
	public float timeUntilGone = 4.0f;
	public float pushMultiplier = 80.0f;
	public float upPush = 5.0f;
	public float explodeTorque = 10.0f;
	private bool deadPlayer = false;

	private float timeAlive = 0.0f;

	private SteeringVehicle vehicle;
	private GameObject textObj;
	private TextMesh text;

	public Color playerColor
	{
		get
		{
			return _playerColor;
		}
		set
		{
			_playerColor = value;
			if(!text)
			{
				createText();
			}
			text.color = value;
			this.GetComponent<MeshRenderer>().material.color = value;
		}
	}

	public string playerName
	{
		get
		{
			return _playerName;
		}
		set
		{
			_playerName = value;
			if(!text)
			{
				createText();
			}
			text.text = value;
			textObj.name = this.name + " text";
		}
	}

	private void createText()
	{
		textObj = new GameObject();
		textObj.name = this.name + " text";
		text = textObj.AddComponent<TextMesh>();
		
		textObj.transform.position = this.transform.position;
		text.anchor = TextAnchor.MiddleCenter;
		text.transform.rotation = Quaternion.LookRotation(Vector3.down);
		text.fontSize = fontSize;
		text.transform.position = this.transform.position + new Vector3(0,0,offset);
		text.transform.localScale = new Vector3(textScale,textScale,textScale);
	}

	// Use this for initialization
	void Start () {
		vehicle = this.GetComponent<SteeringVehicle>();
		if(!text)
		{
			createText();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!deadPlayer)
		{
			Vector3 steering;
			if(!target)
			{
				steering = Wander();
			}
			else
			{
				steering = Approach();
				if(!target.activeSelf)
				{
					target = null;
				}
			}

			vehicle.steeringForce = steering;
		}
		else
		{
			timeAlive += Time.deltaTime;
			if(timeAlive >= timeUntilGone)
			{
				gameController.AddHighScore(playerName,playerColor);
			}
		}

		textObj.transform.position = this.transform.position + new Vector3(0,0,offset);

		if(this.transform.position.y < -10)
		{
			gameController.AddHighScore(playerName,playerColor);
		}
	}
	
	Vector3 Wander()
	{
		Vector3 steering = SteeringVehicle.ClampVector(vehicle.wander(),vehicle.maxForce)*wanderWeight;
		steering += SteeringVehicle.ClampVector(vehicle.fullSpeedAhead(),vehicle.maxForce)*forwardWeight;
		steering += SteeringVehicle.ClampVector(vehicle.stayOnStage(),vehicle.maxForce)*stayOnStageWeight;
		return steering;
	}

	Vector3 Seek()
	{
		Vector3 steering = SteeringVehicle.ClampVector(vehicle.seek(target.transform.position),vehicle.maxForce)*seekWeight;
		return steering;
	}

	Vector3 Approach()
	{
		Vector3 steering = SteeringVehicle.ClampVector(vehicle.approach(target.transform.position),vehicle.maxForce);
		return steering;
	}

	void OnTriggerEnter(Collider collider)
	{
		GameObject obj = collider.gameObject;
		if(obj.tag == "plant")
		{
			target = obj.GetComponent<PlantScript>().flower;
		}
		else if(obj.tag == "flower")
		{
			obj.GetComponentInParent<SphereCollider>().enabled = false;
			obj.SetActive(false);
			vehicle.stop();
			Player.gameController.AddScore(playerName,1);
			target = null;
		}
		else if(obj.tag == "bomb")
		{
			obj.GetComponentInParent<PlantScript>().SetExplosion();
			StartFling(obj.transform.position);
		}
	}
	void OnTriggerExit(Collider collider)
	{
		if(target && target == collider.gameObject)
		{
			target = null;
		}
	}

	//DEAD PLAYER METHODS
	public void StartFling(Vector3 explosionPos)
	{
		deadPlayer = true;
		this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		vehicle.forwardOverride(true);

		Vector3 pushDir = this.transform.position - explosionPos;
		pushDir *= pushMultiplier;
		pushDir += Vector3.up*upPush;
		this.GetComponent<Rigidbody>().AddForce(pushDir,ForceMode.Impulse);
		this.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.value*360,Random.value*360,Random.value*360)*explodeTorque,ForceMode.Impulse);
	}

}
