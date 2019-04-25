using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to rotate the current GameObject using a twist gesture
	public class LeanCameraTwist : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does rotation require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The camera we will be used to calculate relative rotations (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("Should the rotation be performanced relative to the finger center?")]
		public bool Relative;

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
			var fingers = LeanSelectable.GetFingersOrClear(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount, RequiredSelectable);

			// Calculate the rotation values based on these fingers
			var twistDegrees = -LeanGesture.GetTwistDegrees(fingers);

			if (Relative == true)
			{
				var twistScreenCenter = LeanGesture.GetScreenCenter(fingers);

				Translate(twistDegrees, twistScreenCenter);
				Rotate(twistDegrees);
			}
			else
			{
				Rotate(twistDegrees);
			}
		}

		protected virtual void Translate(float twistDegrees, Vector2 twistScreenCenter)
		{
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				// Screen position of the transform
				var screenPoint = camera.WorldToScreenPoint(transform.position);

				// Twist screen point around the twistScreenCenter by twistDegrees
				var twistRotation = Quaternion.Euler(0.0f, 0.0f, twistDegrees);
				var screenDelta   = twistRotation * ((Vector2)screenPoint - twistScreenCenter);

				screenPoint.x = twistScreenCenter.x + screenDelta.x;
				screenPoint.y = twistScreenCenter.y + screenDelta.y;

				// Convert back to world space
				transform.position = camera.ScreenToWorldPoint(screenPoint);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}

		protected virtual void Rotate(float twistDegrees)
		{
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var axis = transform.InverseTransformDirection(camera.transform.forward);

				transform.rotation *= Quaternion.AngleAxis(twistDegrees, axis);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}