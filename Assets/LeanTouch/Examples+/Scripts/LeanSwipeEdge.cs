using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This calls an event when you swipe from the edge of the screen
	public class LeanSwipeEdge : MonoBehaviour
	{
		public bool Left = true;

		public bool Right = true;

		public bool Bottom = true;

		public bool Top = true;

		public float DirectionThreshold = 0.1f;

		public float EdgeThreshold = 10.0f;

		public UnityEvent OnSwipe;

		public void Swipe(LeanFinger finger)
		{
			var point  = finger.StartScreenPosition;
			var rect   = new Rect(0, 0, Screen.width, Screen.height);
			var vector = finger.SwipeScreenDelta.normalized;

			if (Left == true && CheckDirection(vector, Vector2.right) == true && CheckEdge(point.x - rect.xMin) == true)
			{
				Swipe(); return;
			}
			else if (Right == true && CheckDirection(vector, -Vector2.right) == true && CheckEdge(point.x - rect.xMax) == true)
			{
				Swipe(); return;
			}
			else if (Bottom == true && CheckDirection(vector, Vector2.up) == true && CheckEdge(point.y - rect.yMin) == true)
			{
				Swipe(); return;
			}
			else if (Top == true && CheckDirection(vector, -Vector2.up) == true && CheckEdge(point.y - rect.yMax) == true)
			{
				Swipe(); return;
			}
		}

		private void Swipe()
		{
			if (OnSwipe != null)
			{
				OnSwipe.Invoke();
			}
		}

		private bool CheckDirection(Vector2 a, Vector2 b)
		{
			a = a.normalized;
			b = b.normalized;

			return Vector2.Dot(a, b) > 1.0f - DirectionThreshold;
		}

		private bool CheckEdge(float distance)
		{
			return Mathf.Abs(distance * LeanTouch.ScalingFactor) < EdgeThreshold;
		}
	}
}