using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanManualRotate to be smooth
	public class LeanManualRotateSmooth : LeanManualRotate
	{
		[Tooltip("How quickly the rotation goes to the target value")]
		public float Dampening = 10.0f;

		[System.NonSerialized]
		private Quaternion remainingDelta = Quaternion.identity;

		public override void ResetRotation()
		{
			var finalTransform = Target != null ? Target.transform : transform;
			var oldRotation    = finalTransform.localRotation;

			// Rotate and increment by delta
			base.ResetRotation();

			remainingDelta *= Quaternion.Inverse(oldRotation) * finalTransform.localRotation;

			// Revert
			finalTransform.localRotation = oldRotation;
		}

		public override void Rotate(Vector2 delta)
		{
			var finalTransform = Target != null ? Target.transform : transform;
			var oldRotation    = finalTransform.localRotation;

			// Rotate and increment by delta
			base.Rotate(delta);

			remainingDelta *= Quaternion.Inverse(oldRotation) * finalTransform.localRotation;

			// Revert
			finalTransform.localRotation = oldRotation;
		}

		protected virtual void Update()
		{
			var finalTransform = Target != null ? Target.transform : transform;
			var factor         = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			var newDelta       = Quaternion.Slerp(remainingDelta, Quaternion.identity, factor);

			finalTransform.localRotation = finalTransform.localRotation * Quaternion.Inverse(newDelta) * remainingDelta;

			remainingDelta = newDelta;
		}
	}
}