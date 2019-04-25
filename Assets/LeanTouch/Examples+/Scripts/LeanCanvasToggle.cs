using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to toggle between two RectTransform states
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class LeanCanvasToggle : MonoBehaviour
	{
		[Tooltip("If you enable this, the toggled values will be used, otherwise the default ones will be.")]
		public bool toggled;

		[Tooltip("How fast the value transition is.")]
		public float Dampening = 10.0f;

		[Tooltip("Enable this if you want to control the RectTransform.anchoredPosition value.")]
		public bool AnchoredPosition;

		[Tooltip("The RectTransform.anchoredPosition we will transition to when Toggled = false.")]
		public Vector2 AnchoredPositionDefault;

		[Tooltip("The RectTransform.anchoredPosition we will transition to when Toggled = true.")]
		public Vector2 AnchoredPositionToggled;

		[Tooltip("Enable this if you want to control the RectTransform.pivot value.")]
		public bool Pivot;

		[Tooltip("The RectTransform.pivot we will transition to when Toggled = false.")]
		public Vector2 PivotDefault;

		[Tooltip("The RectTransform.pivot we will transition to when Toggled = true.")]
		public Vector2 PivotToggled;

		[System.NonSerialized]
		private RectTransform cachedRectTransform;

		public bool Toggled
		{
			set
			{
				toggled = value;
			}

			get
			{
				return toggled;
			}
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			cachedRectTransform = GetComponent<RectTransform>();

			AnchoredPositionDefault = AnchoredPositionToggled = cachedRectTransform.anchoredPosition;

			PivotDefault = PivotToggled = cachedRectTransform.pivot;
		}
#endif

		protected virtual void OnEnable()
		{
			cachedRectTransform = GetComponent<RectTransform>();
		}

		protected virtual void Update()
		{
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			if (AnchoredPosition == true)
			{
				var target = Toggled == true ? AnchoredPositionToggled : AnchoredPositionDefault;

				cachedRectTransform.anchoredPosition = Vector2.Lerp(cachedRectTransform.anchoredPosition, target, factor);
			}

			if (Pivot == true)
			{
				var target = Toggled == true ? PivotToggled : PivotDefault;

				cachedRectTransform.pivot = Vector2.Lerp(cachedRectTransform.pivot, target, factor);
			}
		}
	}
}