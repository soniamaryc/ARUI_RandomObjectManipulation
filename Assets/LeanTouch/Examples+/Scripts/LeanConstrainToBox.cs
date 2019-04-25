using UnityEngine;

namespace Lean.Touch
{
	// This script will constrain the current transform.position to a BoxCollider
	public class LeanConstrainToBox : MonoBehaviour
	{
		[Tooltip("The box collider this transform will be constrained to")]
		public BoxCollider Target;
		
		protected virtual void LateUpdate()
		{
			if (Target != null)
			{
				var local = Target.transform.InverseTransformPoint(transform.position);
				var min   = Target.center - Target.size * 0.5f;
				var max   = Target.center + Target.size * 0.5f;
				var set   = false;

				if (local.x < min.x) { local.x = min.x; set = true; }
				if (local.y < min.y) { local.y = min.y; set = true; }
				if (local.z < min.z) { local.z = min.z; set = true; }
				if (local.x > max.x) { local.x = max.x; set = true; }
				if (local.y > max.y) { local.y = max.y; set = true; }
				if (local.z > max.z) { local.z = max.z; set = true; }

				if (set == true)
				{
					transform.position = Target.transform.TransformPoint(local);
				}
			}
		}
	}
}