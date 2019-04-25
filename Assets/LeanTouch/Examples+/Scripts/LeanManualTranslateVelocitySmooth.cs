using UnityEngine;

namespace Lean.Touch
{
	// This implements LeanManualTranslateSmooth using rigidbody.velocity
	public class LeanManualTranslateVelocitySmooth : MonoBehaviour
	{
		[Tooltip("The translation space")]
		public Space Space = Space.World;

		[Tooltip("The first translation direction")]
		public Vector3 DirectionA = Vector3.right;

		[Tooltip("The second translation direction")]
		public Vector3 DirectionB = Vector3.forward;

		[Tooltip("The translation distance is multiplied by this")]
		public float Multiplier = 1.0f;

		[Tooltip("If your Rigidbody is on a different GameObject, set it here")]
		public GameObject Target;

		[Tooltip("How quickly the position goes to the target value")]
		public float Dampening = 10.0f;

		[Tooltip("If you want this component to override velocity enable this, otherwise disable this and rely on Rigidbody.drag")]
		public bool ResetVelocityInUpdate = true;

		[System.NonSerialized]
		private Vector3 remainingDelta;

		public void AddForce(float delta)
		{
			remainingDelta += DirectionA * delta * Multiplier;
		}

		public void AddForce(Vector2 delta)
		{
			remainingDelta += (DirectionA * delta.x + DirectionB * delta.y) * Multiplier;
		}

		protected virtual void FixedUpdate()
		{
			var finalGameObject = Target != null ? Target : gameObject;
			var factor          = LeanTouch.GetDampenFactor(Dampening, Time.fixedDeltaTime);
			var newDelta        = Vector3.Lerp(remainingDelta, Vector3.zero, factor);
			var rigidbody       = finalGameObject.GetComponent<Rigidbody>();

			if (rigidbody != null)
			{
				var velocity = remainingDelta - newDelta;

				if (Space == Space.Self)
				{
					velocity = rigidbody.transform.rotation * velocity;
				}

				rigidbody.velocity += velocity;
			}

			remainingDelta = newDelta;
		}

		protected virtual void Update()
		{
			if (ResetVelocityInUpdate == true)
			{
				var finalGameObject = Target != null ? Target : gameObject;
				var rigidbody       = finalGameObject.GetComponent<Rigidbody>();

				if (rigidbody != null)
				{
					rigidbody.velocity = Vector3.zero;
				}
			}
		}
	}
}