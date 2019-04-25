using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to spawn a prefab between two points
	public class LeanSpawnBetween : MonoBehaviour
	{
		[Tooltip("The prefab that gets spawned")]
		public Transform Prefab;

		[Tooltip("The camera that the prefabs will spawn in front of (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("The force multiplier of SpawnWithVelocity")]
		public float VelocityMultiplier = 1.0f;

		public void Spawn(Vector3 start, Vector3 end)
		{
			if (Prefab != null)
			{
				// Vector between points
				var direction = end - start;

				// Angle between points
				var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

				// Spawn and set transform
				var instance = Instantiate(Prefab);

				instance.position = start;
				instance.rotation = Quaternion.Euler(0.0f, 0.0f, -angle);

				// Apply 3D force?
				var rigidbody3D = instance.GetComponent<Rigidbody>();

				if (rigidbody3D != null)
				{
					rigidbody3D.velocity = direction * VelocityMultiplier;
				}

				// Apply 2D force?
				var rigidbody2D = instance.GetComponent<Rigidbody2D>();

				if (rigidbody2D != null)
				{
					rigidbody2D.velocity = direction * VelocityMultiplier;
				}
			}
		}
	}
}