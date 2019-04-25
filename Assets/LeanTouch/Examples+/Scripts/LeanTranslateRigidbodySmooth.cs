using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanTranslateRigidbody to be smooth
	public class LeanTranslateRigidbodySmooth : LeanTranslateRigidbody
	{
		[Tooltip("How sharp the velocity value changes update (-1 = instant)")]
		public float Dampening = 10.0f;

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			// Make sure the camera exists and the targetScreenPoint is set
			if (cachedCamera != null && targetSet == true)
			{
				// Dampen the velocity
				cachedRigidbody.velocity *= LeanTouch.GetDampenFactor(Dampening, Time.fixedDeltaTime);
			}
		}
	}
}