using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This script displays the result of LeanMultiTap in a Text element
	public class LeanMultiSwipeInfo : MonoBehaviour
	{
		[Tooltip("The Text element you want to display the info in")]
		public Text Text;

		// This method is called from the LeanMultiTap event
		public void MultiSwipe(List<LeanFinger> fingers)
		{
			if (Text != null)
			{
				Text.text = "You just multi-swiped with " + fingers.Count + " finger(s)";
			}
		}
	}
}