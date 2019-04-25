using UnityEngine;

namespace Lean.Touch
{
	// This script will swap the target GameObject with one of the specified prefabs when swiping
	public class LeanSwap : MonoBehaviour
	{
		[Tooltip("The GameObject that was swapped")]
		public GameObject Target;

		[Tooltip("The current index of Target within the Prefabs array")]
		public int Index;

		[Tooltip("The alternative prefabs that can be swapped to")]
		public GameObject[] Prefabs;

		[ContextMenu("Swap")]
		public void Swap()
		{
			if (Prefabs != null && Prefabs.Length > 0)
			{
				// Wrap index to stay within Prefabs.length
				Index %= Prefabs.Length;

				if (Index < 0)
				{
					Index += Prefabs.Length;
				}

				var clone = Instantiate(Prefabs[Index]);

				// Copy transform data from previous target?
				if (clone != null && Target != null)
				{
					clone.transform.SetParent(Target.transform.parent, false);

					clone.transform.position = Target.transform.position;
					clone.transform.rotation = Target.transform.rotation;

					Destroy(Target);
				}

				Target = clone;
			}
		}

		[ContextMenu("Swap To Previous")]
		public void SwapToPrevious()
		{
			Index -= 1;

			Swap();
		}

		[ContextMenu("Swap To Next")]
		public void SwapToNext()
		{
			Index += 1;

			Swap();
		}
	}
}