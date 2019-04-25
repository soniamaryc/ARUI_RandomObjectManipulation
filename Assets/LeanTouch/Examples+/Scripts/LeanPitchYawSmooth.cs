using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanPitchYaw to be smooth
	public class LeanPitchYawSmooth : LeanPitchYaw
	{
		[Tooltip("How sharp the rotation value changes update")]
		[Space(10.0f)]
		public float Dampening = 3.0f;

		[System.NonSerialized]
		private float currentPitch;

		[System.NonSerialized]
		private float currentYaw;

		public override void UpdateRotation()
		{
			base.UpdateRotation();

			if (PitchClamp == true)
			{
				currentPitch = Mathf.Clamp(currentPitch, PitchMin, PitchMax);
			}

			if (YawClamp == true)
			{
				currentYaw = Mathf.Clamp(currentYaw, YawMin, YawMax);
			}

			// Rotate to pitch and yaw values
			var finalTransform = Target != null ? Target.transform : transform;

			finalTransform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
		}

		protected virtual void OnEnable()
		{
			currentPitch = Pitch;
			currentYaw   = Yaw;
		}

		protected override void LateUpdate()
		{
			// Call LeanPitchYaw.LateUpdate
			base.LateUpdate();

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Lerp the current values to the target ones
			currentPitch = Mathf.Lerp(currentPitch, Pitch, factor);
			currentYaw   = Mathf.Lerp(currentYaw  , Yaw  , factor);

			// Rotate to pitch and yaw values
			var finalTransform = Target != null ? Target.transform : transform;

			finalTransform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
		}
	}
}