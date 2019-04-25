using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This script will throw the current GameObject when the selecting finger performs a swipe
	[RequireComponent(typeof(Rigidbody))]
	public class LeanSelectableSwipeThrow : LeanSelectableBehaviour
	{
		[Tooltip("The camera we will be used")]
		public Camera Camera;

		[Tooltip("The throw force")]
		public float Force = 1.0f;

		public UnityEvent OnSwiped;

		// The cached rigidbody attached to this GameObject
		[System.NonSerialized]
		private Rigidbody cachedRigidbody;

		protected override void OnSelectUp(LeanFinger finger)
		{
			// Did this finger swipe?
			// NOTE: We manually calculate the swipe delta, because OnSelectUp will get called before the standard LeanTouch.OnFingerSwipe and thus never tell is if it's selected
			var swipeScaledDelta = finger.GetSnapshotScaledDelta(LeanTouch.CurrentTapThreshold);

			if (swipeScaledDelta.magnitude > LeanTouch.CurrentSwipeThreshold)
			{
				if (cachedRigidbody == null) cachedRigidbody = GetComponent<Rigidbody>();

				cachedRigidbody.AddForce(swipeScaledDelta * Force);

				OnSwiped.Invoke();
			}
		}
	}
}