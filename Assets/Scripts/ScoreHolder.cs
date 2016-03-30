using UnityEngine;
using System.Collections;
using System;

public class ScoreHolder : IComparable<ScoreHolder> {

	public string user;
	public int score;
	public Color color;

	public ScoreHolder(string name, int newScore, Color newColor)
	{
		user = name;
		score = newScore;
		color = newColor;
	}

	//this method is required by the IComparable interface
	public int CompareTo(ScoreHolder other)
	{
		if(other == null)
		{
			return 1;
		}

		return other.score - score;
	}


}
