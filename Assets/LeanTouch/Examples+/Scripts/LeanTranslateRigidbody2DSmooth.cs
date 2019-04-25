using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanTranslateRigidbody2D to be smooth
	public class LeanTranslateRigidbody2DSmooth : LeanTranslateRigidbody2D
	{
		[Tooltip("How sharp the position value changes update (-1 = instant)")]
		public float Dampening = -1.0f;

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