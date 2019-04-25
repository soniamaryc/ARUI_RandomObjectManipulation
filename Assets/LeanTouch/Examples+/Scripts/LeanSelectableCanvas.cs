using UnityEngine;

namespace Lean.Touch
{
	// This script will make the current GameObject selectable when a finger begins pressing the Target RectTransform
	public class LeanSelectableCanvas : LeanSelectableBehaviour
	{
		[Tooltip("The RectTransform that must be pressed for selection to begin")]
		public RectTransform Target;

		protected override void OnEnable()
		{
			base.OnEnable();

			LeanTouch.OnFingerDown += FingerDown;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			LeanTouch.OnFingerDown -= FingerDown;
		}

		private void FingerDown(LeanFinger finger)
		{
			// Find GUI elements under finger
			var results = LeanTouch.RaycastGui(finger.ScreenPosition);

			if (results.Count > 0)
			{
				// If the first one is the target, select
				if (results[0].gameObject.transform == Target)
				{
					Selectable.Select(finger);
				}
			}
		}
	}
}