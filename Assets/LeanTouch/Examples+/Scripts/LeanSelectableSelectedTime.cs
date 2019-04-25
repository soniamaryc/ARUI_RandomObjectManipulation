using UnityEngine;
using UnityEngine.UI;

namespace Lean.Touch
{
	// This component outputs LeanSelectableSelected.Seconds to NumberText
	public class LeanSelectableSelectedTime : LeanSelectableSelected
	{
		[Tooltip("The text element we will modify")]
		public Text NumberText;

		[Tooltip("The text to display when Seconds is exactly 0")]
		public string ZeroText = "0.0";

		protected override void Update()
		{
			base.Update();

			if (NumberText != null)
			{
				if (Seconds == 0.0f)
				{
					NumberText.text = ZeroText;
				}
				else
				{
					NumberText.text = Seconds.ToString("0.0");
				}
			}
		}
	}
}