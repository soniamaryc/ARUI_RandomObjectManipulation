using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Touch
{
	// This component allows you to make a quick button that fires as soon as your finger presses down on it
	[RequireComponent(typeof(RectTransform))]
	public class LeanCanvasDown : Selectable, IPointerDownHandler
	{
		// Called when pressing
		public UnityEvent OnDown;

		[Tooltip("If one pointer is currently down on this button, ignore any other pointers?")]
		public bool IgnoreMultiple = true;

		[System.NonSerialized]
		private int count;

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);

			count++;

			if (IgnoreMultiple == true && count > 1)
			{
				return;
			}

			if (OnDown != null)
			{
				OnDown.Invoke();
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);

			count--;
		}
	}
}