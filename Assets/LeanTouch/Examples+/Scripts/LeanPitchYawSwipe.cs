using UnityEngine;

namespace Lean.Touch
{
	// This script will orbit the current GameObject
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class LeanPitchYawSwipe : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreIfStartedOverGui = true;

		[Tooltip("The amount the pitch/yaw changes with swipes in degrees")]
		public float SwipeAngle = 45.0f;

		[Tooltip("Pitch of the rotation in degrees")]
		[Space(10.0f)]
		public float Pitch;

		[Tooltip("Limit the pitch to min/max?")]
		public bool PitchClamp = true;

		[Tooltip("The minimum pitch angle in degrees")]
		public float PitchMin = -90.0f;

		[Tooltip("The maximum pitch angle in degrees")]
		public float PitchMax = 90.0f;

		[Tooltip("Yaw of the rotation in degrees")]
		[Space(10.0f)]
		public float Yaw;

		[Tooltip("Limit the yaw to min/max?")]
		public bool YawClamp;

		[Tooltip("The minimum yaw angle in degrees")]
		public float YawMin = -45.0f;

		[Tooltip("The maximum yaw angle in degrees")]
		public float YawMax = 45.0f;

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSwipe += OnFingerSwipe;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSwipe -= OnFingerSwipe;
		}

		protected virtual void LateUpdate()
		{
			// Limit pitch to min/max values
			if (PitchClamp == true)
			{
				Pitch = Mathf.Clamp(Pitch, PitchMin, PitchMax);
			}

			// Limit yaw to min/max values
			if (YawClamp == true)
			{
				Yaw = Mathf.Clamp(Yaw, YawMin, YawMax);
			}

			// Rotate to pitch and yaw values
			transform.localRotation = Quaternion.Euler(Pitch, Yaw, 0.0f);
		}

		private void OnFingerSwipe(LeanFinger finger)
		{
			// Ignore this swipe?
			if (IgnoreIfStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			var swipe = finger.SwipeScreenDelta;

			if (swipe.x < -Mathf.Abs(swipe.y))
			{
				Yaw += SwipeAngle;
			}

			if (swipe.x > Mathf.Abs(swipe.y))
			{
				Yaw -= SwipeAngle;
			}

			if (swipe.y < -Mathf.Abs(swipe.x))
			{
				Pitch -= SwipeAngle;
			}

			if (swipe.y > Mathf.Abs(swipe.x))
			{
				Pitch += SwipeAngle;
			}
		}
	}
}