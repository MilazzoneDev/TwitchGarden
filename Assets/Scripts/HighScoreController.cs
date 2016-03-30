using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Globalization;

public class HighScoreController : MonoBehaviour {

	public Text ScoreBoard;
	private List<ScoreHolder> scoreList;
	private bool scoreChanged = false;

	void Start()
	{
		scoreChanged = false;
		scoreList = new List<ScoreHolder>();
		UpdateScore();
	}

	public int removeScore(string user)
	{
		int userPos = FindUser(user);
		if(userPos >= 0)
		{
			int oldScore = scoreList[userPos].score;
			scoreList.RemoveAt(userPos);
			UpdateScore();
			return oldScore;
		}
		return userPos;
	}

	public int getNumPlayers()
	{
		return scoreList.Count;
	}

	public void AddScore(string user, int numToAdd)
	{
		int findUser = FindUser(user);
		if(findUser >= 0)
		{
			scoreList[findUser].score += numToAdd;
		}
		scoreChanged = true;
		UpdateScore();
	}

	public void AddScore(string user, int numToAdd, Color playerColor)
	{
		int findUser = FindUser(user);
		if(findUser >= 0)
		{
			scoreList[findUser].score += numToAdd;
		}
		else
		{
			AddUser(user,numToAdd,playerColor);
		}
		scoreChanged = true;
		UpdateScore();
	}

	public void AddUser(string user, Color userColor)
	{
		if(FindUser(user) < 0)
		{
			scoreList.Add(new ScoreHolder(user,0, userColor));
			UpdateScore();
		}
	}

	public void AddUser(string user, int score, Color userColor)
	{
		int findUser = FindUser(user);
		if(findUser < 0)
		{
			scoreList.Add(new ScoreHolder(user,score, userColor));
		}
		else if(scoreList[findUser].score < score)
		{
			scoreList[findUser].score = score;
			scoreList[findUser].color = userColor;
		}
		scoreChanged = true;
		UpdateScore();
	}

	void UpdateScore()
	{
		if(scoreList.Count > 0)
		{
			if(scoreChanged)
			{
				sortList();
				scoreChanged = false;
			}
			PrintScore();
		}
		else
		{
			if(ScoreBoard)
			{
				ScoreBoard.text = "No Scores to report";
			}
		}
	}

	//finds the user if user is not in list it returns -1
	int FindUser(string user)
	{
		for(int i = 0; i<scoreList.Count;i++)
		{
			if(scoreList[i].user.Equals(user))
			{
				return i;
			}
		}
		return -1;
	}


	void PrintScore()
	{
		ScoreBoard.text = "";
		Color32 c;
		string nameColor;
		for(int i = 0; i < scoreList.Count; i++)
		{
			ScoreHolder place = scoreList[i];
			c = place.color;
			nameColor = "#" + c.r.ToString("X2")+c.g.ToString("X2")+c.b.ToString("X2");
			ScoreBoard.text += (i+1) + ": <color="+nameColor+">"+ place.user + "</color>: " + place.score + "\n";
		}
	}

	//simple bubble sort (look into making a better sort that keeps the first one who got that score in first)
	void sortList()
	{
		for(int i = 0; i < scoreList.Count; i++)
		{
			for(int j= 0; j < (scoreList.Count-1)-i;j++)
			{
				if(scoreList[j].score < scoreList[j+1].score)
				{
					ScoreHolder temp = scoreList[j];
					scoreList[j] = scoreList[j+1];
					scoreList[j+1] = temp;
				}
			}
		}
	}
}
