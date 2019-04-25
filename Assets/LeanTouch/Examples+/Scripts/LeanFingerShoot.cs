#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
	#define UNITY_OLD_LINE_RENDERER
#endif
using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This script shows you how you can check tos ee which part of the screen a finger is on, and work accordingly
	public class LeanFingerShoot : MonoBehaviour
	{
		// This class will store an association between a Finger and a LineRenderer instance
		[System.Serializable]
		public class Link
		{
			public LeanFinger   Finger;
			public LineRenderer Line;
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("The line prefab")]
		public LineRenderer LinePrefab;

		[Tooltip("The distance from the camera the line points will be spawned in world space")]
		public float Distance = 1.0f;

		[Tooltip("The prefab you want to throw")]
		public GameObject ShootPrefab;

		[Tooltip("The strength of the throw")]
		public float Force = 1.0f;

		public float Thickness = 1.0f;

		public float Length = 1.0f;

		[Tooltip("The maximum amount of fingers used")]
		public int MaxLines;

		[Tooltip("The camera the translation will be calculated using (default = MainCamera)")]
		public Camera Camera;

		private List<Link> links = new List<Link>();

		protected virtual void OnEnable()
		{
			// Hook events
			LeanTouch.OnFingerDown += FingerDown;
			LeanTouch.OnFingerSet  += FingerSet;
			LeanTouch.OnFingerUp   += FingerUp;
		}

		protected virtual void OnDisable()
		{
			// Unhook events
			LeanTouch.OnFingerDown -= FingerDown;
			LeanTouch.OnFingerSet  -= FingerSet;
			LeanTouch.OnFingerUp   -= FingerUp;
		}

		// Override the WritePositions method from LeanDragLine
		protected virtual void WritePositions(LineRenderer line, LeanFinger finger)
		{
			// Get start and current world position of finger
			var start = finger.GetStartWorldPosition(Distance);
			var end   = finger.GetWorldPosition(Distance);

			// Limit the length?
			if (start != end)
			{
				end = start + Vector3.Normalize(end - start) * Length;
			}

			// Write positions
#if UNITY_OLD_LINE_RENDERER
			line.SetVertexCount(2);

			line.SetWidth(Thickness, Thickness);
#else
			line.positionCount = 2;

			line.startWidth = Thickness;
			line.endWidth   = Thickness;
#endif
			line.SetPosition(0, start);
			line.SetPosition(1, end);
		}

		private void FingerDown(LeanFinger finger)
		{
			// Ignore?
			if (MaxLines > 0 && links.Count >= MaxLines)
			{
				return;
			}

			if (IgnoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (IgnoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			// Make new link
			var link = new Link();

			// Assign this finger to this link
			link.Finger = finger;

			// Create LineRenderer instance for this link
			link.Line = Instantiate(LinePrefab);

			// Add new link to list
			links.Add(link);
		}

		private void FingerSet(LeanFinger finger)
		{
			// Try and find the link for this finger
			var link = FindLink(finger);

			// Link exists?
			if (link != null && link.Line != null)
			{
				WritePositions(link.Line, link.Finger);
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			// Try and find the link for this finger
			var link = FindLink(finger);

			// Link exists?
			if (link != null)
			{
				// Remove link from list
				links.Remove(link);

				// Destroy line GameObject
				if (link.Line != null)
				{
					Destroy(link.Line.gameObject);
				}

				Shoot(finger);
			}
		}

		private Link FindLink(LeanFinger finger)
		{
			for (var i = 0; i < links.Count; i++)
			{
				var link = links[i];

				if (link.Finger == finger)
				{
					return link;
				}
			}

			return null;
		}

		private void Shoot(LeanFinger finger)
		{
			// Start and end points of the drag
			var start = finger.GetStartWorldPosition(Distance);
			var end   = finger.GetWorldPosition(Distance);

			if (start != end)
			{
				// Vector between points
				var direction = Vector3.Normalize(end - start);

				// Angle between points
				var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

				// Instance the prefab, position it at the start point, and rotate it to the vector
				var instance = Instantiate(ShootPrefab);

				instance.transform.position = start;
				instance.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -angle);

				// Apply 3D force?
				var rigidbody3D = instance.GetComponent<Rigidbody>();

				if (rigidbody3D != null)
				{
					rigidbody3D.velocity = direction * Force;
				}

				// Apply 2D force?
				var rigidbody2D = instance.GetComponent<Rigidbody2D>();

				if (rigidbody2D != null)
				{
					rigidbody2D.velocity = direction * Force;
				}
			}
		}
	}
}