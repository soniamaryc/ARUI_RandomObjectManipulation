using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This component shows you a basic implementation of a block in a match-3 style game
	[ExecuteInEditMode]
	public class LeanMatchBlock : LeanSelectableBehaviour
	{
		// This stores a list of all blocks
		public static List<LeanMatchBlock> Instances = new List<LeanMatchBlock>();

		[Tooltip("Current X grid coordinate of this block")]
		public int X;

		[Tooltip("Current Y grid coordinate of this block")]
		public int Y;

		[Tooltip("The size of the block in world space")]
		public float BlockSize = 2.5f;

		[Tooltip("Auto deselect this block when swapping?")]
		public bool DeselectOnSwap = true;

		[Tooltip("How quickly this block moves to its new position")]
		public float Dampening = 10.0f;

		[Tooltip("This stores the layers we want the overlap to hit (make sure this GameObject's layer is included!)")]
		public LayerMask LayerMask = Physics.DefaultRaycastLayers;

		[Tooltip("The camera used to calculate the ray (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("The conversion method used to find a world point from a screen point")]
		public LeanScreenDepth ScreenDepth;

		public static LeanMatchBlock FindBlock(int x, int y)
		{
			for (var i = Instances.Count - 1; i >= 0; i--)
			{
				var block = Instances[i];

				if (block.X == x && block.Y == y)
				{
					return block;
				}
			}

			return null;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Instances.Add(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Instances.Remove(this);
		}

		public static void Swap(LeanMatchBlock a, LeanMatchBlock b)
		{
			var tempX = a.X;
			var tempY = a.Y;

			a.X = b.X;
			a.Y = b.Y;

			b.X = tempX;
			b.Y = tempY;
		}

		protected virtual void Update()
		{
			// Is this selected and has a selecting finger?
			if (Selectable.IsSelected == true)
			{
				var finger = Selectable.SelectingFinger;

				if (finger != null)
				{
					// Find the world space point under the finger
					var dragPoint = ScreenDepth.Convert(finger.ScreenPosition, Camera, gameObject);

					// Find the block coordinate at this point
					var dragX = Mathf.RoundToInt(dragPoint.x / BlockSize);
					var dragY = Mathf.RoundToInt(dragPoint.y / BlockSize);

					// Is this block right next to this one?
					var distX = Mathf.Abs(X - dragX);
					var distY = Mathf.Abs(Y - dragY);

					if (distX + distY == 1)
					{
						// Swap blocks if one exists at this coordinate
						var block = FindBlock(dragX, dragY);

						if (block != null)
						{
							Swap(this, block);

							if (DeselectOnSwap == true)
							{
								Selectable.Deselect();
							}
						}
					}
				}
			}

			// Smoothly move to new position
			var targetPosition = Vector3.zero;
			var factor         = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			targetPosition.x = X * BlockSize;
			targetPosition.y = Y * BlockSize;

			transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, factor);
		}
	}
}