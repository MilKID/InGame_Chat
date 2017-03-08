using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;

public class ServerClient {

	public TcpClient tcpClient;
	public string clientName;

	public ServerClient (TcpClient _tcpClient)
	{
		tcpClient = _tcpClient;
		if (GameObject.Find ("Client").GetComponent<Client> ().clientName == null) {
			clientName = "Guest" + (Server.connectedClients.Count + 1);
		} else {
			clientName = GameObject.Find("Client").GetComponent<Client>().clientName;
		}
	}
}
public class Server : MonoBehaviour {

	[Header("Server Settings:")]
	public int port = 6321;

	private TcpListener server;
	private ServerChat serverchat;
	private bool serverStarted;

	public static List<ServerClient> connectedClients = new List<ServerClient>();
	public static List<ServerClient> disconnectedClients = new List<ServerClient>();

	private void Start(){
		serverchat = GetComponent<ServerChat>();
	}

	private void Update ()
	{
		if (!serverStarted) {
			return;
		}
		CheckConnectionAndStream();
	}

	private void StartUpServer(){
		try {
			server = new TcpListener(IPAddress.Any,port);
			server.Start();
			StartListening();
		} catch (Exception exception){
			Debug.Log("Socket Error: " + exception.Message);
		}
	}

	private bool isConnected (TcpClient _tcpClient){
		try{
			if(_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected){
				if(_tcpClient.Client.Poll(0, SelectMode.SelectRead)){
					return !(_tcpClient.Client.Receive(new byte[1],SocketFlags.Peek) == 0);
				}
				return true;
			} else {
				return false;
			}
		} catch {
			return false;
		}
	}

	private void StartListening(){
		serverStarted = true;
		Debug.Log("Server has been started - Port: " + port.ToString());
		server.BeginAcceptTcpClient(AcceptClient,server);
	}

	private void AcceptClient (IAsyncResult iasyncResult){
		TcpListener listener = (TcpListener) iasyncResult.AsyncState;
		connectedClients.Add(new ServerClient(listener.EndAcceptTcpClient(iasyncResult)));
		StartListening();

		serverchat.SendMessageToChat(connectedClients[connectedClients.Count-1].clientName + "has connected!", connectedClients);
	}

	public void OnIncomingMessage (ServerClient serverClient, string message)
	{
		if (message.Contains ("/w")) {
			for (int i = 0; i < Server.connectedClients.Count; i++) {
				if (message.Contains (Server.connectedClients [i].clientName)) {
					List<ServerClient> recipient = new List<ServerClient> ();
					recipient.Add (serverClient);
					recipient.Add (Server.connectedClients [i]);
					string tempMessage = "/w " + Server.connectedClients [i].clientName;
					int stringlength = tempMessage.Length;
					string newMessage = message.Substring (stringlength);
					serverchat.SendMessageToChat (serverClient.clientName + "whispered: " + newMessage, recipient);
				}
			}
		} else {
			serverchat.SendMessageToChat(serverClient.clientName + ": " + message, Server.connectedClients);
		}
	}

	private void CheckConnectionAndStream ()
	{
		for (int i = 0; i < connectedClients.Count; i++) {
			if (!isConnected (connectedClients [i].tcpClient)) {
				connectedClients [i].tcpClient.Close ();
				disconnectedClients.Add (connectedClients [i]);
				connectedClients.Remove (connectedClients [i]);
				serverchat.SendMessageToChat (disconnectedClients [disconnectedClients.Count - 1].clientName + "has been disconnected!", connectedClients);
				continue;
			} else {
				NetworkStream networkstream = connectedClients [i].tcpClient.GetStream ();
				if (networkstream.DataAvailable) {
					StreamReader streamreader = new StreamReader (networkstream, true);
					string message = streamreader.ReadLine ();
					if (message != null) {
						OnIncomingMessage(connectedClients[i],message);
					}
				}
			}
		}
	}
}
