using UnityEngine;

namespace Lean.Touch
{
	// This component will enable/disable the target pinch and twist components based on total pinch and twist gestures, like mobile map applications
	public class LeanGestureToggle : MonoBehaviour
	{
		public enum StateType
		{
			None,
			Drag,
			Pinch,
			Twist
		}

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

		[Tooltip("If one specific gesture hasn't been isolated yet, keep them all enabled?")]
		public bool EnableWithoutIsolation;

		[Space(10.0f)]
		[Tooltip("The component that will be enabled/disabled when dragging")]
		public MonoBehaviour DragComponent;

		[Tooltip("The amount of drag required to enable dragging mode")]
		public float DragThreshold = 50.0f;

		[Space(10.0f)]
		[Tooltip("The component that will be enabled/disabled when pinching")]
		public MonoBehaviour PinchComponent;

		[Tooltip("The amount of pinch required to enable twisting in scale (e.g. 0.1 = 0.9 to 1.1).")]
		public float PinchThreshold = 0.1f;

		[Space(10.0f)]
		[Tooltip("The component that will be enabled/disabled when twisting")]
		public MonoBehaviour TwistComponent;

		[Tooltip("The amount of twist required to enable twisting in degrees.")]
		public float TwistThreshold = 5.0f;

		[Tooltip("Enable twist component when pinch component is activated?")]
		public bool TwistWithPinch;

		[System.NonSerialized]
		private StateType state;

		[System.NonSerialized]
		private Vector2 delta;

		[System.NonSerialized]
		private float scale = 1.0f;

		[System.NonSerialized]
		private float twist;

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
				delta += LeanGesture.GetScaledDelta(fingers);
				scale *= LeanGesture.GetPinchRatio(fingers);
				twist += LeanGesture.GetTwistDegrees(fingers);

				if (state == StateType.None)
				{
					if (DragComponent != null && delta.magnitude >= DragThreshold)
					{
						state = StateType.Drag;
					}
					else if (PinchComponent != null && Mathf.Abs(scale - 1.0f) >= PinchThreshold)
					{
						state = StateType.Pinch;
					}
					else if (TwistComponent != null && Mathf.Abs(twist) >= TwistThreshold)
					{
						state = StateType.Twist;
					}
				}
			}
			else
			{
				state = StateType.None;
				delta = Vector2.zero;
				scale = 1.0f;
				twist = 0.0f;
			}

			if (DragComponent != null)
			{
				DragComponent.enabled = state == StateType.Drag || (EnableWithoutIsolation == true && state == StateType.None);
			}

			if (PinchComponent != null)
			{
				PinchComponent.enabled = state == StateType.Pinch || (EnableWithoutIsolation == true && state == StateType.None);
			}

			if (TwistComponent != null)
			{
				TwistComponent.enabled = state == StateType.Twist || (EnableWithoutIsolation == true && state == StateType.None) || (TwistWithPinch == true && state == StateType.Pinch);
			}
		}
	}
}