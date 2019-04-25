using UnityEngine;

namespace Lean.Touch
{
	// This snaps the current GameObject along the specified local axes
	public class LeanSnap : MonoBehaviour
	{
		[Tooltip("Size of each snap interval in local space (0 = none)")]
		public float SnapSizeX = 1.0f;

		[Tooltip("Size of each snap interval in local space (0 = none)")]
		public float SnapSizeY = 1.0f;

		[Tooltip("Size of each snap interval in local space (0 = none)")]
		public float SnapSizeZ = 1.0f;

		protected virtual void Update()
		{
			var position = transform.localPosition;

			if (SnapSizeX != 0.0f)
			{
				position.x = Mathf.Round(position.x / SnapSizeX) * SnapSizeX;
			}

			if (SnapSizeY != 0.0f)
			{
				position.y = Mathf.Round(position.y / SnapSizeY) * SnapSizeY;
			}

			if (SnapSizeZ != 0.0f)
			{
				position.z = Mathf.Round(position.z / SnapSizeZ) * SnapSizeZ;
			}

			transform.localPosition = position;
		}
	}
}