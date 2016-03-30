using UnityEngine;
using System.Collections;

public class DeadPlayer : MonoBehaviour {

	public float timeUntilGone = 4.0f;
	public float pushMultiplier = 20.0f;
	public float upPush = 5.0f;

	private float timeAlive = 0.0f;

	// Use this for initialization
	void Start () {
	
	}

	public void StartFling(Color playerColor, Transform playerTransform, Vector3 explosionPos)
	{
		//this.GetComponent<MeshRenderer>().material.color = playerColor;

		this.transform.position = playerTransform.position;
		this.transform.rotation = playerTransform.rotation;

		Vector3 pushDir = playerTransform.position - explosionPos;
		pushDir *= pushMultiplier;
		pushDir += Vector3.up*upPush;
		this.GetComponent<Rigidbody>().AddForce(pushDir,ForceMode.Impulse);
		this.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.value*360,Random.value*360,Random.value*360));
	}
	
	// Update is called once per frame
	void Update () {
		timeAlive += Time.deltaTime;
		if(timeAlive >= timeUntilGone)
		{
			Destroy(this.gameObject);
		}
		if(this.transform.position.y <= -20)
		{
			Destroy(this.gameObject);
		}
	}
}
