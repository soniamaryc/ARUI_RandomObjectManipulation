using UnityEngine;

namespace Lean.Touch
{
	// This component scales the current selectable based on the selecting finger pressure
	public class LeanSelectablePressureScale : LeanSelectableBehaviour
	{
		[Tooltip("The default scale with no pressure")]
		public Vector3 BaseScale = Vector3.one;

		[Tooltip("The amount BaseScale gets multiplied based on the finger pressure")]
		public float PressureMultiplier = 0.25f;

		[Tooltip("Limit pressure to a range of 0..1?")]
		public bool PressureClamp;

		protected virtual void Update()
		{
			// Get pressure
			var pressure = 0.0f;

			if (Selectable.SelectingFinger != null)
			{
				pressure = Selectable.SelectingFinger.Pressure;
			}
			
			// Clamp?
			if (PressureClamp == true)
			{
				pressure = Mathf.Clamp01(pressure);
			}

			transform.localScale = BaseScale + BaseScale * pressure * PressureMultiplier;
		}
	}
}