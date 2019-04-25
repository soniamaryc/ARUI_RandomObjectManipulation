using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to translate the current GameObject using events
	public class LeanManualTranslate : MonoBehaviour
	{
		public enum ScaleType
		{
			None,
			DeltaTime
		}

		[Tooltip("The translation distance is multiplied by this")]
		public ScaleType Scale;

		[Tooltip("The translation space")]
		public Space Space = Space.World;

		[Tooltip("The first translation direction")]
		public Vector3 DirectionA = Vector3.right;

		[Tooltip("The second translation direction")]
		public Vector3 DirectionB = Vector3.up;

		[Tooltip("The translation distance is multiplied by this")]
		public float Multiplier = 1.0f;

		[Tooltip("If you want to translate a different GameObject, then specify it here")]
		public GameObject Target;

		public void Translate(float delta)
		{
			Translate(new Vector2(delta, 0.0f));
		}

		public virtual void Translate(Vector2 delta)
		{
			var finalTransform = Target != null ? Target.transform : transform;

			if (Scale == ScaleType.DeltaTime)
			{
				delta *= Time.deltaTime;
			}

			finalTransform.Translate(DirectionA * delta.x * Multiplier, Space);
			finalTransform.Translate(DirectionB * delta.y * Multiplier, Space);
		}
	}
}