using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to tilt & pan the current GameObject (e.g. camera) by dragging your finger(s)
	[ExecuteInEditMode]
	public class LeanPitchYaw : MonoBehaviour
	{
		[Tooltip("If you want the rotation to be scaled by the camera FOV, then set that here")]
		public Camera Camera;

		[Tooltip("If you want to rotate a different GameObject, then specify it here")]
		public GameObject Target;

		[Tooltip("Pitch of the rotation in degrees")]
		[Space(10.0f)]
		public float Pitch;

		[Tooltip("The strength of the pitch changes with vertical finger movement")]
		public float PitchSensitivity = 0.25f;

		[Tooltip("Limit the pitch to min/max?")]
		public bool PitchClamp = true;

		[Tooltip("The minimum pitch angle in degrees")]
		public float PitchMin = -90.0f;

		[Tooltip("The maximum pitch angle in degrees")]
		public float PitchMax = 90.0f;

		[Tooltip("Yaw of the rotation in degrees")]
		[Space(10.0f)]
		public float Yaw;

		[Tooltip("The strength of the yaw changes with horizontal finger movement")]
		public float YawSensitivity = 0.25f;

		[Tooltip("Limit the yaw to min/max?")]
		public bool YawClamp;

		[Tooltip("The minimum yaw angle in degrees")]
		public float YawMin = -45.0f;

		[Tooltip("The maximum yaw angle in degrees")]
		public float YawMax = 45.0f;

		public void RotateToFinger(LeanFinger finger)
		{
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var xyz       = finger.GetRay(camera).direction;
				var longitude = Mathf.Atan2(xyz.x, xyz.z);
				var latitude  = Mathf.Asin(xyz.y / xyz.magnitude);

				Pitch = latitude  * -Mathf.Rad2Deg;
				Yaw   = longitude *  Mathf.Rad2Deg;
			}
		}

		public void Rotate(Vector2 delta)
		{
			var sensitivity = GetSensitivity();

			Yaw   += delta.x *   YawSensitivity * sensitivity;
			Pitch -= delta.y * PitchSensitivity * sensitivity;
		}

		public virtual void UpdateRotation()
		{
			if (PitchClamp == true)
			{
				Pitch = Mathf.Clamp(Pitch, PitchMin, PitchMax);
			}

			if (YawClamp == true)
			{
				Yaw = Mathf.Clamp(Yaw, YawMin, YawMax);
			}

			// Rotate to pitch and yaw values
			var finalTransform = Target != null ? Target.transform : transform;

			finalTransform.localRotation = Quaternion.Euler(Pitch, Yaw, 0.0f);
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Start();
		}
#endif

		protected virtual void Start()
		{
			if (Camera == null)
			{
				Camera = GetComponent<Camera>();
			}
		}

		protected virtual void LateUpdate()
		{
			UpdateRotation();
		}

		private float GetSensitivity()
		{
			// Has a camera been set?
			if (Camera != null)
			{
				// Adjust sensitivity by FOV?
				if (Camera.orthographic == false)
				{
					return Camera.fieldOfView / 90.0f;
				}
			}

			return 1.0f;
		}
	}
}