using UnityEngine;
using System.Collections;

public class PlantScript : MonoBehaviour {

	public GameObject flower;
	public GameObject bomb;
	public GameObject explosion;
	public float lifeTime;

	[Header("Flower")]
	public float flowerStart = 0;
	public float flowerGrowUntil = 0.4f;
	public float flowerGrowSpeed = 0.08f;
	public float flowerDieSpeed = 0.008f;
	public float maxFlowerRotation = 45.0f;
	public float flowerRotationPerSecond = 45.0f;
	public float bombDivisor = 2.0f;

	[Header("Dirt")]
	public float dirtStart = -0.1f;
	public float dirtGrowUntil = -0.037f;
	public float dirtGrowSpeed = 0.04f;

	private bool dirtFinished = false;
	private bool flowerFinished = false;
	private float aliveFor = 0.0f;
	private float flowerRotation = 0.0f;
	private bool flowerRotationForward = true;

	// Use this for initialization
	void Start () {
		//find a random location
		var ground = GameObject.FindGameObjectWithTag("ground");
		this.GetComponent<SphereCollider>().enabled = false;
		float xSize = ground.transform.localScale.x-this.transform.localScale.x;
		float zSize = ground.transform.localScale.z-this.transform.localScale.z;
		float randomX = Random.Range(xSize/-2,xSize/2);
		float randomZ = Random.Range(zSize/-2,zSize/2);
		this.transform.position = new Vector3(randomX,dirtStart,randomZ);
		//random flower color
		Color flowerColor = new Color(Random.value,Random.value,Random.value);
		flower.GetComponent<MeshRenderer>().material.color = flowerColor;

	}
	
	// Update is called once per frame
	void Update () {
		var dt = Time.deltaTime;

		float curX = this.transform.position.x;
		float curY = this.transform.position.y;
		float curZ = this.transform.position.z;


		if(!dirtFinished)
		{
			if(curY + (dirtGrowSpeed*dt) >= dirtGrowUntil)
			{
				this.transform.position = new Vector3(curX,dirtGrowUntil,curZ);
				this.GetComponent<SphereCollider>().enabled = true;
				dirtFinished = true;
			}
			else
			{
				this.transform.position = new Vector3(curX,curY+(dirtGrowSpeed*dt),curZ);
			}
		}
		else if(!flowerFinished)
		{
			float flowerScale = flower.transform.localScale.x;
			float flowerY = flower.transform.localScale.y;

			float newScale = flowerScale+(flowerGrowSpeed*dt);
			if(newScale > flowerGrowUntil)
			{
				newScale = flowerGrowUntil;
				flowerFinished = true;
			}

			flower.transform.localScale = new Vector3(newScale,flowerY,newScale);

			if(bomb)
			{
				float bombScale = newScale/bombDivisor;
				float bombScaleY = bombScale * (1/(flowerGrowUntil/bombDivisor));
				bomb.transform.localScale = new Vector3(bombScale,bombScaleY,bombScale);
				bomb.transform.localPosition = new Vector3(bomb.transform.localPosition.x,0.5f+bombScaleY/2.0f,bomb.transform.localPosition.z);
			}
		}
		else
		{
			if(flower)
			{
				int rotation = 0;
				if(flowerRotationForward)
				{
					rotation = 1;
				}
				else
				{
					rotation = -1;
				}

				Vector3 newRot = flower.transform.rotation.eulerAngles;
				newRot.y += flowerRotationPerSecond*dt*rotation;
				flowerRotation += flowerRotationPerSecond*dt*rotation;
				flower.transform.rotation = Quaternion.Euler(newRot);

				if(flowerRotation > maxFlowerRotation)
				{
					flowerRotationForward = false;
				}
				if(flowerRotation < maxFlowerRotation*-1)
				{
					flowerRotationForward = true;
				}
			}
		}
		//add to life
		aliveFor += dt;
		if(aliveFor >= lifeTime || !flower.activeSelf)
		{
			float newDirtY = curY - (dirtGrowSpeed*dt);
			this.transform.position = new Vector3(curX,newDirtY,curZ);
			if(flower)
			{
				Vector3 flowerPos = flower.transform.position;
				flower.transform.position = new Vector3(flowerPos.x,flowerPos.y-(flowerDieSpeed*dt),flowerPos.z);
			}
			if(bomb)
			{
				float newScale = (newDirtY-dirtStart) / (dirtGrowUntil-dirtStart);
				newScale = newScale < 0 ? 0: newScale;
				newScale *= flowerGrowUntil;

				float bombScale = newScale/bombDivisor;
				float bombScaleY = bombScale * (1/(flowerGrowUntil/bombDivisor));
				bomb.transform.localScale = new Vector3(bombScale,bombScaleY,bombScale);
				bomb.transform.localPosition = new Vector3(bomb.transform.localPosition.x,0.5f+bombScaleY/2.0f,bomb.transform.localPosition.z);
			}
			if(newDirtY <= dirtStart)
			{
				Destroy(this.gameObject);
			}
		}


	}

	public void SetExplosion()
	{
		GameObject.Instantiate(explosion,this.transform.position,Quaternion.Euler(Vector3.zero));
		flower.SetActive(false);
		if(bomb)
		{
			bomb.SetActive(false);
		}
	}
}
