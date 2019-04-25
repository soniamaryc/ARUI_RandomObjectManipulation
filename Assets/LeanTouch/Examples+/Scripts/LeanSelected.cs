using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	// This script will trigger events based on the selection of LeanSelectables
	public class LeanSelected : MonoBehaviour
	{
		[Tooltip("Set to true when at least one LeanSelectable was selected last frame.")]
		public bool Selected;

		// Called when at least one LeanSelectable is selected
		public UnityEvent OnSelectedStart;

		// Called when all LeanSelectables are deselected
		public UnityEvent OnSelectedEnd;

		protected virtual void Update()
		{
			if (Selected == true)
			{
				if (LeanSelectable.IsSelectedCount == 0)
				{
					Selected = false;

					if (OnSelectedEnd != null)
					{
						OnSelectedEnd.Invoke();
					}
				}
			}
			else
			{
				if (LeanSelectable.IsSelectedCount > 0)
				{
					Selected = true;

					if (OnSelectedStart != null)
					{
						OnSelectedStart.Invoke();
					}
				}
			}
		}
	}
}