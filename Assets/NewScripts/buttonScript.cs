using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class buttonScript : MonoBehaviour {
	public Button select;
	public Button cancel;
	public Button send;
	public bool isClicked;
	public bool isPressed;
	public bool isSent;

	void Start()
	{
		Button selectbtn = select.GetComponent<Button>();
		isClicked = false;
		selectbtn.onClick.AddListener(TaskOnClick);

		Button cancelbtn = cancel.GetComponent<Button>();
		isPressed = false;
		cancelbtn.onClick.AddListener(TaskOnPress);
	    
		Button sendbtn = send.GetComponent<Button>();
		isSent = false;
		sendbtn.onClick.AddListener(TaskOnSent);



	}

	void LateUpdate(){

		isClicked = false;
		isPressed = false;
		isSent = false;
	}

	void TaskOnClick()
	{
		//Debug.Log("You have clicked the button!");
		isClicked = true;
		//Debug.Log ("isClicked is  " + isClicked);

			}


	void TaskOnPress()
	{
		//Debug.Log("You have clicked the button!");
		isPressed = true;

	}



	void TaskOnSent()
	{
		//Debug.Log("You have clicked the button!");
		isSent = true;
	}



}
