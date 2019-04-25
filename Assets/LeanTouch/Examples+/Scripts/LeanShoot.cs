using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to shoot a prefab from the current GameObject
	public class LeanShoot : MonoBehaviour
	{
		[Tooltip("The bullet prefab")]
		public GameObject BulletPrefab;

		[Tooltip("The direction of the bullet shooting relative to this GameObject")]
		public Vector3 Direction = Vector3.forward;

		[Tooltip("The speed of the bullet when shot")]
		public float Speed = 1.0f;

		// You can call this from the UI or another script
		public void Shoot()
		{
			// Does the prefab exist?
			if (BulletPrefab != null)
			{
				var bullet = Instantiate(BulletPrefab);

				bullet.transform.position = transform.position;
				bullet.transform.rotation = transform.rotation;

				var rigidbody3D = bullet.GetComponent<Rigidbody>();

				if (rigidbody3D != null)
				{
					var worldDirection = transform.TransformDirection(Direction);

					// Make sure the direction is valid
					if (worldDirection.sqrMagnitude > 0.0f)
					{
						rigidbody3D.velocity = worldDirection.normalized * Speed;
					}
				}
			}
		}
	}
}