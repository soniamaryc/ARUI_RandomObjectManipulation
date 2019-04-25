using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This component allows you to accumilate value changes until they reach a threshold value, and then output them in fixed steps
	public class LeanStepValue2D : MonoBehaviour
	{
		// Event signature
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		[Tooltip("The current accumilated value type")]
		public Vector2 Value;

		[Tooltip("The step required to call OnStep")]
		public Vector2 Step = Vector2.one;

		public Vector2Event OnStep;

		public void AddValue(Vector2 delta)
		{
			Value += delta;
		}

		protected virtual void Update()
		{
			var stepX = (int)(Value.x / Step.x);
			var stepY = (int)(Value.y / Step.y);

			if (stepX != 0 || stepY != 0)
			{
				var deltaX = stepX * Step.x;
				var deltaY = stepY * Step.y;

				Value.x -= deltaX;
				Value.y -= deltaY;

				if (OnStep != null)
				{
					OnStep.Invoke(new Vector2(deltaX, deltaY));
				}
			}
		}
	}
}