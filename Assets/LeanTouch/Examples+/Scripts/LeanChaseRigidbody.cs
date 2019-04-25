using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to drag this rigidbody
	[RequireComponent(typeof(Rigidbody))]
	public class LeanChaseRigidbody : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does translation require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The camera we will be used (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("How sharp the position value changes update (-1 = instant)")]
		public float Dampening = -1.0f;

		/*
		public bool Rotation;

		[Tooltip("How sharp the position value changes update (-1 = instant)")]
		public float RotationDampening = -1.0f;
		*/

		[Tooltip("The conversion method used to find a world point from a screen point")]
		public LeanScreenDepth ScreenDepth;

		private Rigidbody cachedRigidbody;

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

		protected virtual void OnEnable()
		{
			cachedRigidbody = GetComponent<Rigidbody>();
		}

		protected virtual void FixedUpdate()
		{
			// Get the fingers we want to use
			var fingers = LeanSelectable.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount, RequiredSelectable);

			if (fingers.Count > 0)
			{
				var oldPosition = transform.position;
				var targetPoint = LeanGesture.GetScreenCenter(fingers);
				var newPosition = ScreenDepth.Convert(targetPoint, Camera, gameObject);
				var direction   = newPosition - oldPosition;
				var velocity    = direction / Time.fixedDeltaTime;

				// Apply the velocity
				velocity *= LeanTouch.GetDampenFactor(Dampening, Time.fixedDeltaTime);

				cachedRigidbody.velocity = velocity;

				/*
				if (Rotation == true && direction != Vector3.zero)
				{
					var angle           = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
					var directionB      = (Vector2)transform.up;
					var angleB          = Mathf.Atan2(directionB.x, directionB.y) * Mathf.Rad2Deg;
					var delta           = Mathf.DeltaAngle(angle, angleB);
					var angularVelocity = delta / Time.fixedDeltaTime;

					angularVelocity *= LeanTouch.GetDampenFactor(RotationDampening, Time.fixedDeltaTime);

					//cachedRigidbody.angularVelocity = angularVelocity;
				}
				*/
			}
		}
	}
}