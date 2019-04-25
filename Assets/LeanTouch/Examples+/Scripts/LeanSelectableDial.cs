using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to twist the selected object around like a dial or knob
	[ExecuteInEditMode]
	public class LeanSelectableDial : LeanSelectableBehaviour
	{
		[Tooltip("The camera we will be used (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("The angle of the dial in degrees.")]
		public float Angle;

		[Tooltip("The axis of the rotation in local space.")]
		public Vector3 EulerAngles;

		private Vector3 oldPoint;

		private bool oldPointSet;

		protected virtual void Update()
		{
			// Reset rotation and get axis
			transform.localEulerAngles = EulerAngles;

			var axis = transform.up;

			// Is this GameObject selected?
			if (Selectable.IsSelected == true)
			{
				// Does it have a selected finger?
				var finger = Selectable.SelectingFinger;

				if (finger != null)
				{
					var newPoint  = GetPoint(axis, finger.ScreenPosition);

					if (oldPointSet == true)
					{
						var oldVector = oldPoint - transform.position;
						var newVector = newPoint - transform.position;
						var cross     = Vector3.Cross(oldVector, newVector);
						var delta     = Vector3.Angle(oldVector, newVector);

						if (Vector3.Dot(cross, axis) >= 0.0f)
						{
							Angle += delta;
						}
						else
						{
							Angle -= delta;
						}
					}

					oldPoint    = newPoint;
					oldPointSet = true;
				}
			}
			else
			{
				oldPointSet = false;
			}

			transform.Rotate(axis, Angle, Space.World);
		}

		private Vector3 GetPoint(Vector3 axis, Vector2 screenPoint)
		{
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var ray      = camera.ScreenPointToRay(screenPoint);
				var plane    = new Plane(axis, transform.position);
				var distance = default(float);

				if (plane.Raycast(ray, out distance) == true)
				{
					return ray.GetPoint(distance);
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}

			return oldPoint;
		}
	}
}