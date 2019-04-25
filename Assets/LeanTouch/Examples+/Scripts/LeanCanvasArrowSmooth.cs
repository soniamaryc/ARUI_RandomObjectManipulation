using UnityEngine;

namespace Lean.Touch
{
	// This component modifies LeanCanvasArrow to be smooth
	public class LeanCanvasArrowSmooth : LeanCanvasArrow
	{
		[Tooltip("How quickly the angle reaches the target value")]
		public float Dampening = 10.0f;

		private float currentAngle;

		protected override void Update()
		{
			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Lerp angle
			currentAngle = Mathf.LerpAngle(currentAngle, Angle, factor);

			// Update rotation
			transform.rotation = Quaternion.Euler(0.0f, 0.0f, -currentAngle);
		}
	}
}