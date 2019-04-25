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
public class VerticalManipulator : MonoBehaviour
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

    private Dictionary<int, AugmentedImageVisualizer> m_Visualizers = new Dictionary<int, AugmentedImageVisualizer>();

    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();


    //New variables
    BtAutoScript btmodule;
    buttonScript buttonModule;
    bool ButtonClicked;
    bool ButtonPressed;
    bool ButtonSend;
    bool doneOnce = false;
    bool clearedALL = true;
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

    //bool ImageTracking = false;
    int count = 0;
    Vector3 thePosition;
    // public int reachscale =2;
    // Vector3 offsetVector = new Vector3(0,0,-0.01);
    public Vector3 objectScale1;
    public Vector3 objectScale2;
    public Vector3 objectPose;
    public float angleTurned1;
    public float angleTurned2;
    float xVal_initial;
    float yVal_initial;
    float xVal_final;
    float yVal_final;

    public void Start()
    {
        btmodule = gameObject.GetComponent<BtAutoScript>();
        buttonModule = gameObject.GetComponent<buttonScript>();
        //ButtonClicked = gameObject.GetComponent<buttonScript> ().isClicked;
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    /// 

    public void Update()
    {


        if (targetObject1 != null)
        {
            objectScale1 = targetObject1.transform.localScale;
            //objectPose = targetObject1.transform.position;
            
        }
        if (targetObject2 != null)
        {
            objectScale2 = targetObject2.transform.localScale;
           
        }



        ButtonClicked = buttonModule.isClicked;
        // Debug.Log("Button status:"+ ButtonClicked);
        ButtonPressed = buttonModule.isPressed;
        ButtonSend = buttonModule.isSent;
        //		Debug.Log("Sent Button status:"+ ButtonSend);
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
                //visualizer.gameObject.SetActive (false); // this line is fortnot showing the frame.

                //------------------------- Adding a visual for showing the robot workspace----------------------------
                // Not updating it always. Detecting and storin the values only once at the beginning.
                //-----------------------------------------------------------------------------------------------------
                markerCenter = Instantiate(MarkerPrefab, anchor1.transform.position, anchor1.transform.rotation);
                markerCenter.transform.parent = anchor1.transform;
                Vector3 worklocation = (anchor1.transform.position + anchor1.transform.forward * 0.095f); // adjusting the workspace. The origin of the workspace plane is ahead of the anchor origin
                robotworkspace = Instantiate(workspace, worklocation, anchor1.transform.rotation);//Quaternion.identity);
                robotworkspace.transform.parent = anchor1.transform;
                
            }
            else if (image.TrackingState == TrackingState.Stopped && visualizer != null)

            {
                m_Visualizers.Remove(image.DatabaseIndex);
                //GameObject.Destroy (markerCenter.gameObject);
                markerCenter.gameObject.SetActive(false);

                GameObject.Destroy(visualizer.gameObject);
                visualizer.gameObject.SetActive(false);
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

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (ButtonClicked && !doneOnce && clearedALL)
        {
            //Debug.Log ("Button clicked");
            //if (Frame.Raycast (touch.position.x, touch.position.y, raycastFilter, out hit)) {
            if (Frame.Raycast((float)Screen.width / 2, (float)Screen.height / 2, raycastFilter, out hit))
            {

                // Use hit pose and camera pose to check if hittest is from the back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) && Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {


                    //Instantiate PICK or PLACE at the hit poses.
                    count = count + 1;
                    if (count == 1)
                    {
                        anchor2 = hit.Trackable.CreateAnchor(hit.Pose);
                        targetObject1 = Instantiate(pickPrefab, hit.Pose.position, anchor1.transform.rotation);
                        //targetObject1.transform.Rotate(0, k_ModelRotation, 0, Space.Self);
                        targetObject1.transform.parent = anchor2.transform;
                        //count = count + 1;
                    }

                    else
                    {
                        anchor3 = hit.Trackable.CreateAnchor(hit.Pose);

                        targetObject2 = Instantiate(placePrefab, hit.Pose.position, anchor1.transform.rotation);
                        //targetObject2.transform.Rotate(0, k_ModelRotation, 0, Space.Self);	
                        //Default scale of the place object is same as pick object:
                        targetObject2.transform.localScale = new Vector3(objectScale1.x,0.001f,objectScale1.z);
                        targetObject2.transform.parent = anchor3.transform;
                        count = 0;
                        doneOnce = true;
                    }


                } //else
            }
        }//ButtonClicked
         // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
         //targetObject.transform.Rotate (0, k_ModelRotation, 0, Space.Self);

        //................................. Non homogenous transformation.....................................
        //............................Sending Marker (angle,position) and object's pick and pose (position) to the Manipulator
        //.....................................................................................................

        if (doneOnce && ButtonSend)
        {
            //1: Calcualting the coordiantes in robot coordiante system
            CalculateCoordiantes();
            //2: Sending the data to the robot
            SendData();
            //3:Clearing the flags
            doneOnce = false;
            clearedALL = false;
            ButtonSend = false;

        } //if (doneOnce) 


        // Fore resetting and selecting new object
        if (ButtonPressed)
        {

            if (targetObject1 != null)
            {
                objectScale1 = Vector3.zero;
                objectScale2 = Vector3.zero;
                angleTurned1 = 0.0f;
                angleTurned2 = 0.0f;
            }

            Debug.Log("Resetted the object");
            //robot.SetActive (false);
            //targetObject1.SetActive (false);
            //targetObject2.SetActive (false);
            GameObject.Destroy(targetObject1);
            GameObject.Destroy(targetObject2);
            ARCoreSession.Destroy(anchor2);
            ARCoreSession.Destroy(anchor3);
            clearedALL = true;
            doneOnce = false;
            count = 0;

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

        
    } //void update


    private void CalculateCoordiantes()
    {
        float deg2rad = (float)(Mathf.PI / 180.0f);

        // The  marker alpha,beta and gamma
        float alpha = anchor1.transform.eulerAngles.x * deg2rad;
        float beta = anchor1.transform.eulerAngles.y * deg2rad;
        float gamma = anchor1.transform.eulerAngles.x * deg2rad;
        float Rx = anchor1.transform.position.x;
        float Ry = anchor1.transform.position.y;
        float Rz = anchor1.transform.position.z;

        //Pick			
        float Ox1 = targetObject1.transform.position.x;
        float Oy1 = targetObject1.transform.position.y;
        float Oz1 = targetObject1.transform.position.z;

        //Place			
        float Ox2 = targetObject2.transform.position.x;
        float Oy2 = targetObject2.transform.position.y;
        float Oz2 = targetObject2.transform.position.z;


        float[,] rot = new float[3, 3] { {Mathf.Cos(alpha)*Mathf.Cos(beta), (Mathf.Cos(alpha)*Mathf.Sin(beta)*Mathf.Sin(gamma) - Mathf.Sin(alpha)*Mathf.Cos(gamma)), (Mathf.Cos(alpha)*Mathf.Sin(beta)*Mathf.Cos(gamma) + Mathf.Sin(alpha)*Mathf.Sin(gamma))},
                {Mathf.Sin(alpha)*Mathf.Cos(beta), ((Mathf.Sin(alpha)*Mathf.Sin(beta)*Mathf.Sin(gamma)) - (Mathf.Cos(alpha)*Mathf.Cos(gamma))),((Mathf.Sin(alpha)*Mathf.Sin(beta)*Mathf.Cos(gamma)) - (Mathf.Cos(alpha)*Mathf.Sin(gamma)))},
                {-Mathf.Sin(beta), Mathf.Cos(beta)*Mathf.Sin(gamma), Mathf.Cos(beta)*Mathf.Cos(gamma)} };


        //Transposing rot, rRc= Transpose of cRr
        //Finding : Calcualting rRc
        float[,] rotTrans = new float[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
                rotTrans[j, i] = rot[i, j];
        }

        float[,] B = new float[3, 1] { {-(rotTrans[0,0] * Rx + rotTrans[0,1] * Ry + rotTrans[0,2] * Rz)},
                {-(rotTrans[1,0] * Rx + rotTrans[1,1] * Ry + rotTrans[1,2] * Rz)},
                {-(rotTrans[2,0] * Rx + rotTrans[2,1] * Ry + rotTrans[2,2] * Rz)} };

        //Picking object
        float[,] pickpoint = new float[3, 1] { {rotTrans[0,0] * Ox1 + rotTrans[0,1] * Oy1 + rotTrans[0,2] * Oz1},
                {rotTrans[1,0] * Ox1 + rotTrans[1,1] * Oy1 + rotTrans[1,2] * Oz1},
                {rotTrans[2,0] * Ox1 + rotTrans[2,1] * Oy1 + rotTrans[2,2] * Oz1}};


        float[,] pickLocation = new float[3, 1] { {pickpoint[0,0] + B[0,0]},
                {pickpoint[1,0] + B[1,0]},
                {pickpoint[2,0] + B[2,0]} };

        //Placing object
        float[,] placepoint = new float[3, 1] { {rotTrans[0,0] * Ox2 + rotTrans[0,1] * Oy2 + rotTrans[0,2] * Oz2},
                {rotTrans[1,0] * Ox2 + rotTrans[1,1] * Oy2+ rotTrans[1,2] * Oz2},
                {rotTrans[2,0] * Ox2 + rotTrans[2,1] * Oy2 + rotTrans[2,2] * Oz2}};

        float[,] placeLocation = new float[3, 1] { {placepoint[0,0] + B[0,0]},
                {placepoint[1,0] + B[1,0]},
                {placepoint[2,0] + B[2,0]} };


        //float Xoffset=0;
        //float Yoffset=0; // How far the marker place from the Odometric center of the robot.
         xVal_initial = (pickLocation[0, 0]);  //+ Xoffset);  
         yVal_initial = (pickLocation[2, 0]);  //+ Yoffset);

        xVal_final = (placeLocation[0, 0]);  //+ Xoffset);  
        yVal_final = (placeLocation[2, 0]);  //+ Yoffset);


    }

    private void SendData()
    {

        ///Sending the X and Y position inforamtion to the Computer
        //---------- order of sending ( pick (x,y), place (x,y), scale, pick angle, place angle -------- //
        string xPos1 = xVal_initial.ToString();
        string yPos1 = yVal_initial.ToString();
        btmodule.sendMsg(xPos1);
        btmodule.sendMsg(yPos1);
        string xPos2 = xVal_final.ToString();
        string yPos2 = yVal_final.ToString();
        btmodule.sendMsg(xPos2);
        btmodule.sendMsg(yPos2);

        // ......... Object width, height and orientation.........

        string widthValue = (objectScale1.x / 2).ToString();
        string pickHeightValue = (objectScale1.y / 2).ToString();
        string PlaceHeightValue = ((objectScale2.y+ objectScale1.y )/  2).ToString(); // Default height for palcing is the same as the picking height.
                                                                                     // If we want to place it at a level, adding the new height to the picking height.

        btmodule.sendMsg(widthValue);
        btmodule.sendMsg(pickHeightValue);
        btmodule.sendMsg(PlaceHeightValue);
    }




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



    public void PickScaleYAxis(float newValue)
    {

        Vector3 scaletemp = targetObject1.transform.localScale;
        //Vector3 scaletemp=controllerscript.objectScale;
        scaletemp.y = newValue;
        targetObject1.transform.localScale = scaletemp;
    }

    public void PlaceScaleYAxis(float newValue)
    {

        Vector3 scaletemp = targetObject2.transform.localScale;
        scaletemp.y = newValue;
        targetObject2.transform.localScale = scaletemp;
    }

    public void PlusScaleXAxis(float newValue)
    {

        Vector3 scaletemp = targetObject1.transform.localScale;
        //Vector3 scaletemp=controllerscript.objectScale;
        scaletemp.x = newValue;
        targetObject1.transform.localScale = scaletemp;
    }





}


