using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This component allows you to get the pinch of all fingers
	public class LeanMultiPinch : MonoBehaviour
	{
		public enum ScaleType
		{
			PinchScale, // Pinch Scale     (1 = no change)
			PinchRatio, // 1 / Pinch Scale (1 = no change)
			PinchShift // Pinch Scale - 1  (0 = no change)
		}

		// Event signature
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("If the finger didn't move, ignore it?")]
		public bool IgnoreIfStatic;

		[Tooltip("If RequiredSelectable.IsSelected is false, ignore?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("PinchScale = Normal pinch (e.g. 1 = no change, 2 = double size, 0.5 = half size)\nPinchRatio = 1 / PinchScale (inverted)\nPinchShift = PinchScale - 1 (useful for LeanManualTranslate, etc)")]
		public ScaleType Scale;

		[Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
		[Range(-1.0f, 1.0f)]
		public float WheelSensitivity;

		public FloatEvent OnPinch;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Start();
		}
#endif

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponent<LeanSelectable>();
			}
		}

		protected virtual void Update()
		{
			// Get fingers
			var fingers = LeanSelectable.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount, RequiredSelectable);

			// Get pinch
			var pinch = Scale == ScaleType.PinchRatio == true ? LeanGesture.GetPinchRatio(fingers, WheelSensitivity) : LeanGesture.GetPinchScale(fingers, WheelSensitivity);

			// Ignore?
			if (pinch == 1.0f)
			{
				return;
			}

			// This gives you a 0 based pinch value, allowing usage with translation and rotation
			if (Scale == ScaleType.PinchShift)
			{
				pinch -= 1.0f;
			}

			// Call events
			if (OnPinch != null)
			{
				OnPinch.Invoke(pinch);
			}
		}
	}
}