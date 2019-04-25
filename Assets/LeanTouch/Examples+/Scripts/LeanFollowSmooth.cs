using UnityEngine;

namespace Lean.Touch
{
	// This component moves te current one so it follows the target
	public class LeanFollowSmooth : MonoBehaviour
	{
		[Tooltip("The transform that will be followed")]
		public Transform Target;

		[Tooltip("The point that will be followed (set Target for this to be automatically updated)")]
		public Vector3 TargetPosition;

		[Tooltip("Ignore Z for 2D?")]
		public bool IgnoreZ;

		[Tooltip("How sharp the position value changes update (-1 = instant)")]
		public float Dampening = -1.0f;

		public void FollowSelection()
		{
			var center = default(Vector3);
			var count  = 0;

			for (var i = 0; i < LeanSelectable.Instances.Count; i++)
			{
				var selectable = LeanSelectable.Instances[i];

				if (selectable.IsSelected == true)
				{
					center += selectable.transform.position;
					count  += 1;
				}
			}

			if (count > 0)
			{
				TargetPosition = center / count;
			}
		}

		protected virtual void LateUpdate()
		{
			if (Target != null)
			{
				TargetPosition = Target.position;
			}

			var oldPoint = transform.position;
			var newPoint = TargetPosition;
			var point    = default(Vector3);

			if (IgnoreZ == true)
			{
				point.x = newPoint.x;
				point.y = newPoint.y;
				point.z = oldPoint.z;
			}
			else
			{
				point.x = newPoint.x;
				point.y = newPoint.y;
				point.z = newPoint.z;
			}

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Lerp the current values to the target ones
			point = Vector3.Lerp(oldPoint, point, factor);

			// Apply new point
			transform.position = point;
		}
	}
}