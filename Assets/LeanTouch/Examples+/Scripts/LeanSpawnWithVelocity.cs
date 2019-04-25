using UnityEngine;

namespace Lean.Touch
{
	// This script can be used to spawn a GameObject via an event
	public class LeanSpawnWithVelocity : MonoBehaviour
	{
		[Tooltip("The prefab that gets spawned")]
		public Transform Prefab;

		[Tooltip("The camera that the prefabs will spawn in front of (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("If you're using NoRelease swiping, then enable this")]
		public bool UseSnapshotVelocity;

		[Tooltip("If spawning with velocity, rotate to it?")]
		public bool RotateToVelocity;

		[Tooltip("If spawning with velocity, scale it?")]
		public float VelocityMultiplier = 1.0f;

		public void SpawnWithVelocity(LeanFinger finger)
		{/*
			if (Prefab != null && finger != null)
			{
				// Get screen positions
				var screenPositionA = finger.StartScreenPosition;
				var screenPositionB = finger.ScreenPosition;

				if (UseSnapshotVelocity == true)
				{
					// The amount of seconds we consider valid for a swipe
					var tapThreshold = LeanTouch.CurrentTapThreshold;

					screenPositionA = finger.GetSnapshotScreenPosition(finger.Age - tapThreshold);
				}

				// Get world positions and delta
				var worldPointA = ScreenDepth.Convert(screenPositionA, Camera, gameObject);
				var worldPointB = ScreenDepth.Convert(screenPositionB, Camera, gameObject);
				var worldDelta  = worldPointB - worldPointA;

				// Spawn and set transform
				var instance = Instantiate(Prefab);

				instance.position = worldPointA;
				instance.rotation = transform.rotation;

				if (RotateToVelocity == true)
				{
					// Angle between points
					var angle = Mathf.Atan2(worldDelta.x, worldDelta.y) * Mathf.Rad2Deg;

					instance.rotation = Quaternion.Euler(0.0f, 0.0f, -angle);
				}

				// Apply 3D force?
				var rigidbody3D = instance.GetComponent<Rigidbody>();

				if (rigidbody3D != null)
				{
					rigidbody3D.velocity = worldDelta * VelocityMultiplier;
				}

				// Apply 2D force?
				var rigidbody2D = instance.GetComponent<Rigidbody2D>();

				if (rigidbody2D != null)
				{
					rigidbody2D.velocity = worldDelta * VelocityMultiplier;
				}
			}*/
		}
	}
}