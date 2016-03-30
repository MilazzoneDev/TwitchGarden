using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float explosionSize = 4;
	[Range(0.0f,1.0f)]
	public float hitRange = 0.7f;
	public float expansionSpeed = 2;
	public float rotSpeed = 900.0f;
	public float maxAlpha = 0.5f;
	public GameObject child;

	private Color mainColor;
	private Color childColor;
	
	// Use this for initialization
	void Start () {
		this.transform.localScale = new Vector3(0,0,0);
		child.transform.localScale = new Vector3(hitRange,hitRange,hitRange);
		mainColor = this.GetComponent<Renderer>().material.GetColor("_TintColor");
		childColor = child.GetComponent<Renderer>().material.GetColor("_TintColor");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 curScale = this.transform.localScale;
		float scaleChange = expansionSpeed * Time.deltaTime;
		curScale = new Vector3(curScale.x+scaleChange,curScale.y+scaleChange,curScale.z+scaleChange);
		this.transform.localScale = curScale;

		this.transform.RotateAround(this.transform.position,Vector3.up,-1.0f*rotSpeed*Time.deltaTime);
		child.transform.RotateAround(this.transform.position, Vector3.up,2.0f*rotSpeed*Time.deltaTime);

		//Shader mainShader = this.GetComponent<>();
		float newAlpha = (1-(curScale.x/explosionSize))*maxAlpha;
		Color newMainColor = new Color(mainColor.r,mainColor.g,mainColor.b,newAlpha);
		this.GetComponent<Renderer>().material.SetColor("_TintColor",newMainColor);

		Color newChildColor = new Color(childColor.r, childColor.g, childColor.b, newAlpha);
		child.GetComponent<Renderer>().material.SetColor("_TintColor",newChildColor);

		if(curScale.x >= explosionSize)
		{
			Debug.Log(curScale.x);
			Destroy(this.gameObject);
		}

	}
}
