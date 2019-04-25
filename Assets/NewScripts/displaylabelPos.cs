using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displaylabelPos : MonoBehaviour {

	public Text heightlabel;
	private ManipulateController controllerscriptinstant;
	private Vector3 objectPosition;
	// Use this for initialization
	void Start () {

		controllerscriptinstant = GetComponent<ManipulateController>();

	}
	
	// Update is called once per frame
	void Update () {
		objectPosition = controllerscriptinstant.objectPose;
		objectPosition.y += 0.5f; // To keep the labeljust abouve the gameObject
		//Vector3 heightlabelPos = Camera.main.WorldToScreenPoint (this.transform.position);
		Vector3 heightlabelPos = Camera.main.WorldToScreenPoint (objectPosition);
		heightlabel.transform.position = heightlabelPos;
		
	}
}
