using UnityEngine;

namespace Lean.Touch
{
	// This component modifies LeanRotate to be smooth
	public class LeanRotateSmooth : LeanRotate
	{
		[Tooltip("How smoothly this object moves to its target position")]
		public float Dampening = 10.0f;

		[System.NonSerialized]
		private Vector3 remainingTranslation;

		[System.NonSerialized]
		private Quaternion remainingRotation = Quaternion.identity;

		protected override void Update()
		{
			// Store
			var oldPosition = transform.localPosition;
			var oldRotation = transform.localRotation;

			// Update
			base.Update();

			// Increment
			remainingTranslation += transform.localPosition - oldPosition;
			remainingRotation    *= Quaternion.Inverse(oldRotation) * transform.localRotation;

			// Revert
			transform.localRotation = oldRotation;
		}

		protected virtual void LateUpdate()
		{
			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Dampen remainingDelta
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);
			var newRemainingRotation    = Quaternion.Slerp(remainingRotation, Quaternion.identity, factor);

			// Shift this transform by the change in delta
			transform.localPosition += remainingTranslation - newRemainingTranslation;
			transform.localRotation  = transform.localRotation * Quaternion.Inverse(newRemainingRotation) * remainingRotation;

			// Update remainingDelta with the dampened value
			remainingTranslation = newRemainingTranslation;
			remainingRotation    = newRemainingRotation;
		}
	}
}