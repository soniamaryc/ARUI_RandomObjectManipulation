using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Lean.Touch
{
	// This component allows you to resize the Target RectTransform, or the current one
	[RequireComponent(typeof(RectTransform))]
	public class LeanCanvasResizable : Selectable, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[Tooltip("By default this GameObject will be resized when dragging, but you can override that here")]
		public RectTransform Target;

		[Header("Horizontal")]
		[Tooltip("Horizontal resize strength (0 = none, 1 = normal, -1 = inverted, 2 = centered)")]
		public float WidthScale = 1.0f;

		[Tooltip("Limit the horizontal size?")]
		public bool LimitWidth;

		[Tooltip("Minimum horizontal size")]
		public float MinWidth = 50.0f;

		[Tooltip("Maximum horizontal size")]
		public float MaxWidth = 500.0f;

		[Header("Vertical")]
		[Tooltip("Vertical resize strength (0 = none, 1 = normal, -1 = inverted, 2 = centered)")]
		public float HeightScale = 1.0f;

		[Tooltip("Limit the vertical size?")]
		public bool LimitHeight;

		[Tooltip("Minimum vertical size")]
		public float MinHeight = 50.0f;

		[Tooltip("Maximum vertical size")]
		public float MaxHeight = 500.0f;

		[System.NonSerialized]
		private bool dragging;

		[System.NonSerialized]
		private Vector2 startSize;

		[System.NonSerialized]
		private Vector2 startOffset;

		[System.NonSerialized]
		private RectTransform rectTransform;

		public RectTransform TargetTransform
		{
			get
			{
				if (Target != null)
				{
					return Target;
				}

				if (rectTransform == null)
				{
					rectTransform = GetComponent<RectTransform>();
				}

				return rectTransform;
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform>();
			}

			if (MayDrag(eventData) == true)
			{
				var target = TargetTransform;

				if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, eventData.enterEventCamera) == true)
				{
					var vector = default(Vector2);

					if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out vector) == true)
					{
						dragging    = true;
						startSize   = target.sizeDelta;
						startOffset = vector - target.anchoredPosition;
					}
				}
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform>();
			}

			if (dragging == true)
			{
				if (MayDrag(eventData) == true)
				{
					var vector = default(Vector2);
					var target = TargetTransform;

					if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out vector) == true)
					{
						var offsetDelta = (vector - target.anchoredPosition) - startOffset;
						var   sizeDelta = target.sizeDelta;

						sizeDelta.x = startSize.x + offsetDelta.x * WidthScale;

						if (LimitWidth == true)
						{
							sizeDelta.x = Mathf.Clamp(sizeDelta.x, MinWidth, MaxWidth);
						}

						sizeDelta.y = startSize.y + offsetDelta.y * HeightScale;

						if (LimitHeight == true)
						{
							sizeDelta.y = Mathf.Clamp(sizeDelta.y, MinHeight, MaxHeight);
						}

						target.sizeDelta = sizeDelta;
					}
				}
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			dragging = false;
		}
	}
}