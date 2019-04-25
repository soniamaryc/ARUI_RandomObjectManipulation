using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.HelloAR;

public class displayScale : MonoBehaviour
{
	public Text scaleText;
	//public Text angleText;
	public Text heightText;
	private float boxWidth;
	private float boxHeight;
	//private Vector3 boxAngle;
	private float turnAngle;

    //private ManipulateObject controllerscript;
    //private ManipulateVerticalObjects controllerscript;
    private VerticalManipulator controllerscript; 
    private Vector3 virtualcubescale;


	void Start()
	{
		//controllerscript = GetComponent<ManipulateObject>();
		controllerscript = GetComponent<VerticalManipulator>();
		//virtualcubescale = controllerscript.AndyPlanePrefab;
		//Text sets your text to say this message
		//turnAngle = controllerscript.angleTurned1;
		boxHeight = controllerscript.objectScale1.y;
		boxWidth= controllerscript.objectScale1.x; 
		//pose = transform.position;

	}

	void Update()
	{
		//Press the space key to change the Text message
		//boxSize= GetComponent<Collider>().bounds.size;
		//boxSize = virtualcube.transform.localScale;
		//turnAngle = controllerscript.angleTurned1;
		boxWidth= controllerscript.objectScale1.x; 
		boxHeight = controllerscript.objectScale1.y;

		scaleText.text = "Width:"+ (((boxWidth/2))*100).ToString("#.00")+"cm"; 
		//angleText.text = "Turn:"+ turnAngle.ToString(); // rotation about Y axis.
		heightText.text="Height:"+ (((boxHeight/2))*100).ToString("#.00")+"cm";
//		Debug.Log ("Y Euler Angle x:" + turnAngle);
//		Debug.Log ("size x:" + boxSize.x);
//		Debug.Log ("size y:" + boxSize.y);
//		Debug.Log ("size z:" + boxSize.z);

	}
}