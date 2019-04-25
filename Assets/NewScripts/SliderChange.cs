using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderChange : MonoBehaviour {

	public GameObject virtualObject;
	Vector3 scaletemp;
	private ManipulateVerticalObjects controllerscript;


	void Start()
	{
		//controllerscript = GetComponent<ManipulateObject>();
		controllerscript = GetComponent<ManipulateVerticalObjects> ();

	}

	public void MoveYdirectionUp(float newValue)
	{
	
		float changeY;
		Vector3 pos = virtualObject.transform.position;
		changeY = pos.y + newValue;
		if (changeY > pos.y)
			virtualObject.transform.position = pos;
		else 
		pos.y= newValue;
		virtualObject.transform.position = pos;

		//virtualObject.transform.Translate (0f, newValue*Time.deltaTime, 0f);
	}


	public void MoveYdirectionDown(float newValue)
	{

		//Vector3 pos = virtualObject.transform.position;
		//pos.y= newValue;
		//virtualObject.transform.position = pos;

		virtualObject.transform.Translate (0f, -newValue*Time.deltaTime, 0f);
	}

	public void PlusScaleYAxis(float newValue)
	{

		Vector3 scaletemp = virtualObject.transform.localScale;
		//Vector3 scaletemp=controllerscript.objectScale;
		scaletemp.y= newValue;
		virtualObject.transform.localScale= scaletemp;


	}

//	public void MinusScaleYAxis(float newValue)
//	{
//
//		Vector3 scaletemp = virtualObject.transform.localScale;
//		scaletemp.y-= newValue;
//		if (scaletemp.y < 0)
//			scaletemp.y = 0;
//		virtualObject.transform.localScale= scaletemp;
//
//
//	}


}
