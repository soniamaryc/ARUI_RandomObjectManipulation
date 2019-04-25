using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to drag this rigidbody
	[RequireComponent(typeof(Rigidbody))]
	public class LeanTranslateRigidbody : MonoBehaviour
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

		protected Rigidbody cachedRigidbody;

		protected Camera cachedCamera;

		protected bool targetSet;

		protected Vector3 targetScreenPoint;

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
			// Make sure the camera exists and the targetScreenPoint is set
			if (cachedCamera != null && targetSet == true)
			{
				// Calculate required velocity to arrive in one FixedUpdate
				var oldPosition = transform.position;
				var newPosition = cachedCamera.ScreenToWorldPoint(targetScreenPoint);
				var velocity    = (newPosition - oldPosition) / Time.fixedDeltaTime;

				// Apply the velocity
				cachedRigidbody.velocity = velocity;
			}
		}

		protected virtual void Update()
		{
			// Make sure the camera exists
			cachedCamera = LeanTouch.GetCamera(Camera, gameObject);

			if (cachedCamera != null)
			{
				// Get the fingers we want to use
				var fingers = LeanSelectable.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount, RequiredSelectable);

				if (fingers.Count > 0)
				{
					// If it's the first frame the fingers are down, grab the current screen point of this GameObject
					if (targetSet == false)
					{
						targetSet         = true;
						targetScreenPoint = cachedCamera.WorldToScreenPoint(transform.position);
					}

					// Shift target point based on finger deltas
					// NOTE: targetScreenPoint.z already stores the depth
					targetScreenPoint += (Vector3)LeanGesture.GetScreenDelta(fingers);
				}
				// Unset if no fingers are down
				else
				{
					targetSet = false;
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}