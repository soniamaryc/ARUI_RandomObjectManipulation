using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to face the tap direction
	[RequireComponent(typeof(LeanPitchYaw))]
	public class LeanPitchYawTapToFace : MonoBehaviour
	{
		[Tooltip("The camera used for facing")]
		public Camera Camera;

		[System.NonSerialized]
		private LeanPitchYaw cachedPitchYaw;

		protected virtual void OnEnable()
		{
			cachedPitchYaw = GetComponent<LeanPitchYaw>();

			LeanTouch.OnFingerTap += FingerTap;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerTap -= FingerTap;
		}

		private void FingerTap(LeanFinger finger)
		{
			var xyz       = finger.GetWorldPosition(1.0f, Camera);
			var longitude = Mathf.Atan2(xyz.x, xyz.z);
			var latitude  = Mathf.Asin(xyz.y / xyz.magnitude);

			cachedPitchYaw.Pitch = latitude;
			cachedPitchYaw.Yaw   = longitude;
		}
	}
}