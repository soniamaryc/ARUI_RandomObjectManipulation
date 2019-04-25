using UnityEngine;

namespace Lean.Touch
{
	// This component will keep this GameObject a certain distance away from the center
	[ExecuteInEditMode]
	public class LeanCameraDolly : MonoBehaviour
	{
		[Tooltip("The direction of the dolly")]
		public Vector3 Direction = -Vector3.forward;

		[Tooltip("The current dolly distance")]
		public float Distance = 10.0f;

		[Tooltip("Should the distance value get clamped?")]
		public bool DistanceClamp;

		[Tooltip("The minimum distance")]
		public float DistanceMin = 1.0f;

		[Tooltip("The maximum distance")]
		public float DistanceMax = 100.0f;

		[Tooltip("The layers the dolly should collide against")]
		public LayerMask CollisionLayers;

		[Tooltip("The radius of the dolly collider")]
		public float CollisionRadius = 0.1f;

		public void MultiplyDistance(float scale)
		{
			Distance *= scale;
		}

		protected virtual void LateUpdate()
		{
			// Limit distance to min/max values?
			if (DistanceClamp == true)
			{
				Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
			}

			// Reset position
			transform.localPosition = Vector3.zero;

			// Collide against stuff?
			if (CollisionLayers != 0)
			{
				var hit            = default(RaycastHit);
				var start          = transform.TransformPoint(Direction.normalized * DistanceMin);
				var direction      = transform.TransformDirection(Direction);
				var distanceSpread = DistanceMax - DistanceMin;

				if (Physics.SphereCast(start, CollisionRadius, direction, out hit, distanceSpread, CollisionLayers) == true)
				{
					var newDistance = DistanceMin + hit.distance;

					// Only update if the distance is closer, else the camera can glue to walls behind it
					if (newDistance < Distance)
					{
						Distance = newDistance;
					}
				}
			}

			// Dolly back by distance
			transform.Translate(Direction.normalized * Distance);
		}
	}
}