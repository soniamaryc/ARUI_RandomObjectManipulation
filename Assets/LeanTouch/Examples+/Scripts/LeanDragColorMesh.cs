using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to paint the vertices of the current MeshFilter's mesh
	[RequireComponent(typeof(MeshFilter))]
	public class LeanDragColorMesh : MonoBehaviour
	{
		[Tooltip("The color you want to paint the hit triangles")]
		public Color PaintColor;

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Allows you to force translation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does translation require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The camera the translation will be calculated using (default = MainCamera)")]
		public Camera Camera;

		// The cached mesh filter
		[System.NonSerialized]
		private MeshFilter cachedMeshFilter;

		// Stores a duplicate of the MeshFilter's mesh
		private Mesh modifiedMesh;

		private int[] modifiedIndices;

		// Stores the current vertex position array
		private Color[] modifiedColors;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Start();
		}
#endif
		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSet += Paint;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSet -= Paint;
		}

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponent<LeanSelectable>();
			}
		}

		private void Paint(LeanFinger finger)
		{
			// Ignore?
			if (IgnoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (IgnoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			// Make sure the mesh filter and mesh exist
			if (cachedMeshFilter == null) cachedMeshFilter = GetComponent<MeshFilter>();

			if (cachedMeshFilter.sharedMesh != null)
			{
				// Duplicate mesh?
				if (modifiedMesh == null)
				{
					modifiedMesh = cachedMeshFilter.sharedMesh = Instantiate(cachedMeshFilter.sharedMesh);
				}

				// Duplicate indices and colors?
				if (modifiedColors == null || modifiedColors.Length != modifiedMesh.vertexCount)
				{
					modifiedIndices = modifiedMesh.triangles;
					modifiedColors  = modifiedMesh.colors;

					// If the mesh has no vertex colors, make some
					if (modifiedColors == null || modifiedColors.Length == 0)
					{
						modifiedColors = new Color[modifiedMesh.vertexCount];

						for (var i = modifiedMesh.vertexCount - 1; i >= 0; i--)
						{
							modifiedColors[i] = Color.white;
						}
					}
				}

				// Raycast under the finger and paint the hit triangle
				var hit = default(RaycastHit);

				if (Physics.Raycast(finger.GetRay(Camera), out hit) == true)
				{
					if (hit.collider.gameObject == gameObject)
					{
						var index = hit.triangleIndex * 3;
						var a     = modifiedIndices[index + 0];
						var b     = modifiedIndices[index + 1];
						var c     = modifiedIndices[index + 2];
							
						modifiedColors[a] = Color.black;
						modifiedColors[b] = Color.black;
						modifiedColors[c] = Color.black;

						modifiedMesh.colors = modifiedColors;
					}
				}
			}
		}
	}
}