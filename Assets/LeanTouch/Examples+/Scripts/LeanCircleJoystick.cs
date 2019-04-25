using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Touch
{
	// This component turns the current UI element into a joystick bound to a circle
	[RequireComponent(typeof(RectTransform))]
	public class LeanCircleJoystick : LeanCanvasDraggable
	{
		// Event signature
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		[Tooltip("The size limits of the joystick")]
		public float Size = 25.0f;

		[Tooltip("How quickly the joystick returns to the center when not being dragged")]
		public float Dampening = 5.0f;

		[Tooltip("The -1..1 x/y position of the joystick relative to the Size")]
		public Vector2 ScaledValue;

		public Vector2Event OnSet;

		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);

			if (dragging == true)
			{
				var anchoredPosition = TargetTransform.anchoredPosition;

				if (anchoredPosition.magnitude > Size)
				{
					anchoredPosition = anchoredPosition.normalized * Size;

					TargetTransform.anchoredPosition = anchoredPosition;
				}
			}

			UpdateScaledValue();
		}

		protected virtual void Update()
		{
			if (dragging == false)
			{
				var anchoredPosition = TargetTransform.anchoredPosition;

				// Get t value
				var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

				// Dampen the current position toward the target
				anchoredPosition = Vector2.Lerp(anchoredPosition, Vector2.zero, factor);

				// Write updated anchored position
				TargetTransform.anchoredPosition = anchoredPosition;

				UpdateScaledValue();
			}

			if (OnSet != null)
			{
				OnSet.Invoke(ScaledValue);
			}
		}

		private void UpdateScaledValue()
		{
			if (Size > 0.0f)
			{
				ScaledValue = TargetTransform.anchoredPosition / Size;

				if (ScaledValue.magnitude > 1.0f)
				{
					ScaledValue = ScaledValue.normalized;
				}
			}
			else
			{
				ScaledValue.x = 0.0f;
				ScaledValue.y = 0.0f;
			}
		}
	}
}