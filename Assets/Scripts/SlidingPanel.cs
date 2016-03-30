using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlidingPanel : MonoBehaviour {

	public float ChangeTime = 5.0f;
	public float moveSpeed = 5.0f;
	public Text buttonText;

	private float timeSinceChange = 0.0f;
	private float changeDir = 1;
	private bool moving = false;
	private bool autoMoving = true;
	private string leftText = "Chat";
	private string rightText = "Instructions";

	private float initPos;
	private float finalPos;

	// Use this for initialization
	void Start () {
		Rect panel = this.GetComponent<RectTransform>().rect;
		initPos = this.transform.position.x;
		finalPos = this.transform.position.x - panel.width/2;
		buttonText.text = rightText;
	}

	// this will really only work initially for starting on a left direction
	// Update is called once per frame
	void Update () {
		if(autoMoving)
		{
			timeSinceChange += Time.deltaTime;
		}

		if(moving)
		{
			float newX = (changeDir*Time.deltaTime*moveSpeed) + this.transform.position.x;
			if(newX < finalPos && changeDir == -1)
			{
				newX = finalPos;
				FinishMoving();
			}
			if(newX > initPos && changeDir == 1)
			{
				newX = initPos;
				FinishMoving();
			}
			Vector3 newPos = new Vector3(newX,this.transform.position.y,this.transform.position.z);
			this.transform.position = newPos;
		}
		else if(timeSinceChange >= ChangeTime)
		{
			StartMoving();
		}
	}

	void StartMoving()
	{
		changeDir *= -1;
		ChangeButtonText();
		if(!moving)
		{
			moving = true;
		}
	}

	void FinishMoving()
	{
		moving = false;
		timeSinceChange = 0;
	}

	void SlidePannel()
	{
		StartMoving();
	}

	void ChangeButtonText()
	{
		if(buttonText)
		{
			if(changeDir > 0)
			{
				buttonText.text = rightText;
			}
			else
			{
				buttonText.text = leftText;
			}
		}
	}

	void ToggleAuto()
	{
		autoMoving = !autoMoving;
	}
}
