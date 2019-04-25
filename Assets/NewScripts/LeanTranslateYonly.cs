using UnityEngine;

namespace Lean.Touch
{
	// This component modifies LeanTranslate to be smooth
	public class LeanTranslateYonly : LeanTranslate
	{
		[Tooltip("How smoothly this object moves to its target position")]
		public float Dampening = 10.0f;

		[System.NonSerialized]
		private Vector3 remainingTranslation;

		protected override void Update()
		{
			// Store
			var oldPosition = transform.localPosition;

			// Update
			base.Update();

			// Increment
			remainingTranslation += transform.localPosition - oldPosition;

			// Revert
			transform.localPosition = oldPosition;
		}

		protected virtual void LateUpdate()
		{
			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Dampen remainingDelta
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);
			Vector3 moveMe = (remainingTranslation - newRemainingTranslation);
			//-------------  X-Z axis ----------------------------------------------------------//
			//moveMe.y = 0f;
			//-----------Y only-----------------//
			moveMe.z = 0f;
			moveMe.x = 0f;
			//----------------------------//
			//Debug.Log("move me : " + moveMe);
			//transform.position += (RemainingDelta - newDelta);
			transform.position += moveMe;

			// Shift this transform by the change in delta
			//transform.localPosition += remainingTranslation - newRemainingTranslation;

			// Update remainingDelta with the dampened value
			remainingTranslation = newRemainingTranslation;
		}
	}
}