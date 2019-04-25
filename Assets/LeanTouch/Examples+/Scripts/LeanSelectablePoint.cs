using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This component allows you to specify a point that can be selected by the LeanCanvasSelectionBox component
	public class LeanSelectablePoint : MonoBehaviour
	{
		public List<LeanSelectablePoint> Instances = new List<LeanSelectablePoint>();

		[Tooltip("The size of the select AABB")]
		public Bounds Bounds;

		// This calculates the viewport AABB
		public Rect CalculateRect(Camera camera)
		{
			var rect = default(Rect);

			if (camera != null)
			{
				var min = Bounds.min;
				var max = Bounds.max;

				var a = camera.WorldToViewportPoint(transform.TransformPoint(min.x, min.y, min.z));
				var b = camera.WorldToViewportPoint(transform.TransformPoint(max.x, min.y, min.z));
				var c = camera.WorldToViewportPoint(transform.TransformPoint(min.x, min.y, max.z));
				var d = camera.WorldToViewportPoint(transform.TransformPoint(max.x, min.y, max.z));
				var e = camera.WorldToViewportPoint(transform.TransformPoint(min.x, max.y, min.z));
				var f = camera.WorldToViewportPoint(transform.TransformPoint(max.x, max.y, min.z));
				var g = camera.WorldToViewportPoint(transform.TransformPoint(min.x, max.y, max.z));
				var h = camera.WorldToViewportPoint(transform.TransformPoint(max.x, max.y, max.z));

				rect.center = a;

				Expand(ref rect, b);
				Expand(ref rect, c);
				Expand(ref rect, d);
				Expand(ref rect, e);
				Expand(ref rect, f);
				Expand(ref rect, g);
				Expand(ref rect, h);
			}

			return rect;
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Add(this);
		}

		private void Expand(ref Rect rect, Vector2 xy)
		{
			if (xy.x < rect.xMin) rect.xMin = xy.x; else if (xy.x > rect.xMax) rect.xMax = xy.x;
			if (xy.y < rect.yMin) rect.yMin = xy.y; else if (xy.y > rect.yMax) rect.yMax = xy.y;
		}
	}
}