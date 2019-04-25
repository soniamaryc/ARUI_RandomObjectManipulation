    using GoogleARCore.Examples.HelloAR;
    using GoogleARCore.Examples.AugmentedImage;
	using System.Collections.Generic;
	using GoogleARCore;
	using GoogleARCore.Examples.Common;
	using UnityEngine;
#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

	/// <summary>
	/// Controls the HelloAR example.
	/// </summary>
	public class ControllerArAug : MonoBehaviour
	{
		/// <summary>
		/// The first-person camera being used to render the passthrough camera image (i.e. AR background).
		/// </summary>
		public Camera FirstPersonCamera;

		/// <summary>
		/// A prefab for tracking and visualizing detected planes.
		/// </summary>
		public GameObject DetectedPlanePrefab;

		/// <summary>
		/// A model to place when a raycast from a user touch hits a plane.
		/// </summary>
		public GameObject pickPrefab;
	    public GameObject placePrefab;
	    public GameObject MarkerPrefab;

		/// <summary>
		/// A gameobject parenting UI for displaying the "searching for planes" snackbar.
		/// </summary>
		public GameObject SearchingForPlaneUI;

		/// <summary>
		/// The rotation in degrees need to apply to model when the Andy model is placed.
		/// </summary>
		private const float k_ModelRotation = 180.0f;

		/// <summary>
		/// A list to hold all planes ARCore is tracking in the current frame. This object is used across
		/// the application to avoid per-frame allocations.
		/// </summary>
		private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

		/// <summary>
		/// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
		/// </summary>
		private bool m_IsQuitting = false;

	    //
		public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;

		/// <summary>
		/// The overlay containing the fit to scan user guide.
		/// </summary>
		public GameObject FitToScanOverlay;

		private Dictionary<int, AugmentedImageVisualizer> m_Visualizers= new Dictionary<int, AugmentedImageVisualizer>();

		private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();


		//New variables
		BtAutoScript btmodule;
		buttonScript buttonModule;
		bool ButtonClicked;
		bool ButtonPressed;
		bool doneOnce =false;
	    bool clearedALL =true;
	    //bool firstDetect = true;
		GameObject targetObject1;
	    GameObject targetObject2;
	    GameObject markerCenter;
	    GameObject robotworkspace;
	    public GameObject workspace; 

	    Anchor anchor1;
	    Anchor anchor2;
	    Anchor anchor3;
	    Anchor Markeranchor;
	    Vector3 startPose;
	    Vector3 endPose;
 	    //bool ImageTracking = false;
	    int count=0;
	    Vector3 thePosition;
	   // public int reachscale =2;
	   // Vector3 offsetVector = new Vector3(0,0,-0.01);
	 
        
		public void Start(){
			btmodule= gameObject.GetComponent<BtAutoScript>();
			buttonModule = gameObject.GetComponent<buttonScript> ();
			//ButtonClicked = gameObject.GetComponent<buttonScript> ().isClicked;
		}

		/// <summary>
		/// The Unity Update() method.
		/// </summary>
		/// 

		public void Update()
		{

			ButtonClicked = buttonModule.isClicked;
		   // Debug.Log("Button status:"+ ButtonClicked);
			ButtonPressed = buttonModule.isPressed;
			_UpdateApplicationLifecycle();

		//--------------------------------------------------------------------------------------------------
		  // ------------------------Marker detection------------------------------------------------------
		// --------------------------------------------------------------------------------------------------
		// Get updated augmented images for this frame.
		Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

		// Create visualizers and anchors for updated augmented images that are tracking and do not previously
		// have a visualizer. Remove visualizers for stopped images.
		foreach (var image in m_TempAugmentedImages)

		{
			AugmentedImageVisualizer visualizer = null;
			m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
			if (image.TrackingState == TrackingState.Tracking && visualizer == null)
			//if (image.TrackingState == TrackingState.Tracking && markerCenter==null )
			{
				// Create an anchor to ensure that ARCore keeps tracking this augmented image.
				anchor1 = image.CreateAnchor(image.CenterPose);
				//ImageTracking = true;

				//------------------------- Adding a visual for showing the robot workspace----------------------------
				// Not updating it always. Detecting and storing the values only once at the beginning.
				//-----------------------------------------------------------------------------------------------------




				visualizer = (AugmentedImageVisualizer)Instantiate(AugmentedImageVisualizerPrefab, anchor1.transform);
			    visualizer.Image = image;
				m_Visualizers.Add(image.DatabaseIndex, visualizer);
				//visualizer.gameObject.SetActive (false); // this line is fort not showing the frame.

				//------------------------- Adding a visual for showing the robot workspace----------------------------
				// Not updating it always. Detecting and storin the values only once at the beginning.
				//-----------------------------------------------------------------------------------------------------
			
				markerCenter = Instantiate (MarkerPrefab,anchor1.transform.position, anchor1.transform.rotation);
				markerCenter.transform.parent = anchor1.transform;
				Vector3 worklocation = (anchor1.transform.position + anchor1.transform.forward*0.095f); // adjusting the workspace. The origin of the workspace plane is ahead of the anchor origin
		     	robotworkspace = Instantiate (workspace,worklocation, anchor1.transform.rotation);//Quaternion.identity);
				robotworkspace.transform.parent = anchor1.transform;
				//-------------------------------------------------------------------------------------------------
				//Debug.Log ("Marker  center pose:" + image.CenterPose.position);
				Debug.Log ("Marker height:" + image.ExtentZ);
				Debug.Log ("Marker width:" + image.ExtentX);


				Debug.Log ("anchor1X:" + anchor1.transform.position.x*100 + ", anchor1Y:" + anchor1.transform.position.y*100 + ", anchor1Z:" + anchor1.transform.position.z*100);
				//Debug.Log ("MarkerXangle=" + anchor1.transform.eulerAngles.x + ", MarkerYangle=" + anchor1.transform.eulerAngles.y+ ", MarkerZangle=" + anchor1.transform.eulerAngles.z);
			}
			else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
				
			{
				m_Visualizers.Remove(image.DatabaseIndex);
				//GameObject.Destroy (markerCenter.gameObject);
				markerCenter.gameObject.SetActive (false);
						
				GameObject.Destroy(visualizer.gameObject);
				visualizer.gameObject.SetActive (false);
			}

			//Debug.Log ("The current marker image center: X=" + image.CenterPose.position.x * 100 + ", Y=" + image.CenterPose.position.y * 100 + ", Z=" + image.CenterPose.position.z * 100);
		}



		//-----------------------------------------------------------------------------
		// Script for ground plane detection starts here.
		//-----------------------------------------------------------------------------
			// Hide snackbar when currently tracking at least one plane.
			Session.GetTrackables<DetectedPlane>(m_AllPlanes);
			bool showSearchingUI = true;
			for (int i = 0; i < m_AllPlanes.Count; i++)
			{
				if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
				{
					showSearchingUI = false;

					break;
				}
			}

			SearchingForPlaneUI.SetActive(showSearchingUI);

			// If the player has not touched the screen, we are done with this update.
			// Touch touch;
			// if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
			//{
			//    return;
			// }

			// Raycast against the location the player touched to search for planes.
			TrackableHit hit;
			TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

		if (ButtonClicked && !doneOnce && clearedALL ) {
			//Debug.Log ("Button clicked");
				//if (Frame.Raycast (touch.position.x, touch.position.y, raycastFilter, out hit)) {
				if (Frame.Raycast ((float)Screen.width/2,(float)Screen.height/2, raycastFilter, out hit)) {   
				    
					// Use hit pose and camera pose to check if hittest is from the back of the plane, if it is, no need to create the anchor.
					if ((hit.Trackable is DetectedPlane) && Vector3.Dot (FirstPersonCamera.transform.position - hit.Pose.position,hit.Pose.rotation * Vector3.up) < 0) {
						Debug.Log ("Hit at back of the current DetectedPlane");
					} else {
						// Instantiate PICK or PLACE at the hit poses.
					     count = count + 1;
					     if (count == 1) {
						  anchor2 = hit.Trackable.CreateAnchor (hit.Pose);
						  targetObject1 = Instantiate (pickPrefab, hit.Pose.position, hit.Pose.rotation);
						  targetObject1.transform.parent = anchor2.transform;
						  //count = count + 1;
						   }

					  else 
						 {
						 anchor3 = hit.Trackable.CreateAnchor (hit.Pose);
						 targetObject2 = Instantiate (placePrefab, hit.Pose.position, hit.Pose.rotation);
						 targetObject2.transform.parent = anchor3.transform;
						 count = 0;
						 doneOnce = true;
						 }
						
					// Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
						//targetObject.transform.Rotate (0, k_ModelRotation, 0, Space.Self);

					//................................. Non homogenous transformation.....................................
					//............................Sending Marker (angle,position) and object's pick and pose (position) to the Manipulator
					//.....................................................................................................

					if (doneOnce) {
						string alphaX = anchor1.transform.eulerAngles.x.ToString ();
						string alphaY = anchor1.transform.eulerAngles.y.ToString ();
						string alphaZ = anchor1.transform.eulerAngles.z.ToString ();
						btmodule.sendMsg (alphaX);
						btmodule.sendMsg (alphaY); 
						btmodule.sendMsg (alphaZ);

						Debug.Log (" Marker pose before sending\n");
						Debug.Log ("MarkerX:" + anchor1.transform.position.x * 100 + ", MarkerY:" + anchor1.transform.position.y * 100 + ", MarkerZ:" + anchor1.transform.position.z * 100);
						Debug.Log ("MarkerXangle=" + anchor1.transform.eulerAngles.x + ", MarkerYangle=" + anchor1.transform.eulerAngles.y + ", MarkerZangle=" + anchor1.transform.eulerAngles.z);


					    string RX = anchor1.transform.position.x.ToString ();
						string RY = anchor1.transform.position.y.ToString ();
						string RZ = anchor1.transform.position.z.ToString ();
						btmodule.sendMsg (RX);
						btmodule.sendMsg (RY); 
						btmodule.sendMsg (RZ);

						Debug.Log ("objectPoseX:" + anchor2.transform.position.x * 100 + ", objectPoseY:" + anchor2.transform.position.y * 100 + ", objectPoseZ:" + anchor2.transform.position.z * 100);
						string pickX = anchor2.transform.position.x.ToString ();
						string pickY = anchor2.transform.position.y.ToString ();
						string pickZ = anchor2.transform.position.z.ToString ();
						btmodule.sendMsg (pickX);
						btmodule.sendMsg (pickY);
						btmodule.sendMsg (pickZ);

						Debug.Log ("PlaceX:" + anchor3.transform.position.x * 100 + ", PlaceY:" + anchor3.transform.position.y * 100 + ", PlaceZ:" + anchor3.transform.position.z * 100);
						string placeX = anchor3.transform.position.x.ToString ();
						string placeY = anchor3.transform.position.y.ToString ();
						string placeZ = anchor3.transform.position.z.ToString ();

						btmodule.sendMsg (placeX);
						btmodule.sendMsg (placeY);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
						btmodule.sendMsg (placeZ);
						doneOnce = false;
						clearedALL = false;
					   
					} //if (doneOnce) 

				   } //else
				}
			}//ButtonClicked


		// Fore resetting and selecting new object
		if (ButtonPressed) {

				Debug.Log ("Resetted the object");
				//robot.SetActive (false);
           		//targetObject1.SetActive (false);
    		    //targetObject2.SetActive (false);
			    GameObject.Destroy (targetObject1);
			    GameObject.Destroy (targetObject2);
			  
			    clearedALL = true;
				//doneOnce = false;
			    count =0;

			}


	  // Show the fit-to-scan overlay if there are no images that are Tracking.
		foreach (var visualizer in m_Visualizers.Values)
		{
			if (visualizer.Image.TrackingState == TrackingState.Tracking)

			{
				FitToScanOverlay.SetActive(false);
				return;
			}

		}

		FitToScanOverlay.SetActive(true);

//		if (ImageTracking) {
//			FitToScanOverlay.SetActive(false);
//			return;
//		}
//		FitToScanOverlay.SetActive(true);
//

 } //void update



		/// <summary>
		/// Check and update the application lifecycle.
		/// </summary>
		private void _UpdateApplicationLifecycle()
		{
			// Exit the app when the 'back' button is pressed.
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}

			// Only allow the screen to sleep when not tracking.
			if (Session.Status != SessionStatus.Tracking)
			{
				const int lostTrackingSleepTimeout = 15;
				Screen.sleepTimeout = lostTrackingSleepTimeout;
			}
			else
			{
				Screen.sleepTimeout = SleepTimeout.NeverSleep;
			}

			if (m_IsQuitting)
			{
				return;
			}

			// Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
			if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
			{
				_ShowAndroidToastMessage("Camera permission is needed to run this application.");
				m_IsQuitting = true;
				Invoke("_DoQuit", 0.5f);
			}
			else if (Session.Status.IsError())
			{
				_ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
				m_IsQuitting = true;
				Invoke("_DoQuit", 0.5f);
			}
		}

		/// <summary>
		/// Actually quit the application.
		/// </summary>
		private void _DoQuit()
		{
			Application.Quit();
		}

		/// <summary>
		/// Show an Android toast message.
		/// </summary>
		/// <param name="message">Message string to show in the toast.</param>
		private void _ShowAndroidToastMessage(string message)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			if (unityActivity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
					{
						AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
							message, 0);
						toastObject.Call("show");
					}));
			}
		}
	}

