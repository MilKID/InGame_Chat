using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;

public class ServerChat : MonoBehaviour {

	public static List<string> chatHistory = new List<string>();

	private void Update ()
	{
		SaveChatHistory();
	}

	public void SendMessageToChat (string message, List<ServerClient> recipients){
		for (int i = 0; i < recipients.Count; i++) {
			try{
				StreamWriter streamWriter = new StreamWriter(recipients[i].tcpClient.GetStream());
				streamWriter.WriteLine(message);
				streamWriter.Flush();
				ServerChat.chatHistory.Add(message);
			} catch(Exception exception){
				Debug.Log("Write Error: " + exception.Message + " to Client " + recipients[i].clientName);
			}
		}
	}

	private void SaveChatHistory ()
	{
		string fileName = "chatHistory.txt";
		if (File.Exists (fileName)) {
			return;
		}

		TextWriter history = File.CreateText (fileName);
		for (int i = 0; i < chatHistory.Count; i++) {
			history.WriteLine(chatHistory[i]);
		}
		history.Close();
	}
}
