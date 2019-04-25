using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Touch
{
	public class LeanCard : LeanSelectableBehaviour
	{
		public bool Anchor;

		public bool AutoGetAnchorPoint;

		public Vector2 AnchorPoint;

		public Vector3 Velocity;

		public float Dampening = 10.0f;

		[Tooltip("The camera the swipe will be calculated using (None = MainCamera)")]
		public Camera Camera;

		public UnityEvent OnDiscard;

		[Tooltip("If this card is thrown outside of this RectTransform, then it will be destroyed.")]
		public RectTransform Boundary;

		[System.NonSerialized]
		private RectTransform cachedRect;

		protected virtual void Awake()
		{
			cachedRect = GetComponent<RectTransform>();

			if (AutoGetAnchorPoint == true)
			{
				AnchorPoint = cachedRect.anchoredPosition;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			cachedRect = GetComponent<RectTransform>();
		}

		protected virtual void Update()
		{
			if (Velocity != Vector3.zero)
			{
				cachedRect.position += Velocity * Time.deltaTime;

				if (Boundary != null)
				{
					var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera, transform.position);

					if (RectTransformUtility.RectangleContainsScreenPoint(Boundary, screenPoint, Camera) == false)
					{
						Destroy(gameObject);
					}
				}
			}
			else if (Anchor == true && Selectable.IsSelected == false)
			{
				var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

				cachedRect.anchoredPosition = Vector2.Lerp(cachedRect.anchoredPosition, AnchorPoint, factor);
			}
		}

		protected override void OnSelectUp(LeanFinger finger)
		{
			if (Velocity == Vector3.zero)
			{
				var tapThreshold = Mathf.Min(LeanTouch.CurrentTapThreshold, finger.Age);
				var oldPos       = finger.GetSnapshotScreenPosition(finger.Age - tapThreshold);
				var newPos       = finger.ScreenPosition;

				if (Vector2.Distance(oldPos, newPos) * LeanTouch.ScalingFactor >= LeanTouch.CurrentSwipeThreshold)
				{
					// Convert back to world space
					var oldWorldPoint = default(Vector3);

					if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, oldPos, Camera, out oldWorldPoint) == true)
					{
						var newWorldPoint = default(Vector3);

						if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, newPos, Camera, out newWorldPoint) == true)
						{
							var delta = newWorldPoint - oldWorldPoint;

							Velocity = delta / tapThreshold;

							if (OnDiscard != null)
							{
								OnDiscard.Invoke();
							}
						}
					}
				}
			}
		}
	}
}