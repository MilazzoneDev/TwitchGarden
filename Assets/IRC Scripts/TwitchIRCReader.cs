//made with help from https://github.com/Grahnz/TwitchIRC-Unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TwitchIRCReader : MonoBehaviour 
{
	public string oauth = "blah";
	public string nickName = "justinfan45679";
	public string channelName = "";
	private string server = "irc.twitch.tv";
	private int port = 6667;

	public class MsgEvent : UnityEngine.Events.UnityEvent<string>{ }
	public MsgEvent messageRecievedEvent = new MsgEvent();

	public class JoinEvent : UnityEngine.Events.UnityEvent<string>{ }
	public JoinEvent JoinedChannelEvent = new JoinEvent();

	private string buffer = string.Empty;
	private bool stopThreads = false;
	private Queue<string> commandQueue = new Queue<string>();
	private List<string> recievedMsgs = new List<string>();
	private System.Threading.Thread inProc, outProc;
	private bool joinable = false;
	
	// Use this for initialization
	private void StartClient() 
	{
		System.Net.Sockets.TcpClient socket = new System.Net.Sockets.TcpClient();
		socket.Connect (server,port);
		if(!socket.Connected)
		{
			Debug.Log ("Failed to connect to twitch IRC");
			return;
		}

		Debug.Log("Connected to twitch IRC at: " +server +":"+port);
		var networkStream = socket.GetStream();
		var input = new System.IO.StreamReader(networkStream);
		var output = new System.IO.StreamWriter(networkStream);

		//login
		output.WriteLine("PASS " + oauth);
		output.WriteLine("NICK " + nickName.ToLower());
		output.Flush();

		outProc = new System.Threading.Thread(() => IRCOutputProcedure(output));
		outProc.Start();

		inProc = new System.Threading.Thread(() => IRCInputProcedure(input, networkStream));
		inProc.Start();

	}

	private void IRCInputProcedure(System.IO.TextReader input, System.Net.Sockets.NetworkStream networkStream)
	{
		while(!stopThreads)
		{

			if(networkStream.DataAvailable)
			{
				buffer = input.ReadLine();
				Debug.Log("message recieved: "+buffer);

				if(buffer.Contains("PRIVMSG #"))
				{
					lock(recievedMsgs)
					{
						recievedMsgs.Add(buffer);
					}
				}
				else if(buffer.Contains("JOIN"))
				{
					channelName = buffer.Split('#')[1];
					JoinedChannelEvent.Invoke(channelName);
				}

				//send pong reply to ping messages
				if(buffer.StartsWith("PING "))
				{
					SendCommand(buffer.Replace("PING","PONG"));
				}

				//After server sends 001 command, we can join the cahnnel
				if(buffer.Split(' ')[1] == "001")
				{
					joinable = true;
				}
			}
		}
	}

	private void IRCOutputProcedure(System.IO.TextWriter output)
	{
		System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
		stopWatch.Start();

		while(!stopThreads)
		{
			lock(commandQueue)
			{
				if(commandQueue.Count > 0)
				{
					//Debug.Log("Sending: " + commandQueue.Peek());
					//ensure we don't send messages too quickly
					if(stopWatch.ElapsedMilliseconds > 1750)
					{
						Debug.Log("Sending Command: " + commandQueue.Peek());
						output.WriteLine(commandQueue.Peek());
						output.Flush();
						//remove message
						commandQueue.Dequeue();
						//clear stopwatch
						stopWatch.Reset();
						stopWatch.Start();
					}
				}
			}
		}
	}

	public void SendCommand(string cmd)
	{
		lock(commandQueue)
		{
			commandQueue.Enqueue(cmd);
		}
	}

	void Start()
	{

	}

	void OnEnable()
	{
		Debug.Log("IRC enabled");
		stopThreads = false;
		StartClient();
	}

	void OnDisable()
	{
		Debug.Log("IRC disabled"); 
		stopThreads = true;
	}

	void OnDestroy()
	{
		stopThreads = true;
	}

	// Update is called once per frame
	void Update () 
	{
		lock (recievedMsgs)
		{
			if(recievedMsgs.Count > 0)
			{
				for(int i = 0; i<recievedMsgs.Count;i++)
				{
					messageRecievedEvent.Invoke(recievedMsgs[i]);
				}
				recievedMsgs.Clear();
			}
		}
	}

	public void SendJoin (string channel)
	{
		if(joinable)
		{
			SendPart();
			if(!channel.Equals(""))
			{
				SendCommand("JOIN #" + channel.ToLower());
			}
		}
	}

	public void SendPart ()
	{
		if(joinable && !channelName.Equals(""))
		{
			SendCommand("PART #" + channelName.ToLower());
		}
		channelName = "";
	}
}