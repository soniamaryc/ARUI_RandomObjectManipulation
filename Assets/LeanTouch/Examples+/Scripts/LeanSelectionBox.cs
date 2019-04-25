using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This component will draw a selection box
	public class LeanSelectionBox : MonoBehaviour
	{
		// This class will store an association between a Finger and a RectTransform instance
		[System.Serializable]
		public class Link
		{
			public LeanFinger    Finger; // The finger associated with this link
			public RectTransform Box; // The RectTransform instance associated with this link
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreIfStartedOverGui = true;

		[Tooltip("The selection box prefab")]
		public RectTransform Prefab;

		[Tooltip("The transform the prefabs will be spawned on (NOTE: This RectTransform must fill the whole screen, like the main canvas)")]
		public RectTransform Root;

		[Tooltip("The camera used to calculate box coordinates (None = MainCamera)")]
		public Camera Camera;

		// This stores all the links between Fingers and RectTransform instances
		private List<Link> links = new List<Link>();

		// Temporary selectables inside box
		private static List<LeanSelectable> selectables = new List<LeanSelectable>();

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

		private void FingerDown(LeanFinger finger)
		{
			// Limit to one selection box
			if (links.Count > 0)
			{
				return;
			}

			// Only use fingers clear of the GUI
			if (IgnoreIfStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			// Make new link
			var link = new Link();

			// Assign this finger to this link
			link.Finger = finger;

			// Create LineRenderer instance for this link
			link.Box = Instantiate(Prefab);

			// Move box to root
			link.Box.transform.SetParent(Root, false);

			// Add new link to list
			links.Add(link);
		}

		private void FingerSet(LeanFinger finger)
		{
			// Try and find the link for this finger
			var link = FindLink(finger);

			// Link exists?
			if (link != null && link.Box != null)
			{
				WriteTransform(link.Box, link.Finger);
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

				// Destroy box GameObject
				if (link.Box != null)
				{
					Destroy(link.Box.gameObject);
				}
			}
		}

		// Searches through all links for the one associated with the input finger
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

		// Transform rect based on finger data
		private void WriteTransform(RectTransform rect, LeanFinger finger)
		{
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var min = camera.ScreenToViewportPoint(finger.StartScreenPosition);
				var max = camera.ScreenToViewportPoint(finger.     ScreenPosition);
				
				// Fix any inverted values
				if (min.x > max.x)
				{
					var t = min.x;

					min.x = max.x;
					max.x = t;
				}

				if (min.y > max.y)
				{
					var t = min.y;

					min.y = max.y;
					max.y = t;
				}

				// Reset pivot in case you forgot
				rect.pivot = Vector2.zero;

				// Set anchors
				rect.anchorMin = min;
				rect.anchorMax = max;

				// Make rect we check against
				var viewportRect = new Rect();

				viewportRect.min = min;
				viewportRect.max = max;

				// Rebuild list of all selectables within rect
				selectables.Clear();

				for (var i = LeanSelectable.Instances.Count - 1; i >= 0; i--)
				{
					var selectable    = LeanSelectable.Instances[i];
					var viewportPoint = camera.WorldToViewportPoint(selectable.transform.position);

					if (viewportRect.Contains(viewportPoint) == true)
					{
						selectables.Add(selectable);
					}
				}

				// Select them
				LeanSelectable.ReplaceSelection(finger, selectables);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}