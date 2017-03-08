using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientChat : MonoBehaviour {

	[Header("Settings:")]
	public bool showTimeStamp;
	public bool scrollAutomatic;
	public GameObject chatWindow;

	private Client client;
	public List<string> historySinceEnter = new List<string>();

	private void Start ()
	{
		client = GetComponent<Client>();
	}

	private void Update ()
	{
		if (scrollAutomatic) {
			AutoScroll();
		}
		AdjustChatBoxSize();
	}

	private void SendingMessage (string message)
	{
		if (!client.socketReady) {
			return;
		}
		client.streamwriter.WriteLine(message);
		client.streamwriter.Flush();
	}

	public void OnSendButton ()
	{
		string message = GameObject.Find ("ChatBox").GetComponent<InputField> ().text;
		if (message == "") {
			return;
		}
		SendingMessage(message);
		GameObject.Find("ChatBox").GetComponent<InputField>().text = " ";
	}

	private void AutoScroll ()
	{
		Canvas.ForceUpdateCanvases();
		ScrollRect scrollrect = GameObject.Find("ScrollRect").GetComponentInChildren<ScrollRect>();
		scrollrect.verticalNormalizedPosition = 0.0F;
		Canvas.ForceUpdateCanvases();
	}

	private void AdjustChatBoxSize(){
		Text chatbox = GameObject.Find("ChatText").GetComponent<Text>();
		RectTransform recttransform = GameObject.Find("ChatText").GetComponent<RectTransform>();
		recttransform.sizeDelta = new Vector2(recttransform.rect.width, chatbox.preferredHeight);
	}
}
