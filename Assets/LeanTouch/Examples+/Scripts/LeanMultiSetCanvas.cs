using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This modifies LeanMultiSet to require each finger to begin over a RectTransform
	public class LeanMultiSetCanvas : LeanMultiSet
	{
		[Tooltip("Each finger must begin pressing down on this RectTransform for it to be counted.")]
		public GameObject Target;

		private List<LeanFinger> validFingers = new List<LeanFinger>();

		private List<LeanFinger> filteredFingers = new List<LeanFinger>();

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown += FingerDown;
			LeanTouch.OnFingerUp   += FingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= FingerDown;
			LeanTouch.OnFingerUp   -= FingerUp;
		}

		protected override void Update()
		{
			// Get fingers
			filteredFingers.Clear();

			filteredFingers.AddRange(validFingers);

			if (RequiredSelectable != null)
			{
				if (RequiredSelectable.IsSelected == false)
				{
					filteredFingers.Clear();
				}
			}

			UpdateFingers(filteredFingers);
		}

		private void FingerDown(LeanFinger finger)
		{
			var results = LeanTouch.RaycastGui(finger.ScreenPosition);

			if (results.Count > 0)
			{
				if (results[0].gameObject == Target)
				{
					validFingers.Add(finger);
				}
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			validFingers.Remove(finger);
		}
	}
}