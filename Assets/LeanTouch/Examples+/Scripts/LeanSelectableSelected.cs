using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This component fires events when the selectable has been selected for a certain amount of time
	public class LeanSelectableSelected : LeanSelectableBehaviour
	{
		public enum ResetType
		{
			None,
			OnSelect,
			OnDeselect
		}

		// Event signature
		[System.Serializable] public class SelectableEvent : UnityEvent<LeanSelectable> {}

		[Tooltip("The amount of seconds this has been selected")]
		public float Seconds;

		[Tooltip("The finger must be held for this many seconds")]
		public float Threshold = 1.0f;

		[Tooltip("When should Seconds be reset to 0?")]
		public ResetType Reset = ResetType.OnDeselect;

		[Tooltip("Bypass LeanSelectable.HideWithFinger?")]
		public bool RawSelection;

		[Tooltip("If the selecting finger went up, cancel timer?")]
		public bool RequireFinger;

		// Called on the first frame the conditions are met
		public SelectableEvent OnDown;

		// Called on every frame the conditions are met
		public SelectableEvent OnSet;

		// Called on the last frame the conditions are met
		public SelectableEvent OnUp;

		private bool lastSet;

		protected virtual void Update()
		{
			// See if the timer can be incremented
			var set = false;

			if (Selectable.GetIsSelected(RawSelection) == true)
			{
				if (RequireFinger == false || Selectable.SelectingFinger != null)
				{
					Seconds += Time.deltaTime;

					if (Seconds >= Threshold)
					{
						set = true;
					}
				}
			}

			// If this is the first frame of set, call down
			if (set == true && lastSet == false)
			{
				if (OnDown != null)
				{
					OnDown.Invoke(Selectable);
				}
			}

			// Call set every time if set
			if (set == true)
			{
				if (OnSet != null)
				{
					OnSet.Invoke(Selectable);
				}
			}

			// Store last value
			lastSet = set;
		}

		protected override void OnSelect(LeanFinger finger)
		{
			if (Reset == ResetType.OnSelect)
			{
				Seconds = 0.0f;
			}

			// Reset value
			lastSet = false;
		}

		protected override void OnDeselect()
		{
			if (Reset == ResetType.OnDeselect)
			{
				Seconds = 0.0f;
			}

			if (lastSet == true)
			{
				if (OnUp != null)
				{
					OnUp.Invoke(Selectable);
				}
			}
		}
	}
}