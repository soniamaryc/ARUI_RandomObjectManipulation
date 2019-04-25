using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to add force to the current GameObject using events
	public class LeanManualVelocity2D : MonoBehaviour
	{
		[Tooltip("The velocity space")]
		public Space Space = Space.World;

		[Tooltip("The first force direction")]
		public Vector3 DirectionA = Vector3.right;

		[Tooltip("The second force direction")]
		public Vector3 DirectionB = Vector3.forward;

		[Tooltip("The translation distance is multiplied by this")]
		public float Multiplier = 1.0f;

		[Tooltip("If your Rigidbody is on a different GameObject, set it here")]
		public GameObject Target;

		public virtual void AddForce(Vector2 delta)
		{
			var finalGameObject = Target != null ? Target : gameObject;
			var rigidbody       = finalGameObject.GetComponent<Rigidbody>();

			if (rigidbody != null)
			{
				var force = (DirectionA * delta.x + DirectionB * delta.y) * Multiplier;

				if (Space == Space.Self)
				{
					force = rigidbody.transform.rotation * force;
				}

				rigidbody.velocity += force;
			}
		}
	}
}