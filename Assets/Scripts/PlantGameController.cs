using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(TwitchParser))]
[RequireComponent(typeof(TwitchIRCReader))]
public class PlantGameController : MonoBehaviour 
{
	[Header("Prefabs")]
	public GameObject floor;
	public GameObject FlowerPrefab;
	public GameObject BombPrefab;
	public GameObject PlayerPrefab;

	[Header("Score Controllers")]
	public HighScoreController HighScore;
	public HighScoreController CurrentScore;

	[Header("Slider Controls")]
	public Slider flowerSlider;
	public Slider bombSlider;

	[Header("Game Properties")]
	private float flowerSpawnTime = 0f;
	[Range(0.0f,0.4f)]
	public float flowerSpawn = 0.15f;
	[Range(0.0f,1.0f)]
	public float bombChance = 0.25f;

	private TwitchParser commands;
	private float lastFlowerSpawn = 0.0f;


	// Use this for initialization
	void Start () 
	{
		commands = this.GetComponent<TwitchParser>();
		commands.recievedParseEvent.AddListener(OnMessage);
		Player.getGameController(this);
		if(flowerSlider)
		{
			flowerSlider.value = flowerSpawn;
			FlowerSpawnChange();
		}
		if(bombSlider)
		{
			bombSlider.value = bombChance;
			BombChanceChange();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		var dt = Time.deltaTime;
		lastFlowerSpawn += dt;
		if(lastFlowerSpawn >= flowerSpawnTime)
		{
			float bombRoll = Random.value;
			if(bombChance > bombRoll)
			{
				Instantiate(BombPrefab);
			}
			else
			{
				Instantiate(FlowerPrefab);
			}
			lastFlowerSpawn = 0.0f;
		}
	}
	
	void AddNewPlayer(string player, Color playerColor)
	{
		//check if player was already added
		if (!checkForPlayer(player))
		{
			GameObject newPlayer = GameObject.Instantiate(PlayerPrefab);
			newPlayer.transform.parent = this.transform;
			Player newPlayerScript = newPlayer.GetComponent<Player>();
			newPlayer.name = player;
			float xSize = floor.transform.localScale.x-this.transform.localScale.x-newPlayer.transform.localScale.x;
			float zSize = floor.transform.localScale.z-this.transform.localScale.z-newPlayer.transform.localScale.x;
			float randomX = Random.Range(xSize/-2,xSize/2);
			float randomZ = Random.Range(zSize/-2,zSize/2);
			newPlayer.transform.position = new Vector3(randomX,newPlayer.transform.localScale.y*2,randomZ);
			newPlayerScript.playerName = player;
			newPlayerScript.playerColor = playerColor;

			CurrentScore.AddUser(player,playerColor);

			FlowerSpawnChange();
		}

	}

	void ClearPlayers()
	{
		for(int i = 0; i < this.transform.childCount;i++)
		{
			Destroy(GameObject.Find(this.transform.GetChild(i).gameObject.name + " text"));
			Destroy(this.transform.GetChild(i).gameObject);
		}
		FlowerSpawnChange();
	}

	bool checkForPlayer(string playerName)
	{
		if(GameObject.Find(playerName))
		{
			return true;
		}
		return false;
	}

	public void AddScore(string playerName, int points)
	{
		CurrentScore.AddScore(playerName, points);
	}

	public void AddHighScore(string playerName, Color playerColor)
	{
		Destroy(GameObject.Find(playerName + " text"));
		Destroy(GameObject.Find(playerName));
		int finalScore = CurrentScore.removeScore(playerName);
		HighScore.AddUser(playerName,finalScore,playerColor);
		FlowerSpawnChange();
	}

	public void FlowerSpawnChange()
	{
		Debug.Log("flowerChange");
		int numPlayers = CurrentScore.getNumPlayers();
		numPlayers = numPlayers > 0? numPlayers:1;
		float flowersPerSecond = numPlayers * flowerSlider.value;
		Debug.Log("flowers per second: " + flowersPerSecond);
		flowerSpawnTime = 1/flowersPerSecond;
		Debug.Log("flower spawn time: " + flowerSpawnTime);
	}

	public void BombChanceChange()
	{
		bombChance = bombSlider.value;
	}

	void OnMessage(UserMessage cmd)
	{
		if(cmd.command.Equals("!join") || cmd.command.Equals("!Join"))
		{
			if(cmd.arg != null)
			{
				Color newColor = ColorPicker.pickColor(cmd.arg);
				if(newColor != Color.clear)
				{
					AddNewPlayer(cmd.user, newColor);
				}
			}
		}
		if(cmd.command.Equals("!testbug"))
		{
			if(cmd.arg != null)
			{
				int num;
				if(int.TryParse(cmd.arg, out num))
				{
					for(int i=0;i<num;i++)
					{
						AddNewPlayer("testing "+i,ColorPicker.pickColor("random"));
					}
				}
				else
				{
					AddNewPlayer(cmd.arg,ColorPicker.pickColor("random"));
				}
			}
		}

	}//end of on message


}
