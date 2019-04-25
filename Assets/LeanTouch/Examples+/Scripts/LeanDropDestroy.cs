using UnityEngine;
using UnityEngine.UI;

namespace Lean.Touch
{
	// This component implements the 'IDroppable' interface, and will destroy the passed 'droppedGameObject'
	public class LeanDropDestroy : MonoBehaviour, IDroppable
	{
		[Tooltip("The text element we will modify")]
		public Text NumberText;

		[Tooltip("The amount of times this component has destroyed dropped GameObjects")]
		public int DestroyCount;

		// Implemented from the IDroppable interface
		public void OnDrop(GameObject droppedGameObject, LeanFinger finger)
		{
			Destroy(droppedGameObject);

			DestroyCount += 1;

			NumberText.text = "Destroyed " + DestroyCount + "!";
		}
	}
}