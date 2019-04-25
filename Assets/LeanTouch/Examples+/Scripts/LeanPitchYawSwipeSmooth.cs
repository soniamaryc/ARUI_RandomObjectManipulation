using UnityEngine;

namespace Lean.Touch
{
	// This script modifies LeanOrbitCameraSwipe to be smooth
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class LeanPitchYawSwipeSmooth : LeanPitchYawSwipe
	{
		[Tooltip("How sharp the rotation value changes update")]
		[Space(10.0f)]
		public float Dampening = 3.0f;

		private float currentPitch;

		private float currentYaw;

		protected override void OnEnable()
		{
			// Call LeanPitchYawSwipe.LateUpdate
			base.OnEnable();

			currentPitch = Pitch;
			currentYaw   = Yaw;
		}

		protected override void LateUpdate()
		{
			// Call LeanPitchYawSwipe.LateUpdate
			base.LateUpdate();

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Lerp the current values to the target ones
			currentPitch = Mathf.Lerp(currentPitch, Pitch, factor);
			currentYaw   = Mathf.Lerp(currentYaw  , Yaw  , factor);

			// Rotate camera to pitch and yaw values
			transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
		}
	}
}