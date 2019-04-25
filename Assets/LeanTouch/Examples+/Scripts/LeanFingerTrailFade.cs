#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
	#define UNITY_OLD_LINE_RENDERER
#endif
using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This script will draw the path each finger has taken since it started being pressed
	public class LeanFingerTrailFade : MonoBehaviour
	{
		// This class will store an association between a finger and a LineRenderer instance
		[System.Serializable]
		public class Link
		{
			public LeanFinger   Finger; // The finger associated with this link
			public LineRenderer Line; // The LineRenderer instance associated with this link
			public float        Life; // The amount of seconds until this link disappears
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreIfStartedOverGui = true;

		[Tooltip("Must RequiredSelectable.IsSelected be true?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The line prefab")]
		public LineRenderer LinePrefab;

		[Tooltip("The conversion method used to find a world point from a screen point")]
		public LeanScreenDepth ScreenDepth;

		[Tooltip("How many seconds it takes for each line to disappear after a finger is released")]
		public float FadeTime = 1.0f;

		[Tooltip("The maximum amount of fingers used")]
		public int MaxLines;

		public Color StartColor = Color.white;

		public Color EndColor = Color.white;

		[Tooltip("The camera the translation will be calculated using (default = MainCamera)")]
		public Camera Camera;

		// This stores all the links between fingers and LineRenderer instances
		private List<Link> links = new List<Link>();

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Start();
		}
#endif

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponent<LeanSelectable>();
			}
		}
	
		protected virtual void OnEnable()
		{
			// Hook events
			LeanTouch.OnFingerSet += FingerSet;
			LeanTouch.OnFingerUp  += FingerUp;
		}

		protected virtual void OnDisable()
		{
			// Unhook events
			LeanTouch.OnFingerSet += FingerSet;
			LeanTouch.OnFingerUp  += FingerUp;
		}

		protected virtual void Update()
		{
			// Loop through all links
			for (var i = 0; i < links.Count; i++)
			{
				var link = links[i];

				// Has this link's finger been unlinked? (via OnFingerUp)
				if (link.Finger == null)
				{
					// Remove life from the link
					link.Life -= Time.deltaTime;

					// Is the link still alive?
					if (link.Life > 0.0f)
					{
						// Make sure FadeTime is set to prevent divide by 0
						if (FadeTime > 0.0f)
						{
							// Find the life to FadeTime 0..1 ratio
							var ratio = link.Life / FadeTime;

							// Copy the start & end colors and fade them by the ratio
							var color0 = StartColor;
							var color1 =   EndColor;
							
							color0.a *= ratio;
							color1.a *= ratio;

							// Write the new colors
#if UNITY_OLD_LINE_RENDERER
							link.Line.SetColors(color0, color1);
#else
							link.Line.startColor = color0;
							link.Line.endColor   = color1;
#endif
						}
					}
					// Kill the link?
					else
					{
						// Remove link from list
						links.Remove(link);

						// Destroy line GameObject
						Destroy(link.Line.gameObject);
					}
				}
			}
		}

		// Override the WritePositions method from LeanDragLine
		protected virtual void WritePositions(LineRenderer line, LeanFinger finger)
		{
			// Reserve one vertex for each snapshot
#if UNITY_OLD_LINE_RENDERER
			line.SetVertexCount(finger.Snapshots.Count);
#else
			line.positionCount = finger.Snapshots.Count;
#endif
			
			// Loop through all snapshots
			for (var i = 0; i < finger.Snapshots.Count; i++)
			{
				var snapshot = finger.Snapshots[i];
				
				// Get the world postion of this snapshot
				var worldPoint = ScreenDepth.Convert(snapshot.ScreenPosition, Camera, gameObject);

				// Write position
				line.SetPosition(i, worldPoint);
			}
		}

		private void FingerSet(LeanFinger finger)
		{
			if (MaxLines > 0 && links.Count >= MaxLines)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			// Get link for this finger and write positions
			var link = FindLink(finger, true);

			if (link != null && link.Line != null)
			{
				WritePositions(link.Line, link.Finger);
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			// If link with this finger exists, null finger and assign life, so it can be destroyed later
			var link = FindLink(finger, false);

			if (link != null)
			{
				// Call up method
				LinkFingerUp(link);

				link.Finger = null;
				link.Life   = FadeTime;
			}
		}

		protected virtual void LinkFingerUp(Link link)
		{
		}

		private Link FindLink(LeanFinger finger, bool createIfNull)
		{
			// Find existing link?
			for (var i = 0; i < links.Count; i++)
			{
				var link = links[i];

				if (link.Finger == finger)
				{
					return link;
				}
			}

			// Make new link?
			if (createIfNull == true)
			{
				var link = new Link();

				link.Finger = finger;
				link.Line   = Instantiate(LinePrefab);

				links.Add(link);

				return link;
			}

			return null;
		}
	}
}