using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.HelloAR;

public class displayScaleHorizontal : MonoBehaviour
{
	public Text scaleText;
	public Text angleText1;
	public Text angleText2;
	private float boxWidth;

	private float turnAngle1;
	private float turnAngle2;

	//private ManipulateObject controllerscript;
	private ManipulateController controllerscript;
	private Vector3 virtualcubescale;


	void Start()
	{
		controllerscript = GetComponent<ManipulateController>();
				//Text sets your text to say this message
		turnAngle1 = controllerscript.angleTurned1;
		turnAngle2 = controllerscript.angleTurned2;
		boxWidth= controllerscript.objectScale.x; 
		//pose = transform.position;

	}

	void Update()
	{
		//Press the space key to change the Text message

		turnAngle1 = controllerscript.angleTurned1;
		turnAngle2 = controllerscript.angleTurned2;
		boxWidth= controllerscript.objectScale.x; 
		//boxHeight = controllerscript.objectScale.y;

		scaleText.text = "Width:"+ (boxWidth*100).ToString()+"cm"; 
		angleText1.text = "Turn:"+ turnAngle1.ToString(); // rotation about Y axis.
		angleText2.text = "Turn:"+ turnAngle2.ToString(); // rotation about Y axis.
		//		Debug.Log ("Y Euler Angle x:" + turnAngle);
		//		Debug.Log ("size x:" + boxSize.x);
		//		Debug.Log ("size y:" + boxSize.y);
		//		Debug.Log ("size z:" + boxSize.z);

	}
}