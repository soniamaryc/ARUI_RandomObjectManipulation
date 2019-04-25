using UnityEngine;

namespace Lean.Touch
{
	// LeanSelectableDrop finds components that implement this interface so drag and drop actions can be completed
	public interface IDroppable
	{
		void OnDrop(GameObject droppedGameObject, LeanFinger finger);
	}
}