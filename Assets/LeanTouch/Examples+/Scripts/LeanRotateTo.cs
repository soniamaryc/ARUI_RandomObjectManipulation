using UnityEngine;

namespace Lean.Touch
{
	public class LeanRotateTo : MonoBehaviour
	{
		[Tooltip("The camera the force will be calculated using (None = MainCamera)")]
		public Camera Camera;

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
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var direction = end - start;
				var angle     = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

				transform.rotation = Quaternion.Euler(0.0f, 0.0f, -angle);
			}
		}
	}
}