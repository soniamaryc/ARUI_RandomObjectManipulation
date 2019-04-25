using UnityEngine;

namespace Lean.Touch
{
	// This component alows you to translate the current GameObject along the specified surface
	public class LeanTranslateAlong : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does translation require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The camera we will be used (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("The conversion method used to find a world point from a screen point")]
		public LeanScreenDepth ScreenDepth;

		[Tooltip("If your ScreenDepth settings cause the position values to clamp, there will be a difference between where the finger is and where the object is. Should this difference be tracked?")]
		public bool TrackScreenPosition = true;

		[System.NonSerialized]
		private Vector2 deltaDifference;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Start();
		}
#endif

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponent<LeanSelectable>();
			}
		}

		protected virtual void Update()
		{
			// Get the fingers we want to use
			var fingers = LeanSelectable.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount, RequiredSelectable);

			// Calculate the screenDelta value based on these fingers
			var screenDelta = LeanGesture.GetScreenDelta(fingers);

			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (fingers.Count == 0)
			{
				deltaDifference = Vector2.zero;
			}

			if (camera != null)
			{
				if (TrackScreenPosition == true)
				{
					var oldScreenPoint = camera.WorldToScreenPoint(transform.position);
					var worldPosition  = default(Vector3);

					if (ScreenDepth.TryConvert(ref worldPosition, oldScreenPoint + (Vector3)(screenDelta + deltaDifference), camera, gameObject) == true)
					{
						transform.position = worldPosition;
					}

					var newScreenPoint = camera.WorldToScreenPoint(transform.position);
					var oldNewDelta    = (Vector2)(newScreenPoint - oldScreenPoint);

					deltaDifference += screenDelta - oldNewDelta;
				}
				else
				{
					var oldScreenPoint = camera.WorldToScreenPoint(transform.position);
					var worldPosition  = default(Vector3);

					if (ScreenDepth.TryConvert(ref worldPosition, oldScreenPoint + (Vector3)screenDelta, camera, gameObject) == true)
					{
						transform.position = worldPosition;
					}
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}