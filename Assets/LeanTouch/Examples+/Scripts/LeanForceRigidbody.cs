using UnityEngine;

namespace Lean.Touch
{
	[RequireComponent(typeof(Rigidbody))]
	public class LeanForceRigidbody : MonoBehaviour
	{
		[Tooltip("The camera the force will be calculated using (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("Use mass in velocity calculation?")]
		public bool UseMass;

		[Tooltip("The force multiplier")]
		public float VelocityMultiplier = 1.0f;

		[Tooltip("Rotate if using ApplyBetween?")]
		public bool RotateToVelocity;

		[System.NonSerialized]
		private Rigidbody cachedBody;

		public void Apply(Vector2 screenDelta)
		{
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var oldPoint    = transform.position;
				var screenPoint = camera.WorldToScreenPoint(oldPoint);

				screenPoint.x += screenDelta.x;
				screenPoint.y += screenDelta.y;

				var newPoint = camera.ScreenToWorldPoint(screenPoint);

				ApplyBetween(oldPoint, newPoint);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}

		public void ApplyTo(Vector3 end)
		{
			ApplyBetween(transform.position, end);
		}

		public void ApplyBetween(Vector3 start, Vector3 end)
		{
			var direction = end - start;
			var angle     = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
			var forceMode = UseMass == true ? ForceMode.Impulse : ForceMode.VelocityChange;

			if (RotateToVelocity == true)
			{
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, -angle);
			}

			cachedBody.AddForce(direction * VelocityMultiplier, forceMode);
		}

		protected virtual void OnEnable()
		{
			cachedBody = GetComponent<Rigidbody>();
		}
	}
}