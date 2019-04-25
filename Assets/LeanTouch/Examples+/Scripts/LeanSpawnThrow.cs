using UnityEngine;

namespace Lean.Touch
{
	// This script can be used to spawn a GameObject via an event
	public class LeanSpawnThrow : MonoBehaviour
	{
		[Tooltip("The prefab that gets spawned")]
		public Transform Prefab;

		[Tooltip("The distance from the finger the prefab will be spawned in world space")]
		public float Distance = 10.0f;

		[Tooltip("The strength of the throw relative to the drag length")]
		public float ForceMultiplier = 1.0f;

		[Tooltip("Limit the length (0 = none)")]
		public float LengthLimit;

		[Tooltip("Should the force originate from a fixed point?")]
		public Transform Target;

		public void Spawn(LeanFinger finger)
		{
			if (Prefab != null)
			{
				// Start and end points of the drag
				var start    = finger.GetStartWorldPosition(Distance);
				var end      = finger.GetWorldPosition(Distance);
				var distance = Vector3.Distance(start, end);

				// Shift the start and end points to emit from the target?
				if (Target != null)
				{
					end   = Target.position + (end - start);
					start = Target.position;
				}

				// Limit the length?
				if (LengthLimit > 0.0f && distance > LengthLimit)
				{
					var direction = Vector3.Normalize(end - start);

					distance = LengthLimit;
					end      = start + direction * distance;
				}

				// Vector between points
				var vector = end - start;

				// Angle between points
				var angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

				// Instance the prefab, position it at the start point, and rotate it to the vector
				var instance = Instantiate(Prefab);

				instance.position = start;
				instance.rotation = Quaternion.Euler(0.0f, 0.0f, -angle);

				// Apply 3D force?
				var rigidbody3D = instance.GetComponent<Rigidbody>();

				if (rigidbody3D != null)
				{
					rigidbody3D.velocity = vector * ForceMultiplier;
				}

				// Apply 2D force?
				var rigidbody2D = instance.GetComponent<Rigidbody2D>();

				if (rigidbody2D != null)
				{
					rigidbody2D.velocity = vector * ForceMultiplier;
				}
			}
		}
	}
}