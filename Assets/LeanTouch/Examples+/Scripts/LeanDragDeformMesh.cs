using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to drag mesh vertices using finger drags
	[RequireComponent(typeof(MeshFilter))]
	public class LeanDragDeformMesh : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		[Tooltip("Allows you to force translation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does translation require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("Radius around the finger the vertices will be moved in scaled screen space")]
		public float ScaledRadius = 50.0f;

		[Tooltip("Should mesh deformation be applied to an attached MeshCollider?")]
		public bool ApplyToMeshCollider;

		[Tooltip("The camera the translation will be calculated using (None = MainCamera)")]
		public Camera Camera;

		// The cached mesh filter
		[System.NonSerialized]
		private MeshFilter cachedMeshFilter;

		// The cached mesh collider
		[System.NonSerialized]
		private MeshCollider cachedMeshCollider;

		// Stores a duplicate of the MeshFilter's mesh
		private Mesh deformedMesh;

		// Stores the current vertex position array
		private Vector3[] deformedVertices;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Start();
		}
#endif

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponent<LeanSelectable>();
			}
		}

		protected virtual void Update()
		{
			// If we require a selectable and it isn't selected, cancel
			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				// Get the fingers we want to use to translate
				var fingers = LeanTouch.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount);

				if (cachedMeshFilter == null) cachedMeshFilter = GetComponent<MeshFilter>();

				if (cachedMeshFilter.sharedMesh != null)
				{
					// Duplicate mesh?
					if (deformedMesh == null)
					{
						deformedMesh = cachedMeshFilter.sharedMesh = Instantiate(cachedMeshFilter.sharedMesh);
					}

					// Duplicate vertices
					if (deformedVertices == null || deformedVertices.Length != cachedMeshFilter.sharedMesh.vertexCount)
					{
						deformedVertices = cachedMeshFilter.sharedMesh.vertices;
					}

					var scalingFactor = LeanTouch.ScalingFactor;
					var deformed      = false;

					// Go through all vertices and find the screen point
					for (var i = deformedVertices.Length - 1; i >= 0; i--)
					{
						var worldPoint  = transform.TransformPoint(deformedVertices[i]);
						var screenPoint = camera.WorldToScreenPoint(worldPoint);

						// Go through all fingers for this vertex
						for (var j = fingers.Count - 1; j >= 0; j--)
						{
							var finger     = fingers[j];
							var scaledDist = Vector2.Distance(screenPoint, finger.ScreenPosition) * scalingFactor;

							// Is this finger within the required scaled radius of the vertex?
							if (scaledDist <= ScaledRadius)
							{
								deformed = true;

								// Shift screen point
								screenPoint.x += finger.ScreenDelta.x;
								screenPoint.y += finger.ScreenDelta.y;

								// Untransform it back to local space and write
								worldPoint = camera.ScreenToWorldPoint(screenPoint);

								deformedVertices[i] = transform.InverseTransformPoint(worldPoint);
							}
						}
					}

					// If deformed, apply changes
					if (deformed == true)
					{
						deformedMesh.vertices = deformedVertices;

						deformedMesh.RecalculateBounds();
						deformedMesh.RecalculateNormals();

						if (ApplyToMeshCollider == true)
						{
							if (cachedMeshCollider == null) cachedMeshCollider = GetComponent<MeshCollider>();

							if (cachedMeshCollider != null)
							{
								cachedMeshCollider.sharedMesh = null; // Set to null first to force it to update
								cachedMeshCollider.sharedMesh = deformedMesh;
							}
						}
					}
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}