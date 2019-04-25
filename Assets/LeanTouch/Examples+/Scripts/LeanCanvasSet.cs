using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Touch
{
	// This component allows you to perform an action every frame a pointer is pressed 
	[RequireComponent(typeof(RectTransform))]
	public class LeanCanvasSet : Selectable, IPointerDownHandler
	{
		// Called every frame the conditions are met
		public UnityEvent OnSet;

		[Tooltip("Does the pointer need to be over the RectTransform all the time?")]
		public bool RequireOver = true;

		[System.NonSerialized]
		private List<int> downPointers = new List<int>();

		[System.NonSerialized]
		private List<int> enterPointers = new List<int>();

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);

			downPointers.Add(eventData.pointerId);
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);

			downPointers.Remove(eventData.pointerId);
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);

			enterPointers.Add(eventData.pointerId);
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);

			enterPointers.Remove(eventData.pointerId);
		}

		protected virtual void Update()
		{
			for (var i = downPointers.Count - 1; i >= 0; i--)
			{
				var pointerId = downPointers[i];

				if (RequireOver == true)
				{
					if (enterPointers.Contains(pointerId) == false)
					{
						continue;
					}
				}

				if (OnSet != null)
				{
					OnSet.Invoke();
				}

				return;
			}
		}
	}
}