using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour {

	[Header("Ćlient Settings:")]
	public string clientName;

	private string host = "127.0.0.1";
	private int port = 6321;
	[HideInInspector]
	public bool socketReady;
	[HideInInspector]
	public bool connected;
	[HideInInspector]
	public TcpClient clientSocket;
	[HideInInspector]
	public NetworkStream networkstream;
	[HideInInspector]
	public StreamWriter streamwriter;
	[HideInInspector]
	public StreamReader streamreader;

	private ClientChat clientchat;

	void Start(){
		clientchat = GetComponent<ClientChat>();
	}
	void Update ()
	{
		if (connected && socketReady) {
			GameObject obj = GameObject.Find ("LoginPanel");
			obj.SetActive (false);
			GameObject obj2 = GameObject.Find ("ChatPanel");
			obj2.SetActive (true);
		}
		if (socketReady) {
			if (networkstream.DataAvailable) {
				string message = streamreader.ReadLine ();
				if (message != null) {
					OnIncomingMessage(message);
				}
			}
		}	
	}

	public void ConnectAsHost () {
		if (socketReady) {
			return;
		}
		clientName = GameObject.Find("Username").GetComponent<InputField>().text;
		CheckIfHostPortInputChanged();
		CreateSocket(host,port);
		connected = true;
	}

	public void ConnectAsClient () {
		if (socketReady) {
			return;
		}
		clientName = GameObject.Find("Username").GetComponent<InputField>().text;
		CheckIfHostPortInputChanged();
		CreateSocket(host,port);
		connected = true;
		GameObject obj = GameObject.Find("ServerManger");
		obj.SetActive(false);
	}

	private void CheckIfHostPortInputChanged ()
	{
		string _host = GameObject.Find ("ServerInput").GetComponent<InputField> ().text;
		if (_host != "") {
			host = _host;
		}
		int _port;
		int.TryParse (GameObject.Find ("PortInput").GetComponent<InputField> ().text, out _port);
		if (_port != 0) {
			port = _port;
		}
	}

	private void CreateSocket (string _host, int _port){
		try{
			clientSocket = new TcpClient(_host,_port);
			networkstream = clientSocket.GetStream();
			streamwriter = new StreamWriter(networkstream);
			streamreader = new StreamReader(networkstream);
			socketReady = true;
		} catch(Exception exception){
			Debug.Log("Socket Error:" + exception.Message);
		}
	}

	private void OnIncomingMessage (string message)
	{
		if (clientchat.showTimeStamp) {
			message = System.DateTime.Now + " " + message;
			clientchat.chatWindow.GetComponentInChildren<Text>().text += message + "\n";
			clientchat.historySinceEnter.Add(message);
		}
	}
}
