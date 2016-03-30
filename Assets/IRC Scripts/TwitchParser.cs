//made with help from https://github.com/Grahnz/TwitchIRC-Unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(TwitchIRCReader))]
public class TwitchParser : MonoBehaviour 
{
	private TwitchIRCReader IRC;
	private char[] splitters = {' ',':'};

	public GameObject inputField;

	//used for joining channels
	public Text joinedText;
	private string newJoinedText;
	private bool titleChanged;

	//used for chat window
	public int maxMessages = 50;
	private int numMessages = 0;
	public ScrollRect chatRect;
	public Text chatText;

	private TwitchIRCReader joined;

	//used to send messages to the game
	public class ParsedMsgEvent : UnityEngine.Events.UnityEvent<UserMessage>{ }
	public ParsedMsgEvent recievedParseEvent = new ParsedMsgEvent();

	// Use this for initialization
	void Start () 
	{
		IRC = this.GetComponent<TwitchIRCReader>();
		IRC.messageRecievedEvent.AddListener(OnMessageRecieved);

		joined = this.GetComponent<TwitchIRCReader>();
		joined.JoinedChannelEvent.AddListener(JoinedChannel);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(titleChanged && joinedText != null)
		{
			joinedText.text = newJoinedText;
			titleChanged = false;
		}
	}

	void JoinChannel ()
	{
		string channel = inputField.GetComponent<InputField>().text;
		this.GetComponent<TwitchIRCReader>().SendJoin(channel);
		newJoinedText = "Channel Not Connected";
		if(channel.Equals(""))
		{
			newJoinedText = "No Channel";
		}
		titleChanged = true;
		ClearChat();
	}

	void JoinedChannel (string channel)
	{
		Debug.Log("Joined a channel: " + channel);
		newJoinedText = channel + "'s chat";
		titleChanged = true;
	}
	
	void OnMessageRecieved(string msg)
	{
		UserMessage newMessage = Parse (msg);

		if(newMessage != null)
		{
			recievedParseEvent.Invoke(newMessage);
		}

		UpdateChat(msg);

	}

	void UpdateChat(string msg)
	{
		if(msg.Contains("PRIVMSG #"))
		{
			int msgStart = msg.IndexOf("PRIVMSG #");
			//removes the "Privmsg # channelname :" from the message
			string message = msg.Substring(msgStart + IRC.channelName.Length + 11);
			string user = msg.Substring(1, msg.IndexOf('!')-1);

			if(numMessages >= maxMessages)
			{
				int secondMessageStart = chatText.text.IndexOf("\n\n") + 2;
				chatText.text = chatText.text.Substring(secondMessageStart);
				numMessages--;
			}

			Random.seed = user.Length + (int)user[0] + (int)user[user.Length - 1];
			Color32 c = new Color(Random.Range(0.25f,0.65f),Random.Range(0.25f,0.65f),Random.Range(0.25f,0.65f));
			string nameColor = "#" + c.r.ToString("X2")+c.g.ToString("X2")+c.b.ToString("X2");

			if(message.StartsWith("\u0001ACTION"))
			{
				message = message.Substring(8);
				chatText.text += "<color="+nameColor+"><b>"+user + " " + message + "</b></color>\n\n";
			}
			else
			{
				chatText.text += "<color="+nameColor+"><b>"+user+"</b></color>"+": " + message + "\n\n";
			}
			numMessages++;
		}
	}

	void ClearChat()
	{
		chatText.text = "";
		numMessages = 0;
	}

	//parses a message (returns null if not a command)
	UserMessage Parse(string msg)
	{
		var separatedMessage = msg.Split(splitters);
		var nameSeparated = separatedMessage[1].Split('!')[0];
		UserMessage pMsg;
		if(separatedMessage[5].StartsWith("!"))
		{
			if(separatedMessage.Length > 6)
			{
				pMsg = new UserMessage(nameSeparated,separatedMessage[5],separatedMessage[6]);
			}
			else
			{
				pMsg = new UserMessage(nameSeparated,separatedMessage[5]);
			}

			return pMsg;
		}

		return null;
	}


}
