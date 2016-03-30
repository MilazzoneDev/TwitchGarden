using UnityEngine;
using System.Collections;

public class UserMessage {

	public string user;
	public string command;
	public string arg;

	public UserMessage(string userName, string commandSent)
	{
		user = userName;
		command = commandSent;
	}

	public UserMessage(string userName, string commandSent, string arg1)
	{
		user = userName;
		command = commandSent;
		arg = arg1;
	}

}
