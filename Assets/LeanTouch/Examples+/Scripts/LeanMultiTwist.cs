using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This component allows you to get the twist of all fingers
	public class LeanMultiTwist : MonoBehaviour
	{
		public enum AngleType
		{
			Degrees,
			Radians
		}

		// Event signature
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Ignore if there was no change?")]
		public bool IgnoreIfStatic;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("If RequiredSelectable.IsSelected is false, ignore?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("Should the OnTwist angle be in degrees or radians?")]
		public AngleType Angle;

		public FloatEvent OnTwist;

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

			if (fingers.Count > 0)
			{
				// Get twist
				var twist = Angle == AngleType.Degrees ? LeanGesture.GetTwistDegrees(fingers) : LeanGesture.GetTwistRadians(fingers);

				// Ignore?
				if (IgnoreIfStatic == true && twist == 0.0f)
				{
					return;
				}

				// Call events
				if (OnTwist != null)
				{
					OnTwist.Invoke(twist);
				}
			}
		}
	}
}