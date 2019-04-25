using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to open and close the current RectTransform like a window
	public class LeanWindow : MonoBehaviour
	{
		public bool IsOpen;

		public float Visibility;

		public float Speed = 2.0f;

		[Tooltip("Automatically close this window if the user touches the screen outside of this window?")]
		public bool AutoClose = true;

		[Tooltip("Automatically destroy this GameObject when closing finishes?")]
		public bool AutoDestroy = true;

		public void Open()
		{
			IsOpen = true;
		}

		public void Close()
		{
			IsOpen = false;
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown += FingerDown;

			UpdateScale();
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= FingerDown;
		}

		protected virtual void Update()
		{
			var targetVisibility = 0.0f;

			if (IsOpen == true)
			{
				targetVisibility = 1.0f;
			}
			else if (Visibility == 0.0f)
			{
				Destroy(gameObject);
			}

			// Move visibility to target value
			Visibility = Mathf.MoveTowards(Visibility, targetVisibility, Speed * Time.deltaTime);

			// Scale to visibility
			UpdateScale();
		}

		private void FingerDown(LeanFinger finger)
		{
			if (AutoClose == true)
			{
				// If this popup was tapped, ignore
				var results = LeanTouch.RaycastGui(finger.ScreenPosition);

				for (var i = 0; i < results.Count; i++)
				{
					var result = results[i];

					if (result.gameObject == gameObject)
					{
						return;
					}
				}

				// Close?
				Close();
			}
		}

		private void UpdateScale()
		{
			transform.localScale = Vector3.one * Mathf.SmoothStep(0.0f, 1.0f, Visibility);
		}
	}
}