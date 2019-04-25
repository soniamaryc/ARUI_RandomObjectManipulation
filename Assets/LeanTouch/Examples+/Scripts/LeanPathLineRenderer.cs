#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
	#define UNITY_OLD_LINE_RENDERER
#endif
using UnityEngine;

namespace Lean.Touch
{
	// This component copies path points to a line renderer
	[ExecuteInEditMode]
	public class LeanPathLineRenderer : MonoBehaviour
	{
		[Tooltip("The path that will be used")]
		public LeanPath Path;

		[Tooltip("The line renderer the path will be output to")]
		public LineRenderer LineRenderer;

		[Tooltip("The amount of lines between each path point")]
		public int Smoothing = 1;

		protected virtual void Update()
		{
			if (Path != null && LineRenderer != null)
			{
				var pointCount = Path.PointCount;

				if (Smoothing > 1)
				{
					var smoothedPointCount = Path.GetPointCount(Smoothing);
					var smoothedStep       = 1.0f / Smoothing;

					SetPointCount(smoothedPointCount);

					for (var i = 0; i < smoothedPointCount; i++)
					{
						SetPoint(i, Path.GetSmoothedPoint(i * smoothedStep));
					}
				}
				else
				{
					SetPointCount(pointCount);

					for (var i = 0; i < pointCount; i++)
					{
						SetPoint(i, Path.GetPoint(i));
					}
				}
			}
		}

		private void SetPointCount(int pointCount)
		{
#if UNITY_OLD_LINE_RENDERER
			LineRenderer.SetVertexCount(pointCount);
#else
			LineRenderer.positionCount = pointCount;
#endif
		}

		private void SetPoint(int index, Vector3 point)
		{
			LineRenderer.SetPosition(index, point);
		}
	}
}