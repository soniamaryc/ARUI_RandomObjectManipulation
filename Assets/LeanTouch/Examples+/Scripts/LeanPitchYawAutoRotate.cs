using UnityEngine;

namespace Lean.Touch
{
	// This component adds auto rotation to LeanPitchYaw
	[RequireComponent(typeof(LeanPitchYaw))]
	public class LeanPitchYawAutoRotate : MonoBehaviour
	{
		[Tooltip("The amount of seconds until auto rotation begins after no touches")]
		public float Delay = 5.0f;

		[Tooltip("The speed of the yaw changes")]
		public float Speed = 5.0f;

		[Tooltip("The speed the auto rotation goes from 0% to 100%")]
		public float Acceleration = 1.0f;

		private float idleTime;

		private float strength;

		private float expectedPitch;

		private float expectedYaw;

		[System.NonSerialized]
		private LeanPitchYaw cachedPitchYaw;

		protected virtual void OnEnable()
		{
			cachedPitchYaw = GetComponent<LeanPitchYaw>();
		}

		protected virtual void LateUpdate()
		{
			if (cachedPitchYaw.Pitch == expectedPitch && cachedPitchYaw.Yaw == expectedYaw)
			{
				idleTime += Time.deltaTime;

				if (idleTime >= Delay)
				{
					strength += Acceleration * Time.deltaTime;

					cachedPitchYaw.Yaw += Mathf.Clamp01(strength) * Speed * Time.deltaTime;

					cachedPitchYaw.UpdateRotation();
				}
			}
			else
			{
				idleTime = 0.0f;
				strength = 0.0f;
			}

			expectedPitch = cachedPitchYaw.Pitch;
			expectedYaw   = cachedPitchYaw.Yaw;
		}
	}
}