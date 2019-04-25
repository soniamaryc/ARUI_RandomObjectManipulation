using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to add force to the current GameObject using events
	public class LeanManualVelocity : MonoBehaviour
	{
		[Tooltip("The velocity space")]
		public Space Space = Space.World;

		[Tooltip("The first force direction")]
		public Vector3 Direction = Vector3.forward;

		[Tooltip("Fixed multiplier for the force")]
		public float Multiplier = 1.0f;

		[Tooltip("If your Rigidbody is on a different GameObject, set it here")]
		public GameObject Target;

		public virtual void AddForce(float delta)
		{
			var finalGameObject = Target != null ? Target : gameObject;
			var rigidbody       = finalGameObject.GetComponent<Rigidbody>();

			if (rigidbody != null)
			{
				var force = Direction * delta * Multiplier;

				if (Space == Space.Self)
				{
					force = rigidbody.transform.rotation * force;
				}

				rigidbody.velocity += force;
			}
		}
	}
}