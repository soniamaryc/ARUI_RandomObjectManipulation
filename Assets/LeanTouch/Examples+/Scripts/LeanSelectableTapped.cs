using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This script fires an event when the selecting finger releases within tap threshold seconds
	// NOTE: This won't work with tap to select, because that makes no sense and you can hook into LeanSelectable's OnSelect to do that
	public class LeanSelectableTapped : LeanSelectableBehaviour
	{
		public UnityEvent onSelectableTapped;

		protected override void OnSelectUp(LeanFinger finger)
		{
			if (finger.Age <= LeanTouch.CurrentTapThreshold)
			{
				onSelectableTapped.Invoke();
			}
		}
	}
}