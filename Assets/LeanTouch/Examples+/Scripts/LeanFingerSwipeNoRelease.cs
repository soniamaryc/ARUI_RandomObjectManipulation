using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This component detects swipes while the finger is touching the screen
	public class LeanFingerSwipeNoRelease : LeanFingerSwipe
	{
		// This class will store an association between a Finger and cooldown values
		[System.Serializable]
		public class Link
		{
			public LeanFinger Finger; // The finger associated with this link
			public bool Cooldown; // Currently waiting for cooldown to finish?
			public float CooldownTime; // Current cooldown time in seconds
		}

		[Tooltip("Allow multiple swipes for each finger press?")]
		public bool AllowMultiple = true;

		[Tooltip("If multiple swipes are allowed, this is the minimum amount of seconds between each OnFingerSwipe call")]
		public float MultipleSwipeDelay = 0.5f;

		// This stores all the links
		private List<Link> links = new List<Link>();

		protected override void OnEnable()
		{
			// Hook events
			LeanTouch.OnFingerSet += FingerSet;
			LeanTouch.OnFingerUp  += FingerUp;
		}

		protected override void OnDisable()
		{
			// Unhook events
			LeanTouch.OnFingerSet -= FingerSet;
			LeanTouch.OnFingerUp  -= FingerUp;
		}

		protected virtual void Update()
		{
			// Loop through all links
			if (links != null)
			{
				for (var i = 0; i < links.Count; i++)
				{
					var link = links[i];

					// Decrease cooldown?
					if (link.Cooldown == true && AllowMultiple == true)
					{
						link.CooldownTime -= Time.deltaTime;

						if (link.CooldownTime <= 0.0f)
						{
							link.Cooldown = false;
						}
					}
				}
			}
		}

		private void FingerSet(LeanFinger finger)
		{
			// Ignore this finger?
			if (IgnoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (IgnoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelectedBy(finger) == false)
			{
				return;
			}

			// Get link and skip if on cooldown
			var link = FindLink(finger, true);

			if (link.Cooldown == true)
			{
				return;
			}

			// The scaled delta position magnitude required to register a swipe
			var swipeThreshold = LeanTouch.Instance.SwipeThreshold;

			// The amount of seconds we consider valid for a swipe
			var tapThreshold = LeanTouch.CurrentTapThreshold;

			// Get the scaled delta position between now, and 'swipeThreshold' seconds ago
			var recentDelta = finger.GetSnapshotScreenDelta(tapThreshold);

			// Has the finger recently swiped?
			if (recentDelta.magnitude > swipeThreshold)
			{
				if (CheckSwipe(finger, recentDelta) == true)
				{
					// Begin cooldown
					link.CooldownTime = MultipleSwipeDelay;
					link.Cooldown     = true;
				}
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			// Get link and reset cooldown
			var link = FindLink(finger, false);

			if (link != null)
			{
				link.Cooldown = false;
			}
		}

		// Searches through all links for the one associated with the input finger
		public Link FindLink(LeanFinger finger, bool createIfNull)
		{
			for (var i = 0; i < links.Count; i++)
			{
				var link = links[i];

				// Return if it matches
				if (link.Finger == finger)
				{
					return link;
				}
			}

			if (createIfNull == true)
			{
				var link = new Link();

				link.Finger = finger;

				links.Add(link);

				return link;
			}

			return null;
		}
	}
}