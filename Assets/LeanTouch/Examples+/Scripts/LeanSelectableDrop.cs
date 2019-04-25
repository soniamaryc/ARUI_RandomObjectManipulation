using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to change the color of the SpriteRenderer attached to the current GameObject
	public class LeanSelectableDrop : LeanSelectableBehaviour
	{
		public enum SelectType
		{
			Raycast3D,
			Overlap2D,
			CanvasUI
		}

		public enum SearchType
		{
			GetComponent,
			GetComponentInParent,
			GetComponentInChildren
		}

		public SelectType SelectUsing;

		[Tooltip("This stores the layers we want the raycast/overlap to hit (make sure this GameObject's layer is included!)")]
		public LayerMask LayerMask = Physics.DefaultRaycastLayers;

		[Tooltip("How should the selected GameObject be searched for the LeanSelectable component?")]
		public SearchType Search;

		[Tooltip("The camera used to calculate the ray (None = MainCamera)")]
		public Camera Camera;

		protected override void OnSelectUp(LeanFinger finger)
		{
			// Stores the component we hit (Collider or Collider2D)
			var component = default(Component);

			switch (SelectUsing)
			{
				case SelectType.Raycast3D:
				{
					// Make sure the camera exists
					var camera = LeanTouch.GetCamera(Camera, gameObject);

					if (camera != null)
					{
						var ray = camera.ScreenPointToRay(finger.ScreenPosition);
						var hit = default(RaycastHit);

						if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
						{
							component = hit.collider;
						}
					}
					else
					{
						Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
					}
				}
				break;

				case SelectType.Overlap2D:
				{
					// Make sure the camera exists
					var camera = LeanTouch.GetCamera(Camera, gameObject);

					if (camera != null)
					{
						var point = camera.ScreenToWorldPoint(finger.ScreenPosition);

						component = Physics2D.OverlapPoint(point, LayerMask);
					}
					else
					{
						Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
					}
				}
				break;

				case SelectType.CanvasUI:
				{
					var results = LeanTouch.RaycastGui(finger.ScreenPosition, LayerMask);

					if (results != null && results.Count > 0)
					{
						component = results[0].gameObject.transform;
					}
				}
				break;
			}

			// Select the component
			Drop(finger, component);
		}

		private void Drop(LeanFinger finger, Component component)
		{
			// Stores the droppable we will search for
			var droppable = default(IDroppable);

			// Was a collider found?
			if (component != null)
			{
				switch (Search)
				{
					case SearchType.GetComponent:           droppable = component.GetComponent          <IDroppable>(); break;
					case SearchType.GetComponentInParent:   droppable = component.GetComponentInParent  <IDroppable>(); break;
					case SearchType.GetComponentInChildren: droppable = component.GetComponentInChildren<IDroppable>(); break;
				}
			}

			// Drop?
			if (droppable != null)
			{
				droppable.OnDrop(gameObject, finger);
			}
		}
	}
}