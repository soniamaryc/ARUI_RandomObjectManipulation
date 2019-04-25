using UnityEngine;

namespace Lean.Touch
{
	// This script modifies LeanSnapAlong to be smooth
	public class LeanSnapAlongSmooth : LeanSnapAlong
	{
		[Tooltip("How quickly the position value changes update (-1 = instant).")]
		public float Dampening = 3.0f;

		private Vector3 remainingDelta;

		protected override void Update()
		{
			// Store smoothed position
			var smoothPosition = transform.localPosition;

			// Snap to target
			transform.localPosition = transform.localPosition + remainingDelta;

			// Store old position
			var oldPosition = transform.localPosition;

			// Update to new position
			base.Update();

			// Shift delta by old new delta
			remainingDelta += transform.localPosition - oldPosition;

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Dampen remainingDelta
			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			// Shift this position by the change in delta
			transform.localPosition = smoothPosition + remainingDelta - newDelta;

			// Update remainingDelta with the dampened value
			remainingDelta = newDelta;
		}
	}
}