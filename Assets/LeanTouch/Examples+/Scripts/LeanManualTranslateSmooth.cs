using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanManualTranslate to be smooth
	public class LeanManualTranslateSmooth : LeanManualTranslate
	{
		[Tooltip("How quickly the position goes to the target value")]
		public float Dampening = 10.0f;

		[System.NonSerialized]
		private Vector3 remainingDelta;

		public override void Translate(Vector2 delta)
		{
			var finalTransform = Target != null ? Target.transform : transform;
			var oldPosition    = finalTransform.localPosition;

			// Translate and increment by delta
			base.Translate(delta);

			remainingDelta += finalTransform.localPosition - oldPosition;

			// Revert
			finalTransform.localPosition = oldPosition;
		}

		protected virtual void Update()
		{
			var finalTransform = Target != null ? Target.transform : transform;
			var factor         = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			var newDelta       = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			finalTransform.localPosition += remainingDelta - newDelta;

			remainingDelta = newDelta;
		}
	}
}