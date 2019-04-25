using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to open the specified window as a child of a specific parent
	public class LeanWindowOpener : MonoBehaviour
	{
		public enum OpenType
		{
			DoNothing,
			Close,
			CloseAndOpenNewInstance
		}

		[Tooltip("The window prefab that will be spawned")]
		public LeanWindow Prefab;

		[Tooltip("The transform the window will spawn as a child of")]
		public RectTransform Parent;

		[Tooltip("If you open a window when one is already open, what should happen?")]
		public OpenType IfAlreadyOpen;

		[Tooltip("The window that was opened")]
		public LeanWindow Instance;

		[ContextMenu("Open")]
		public void Open()
		{
			if (Prefab != null)
			{
				if (Instance != null && Instance.IsOpen == true)
				{
					switch (IfAlreadyOpen)
					{
						case OpenType.DoNothing:
						{
							return;
						}
						//break;

						case OpenType.Close:
						{
							Instance.Close();

							Instance = null;

							return;
						}
						//break;

						case OpenType.CloseAndOpenNewInstance:
						{
							Instance.Close();

							Instance = null;
						}
						break;
					}
				}

				if (Instance == null)
				{
					Instance = Instantiate(Prefab);

					Instance.transform.SetParent(Parent, false);
				}

				Instance.Open();
			}
		}
	}
}