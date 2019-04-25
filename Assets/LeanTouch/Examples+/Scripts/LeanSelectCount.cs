using UnityEngine;
using UnityEngine.UI;

namespace Lean.Touch
{
	// This component can be used with LeanSelect's Re
	public class LeanSelectCount : MonoBehaviour
	{
		[Tooltip("The text element we will modify")]
		public Text NumberText;

		[Tooltip("The amount of times this GameObject has been reselected")]
		public int ReselectCount;

		public void OnSelect(LeanFinger finger)
		{
			ReselectCount += 1;

			NumberText.text = ReselectCount.ToString();
		}

		public void OnDeselect()
		{
			ReselectCount = 0;

			NumberText.text = "";
		}
	}
}