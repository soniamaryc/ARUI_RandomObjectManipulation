using UnityEngine;

namespace Lean.Touch
{
	// This script will record the state of the current transform, and revert it on command
	public class LeanRevertTransform : MonoBehaviour
	{
		[Tooltip("How quickly this object moves to its original transform.")]
		public float Dampening = 10.0f;

		[Tooltip("Call RecordTransform in Start?")]
		public bool RecordOnStart = true;

		public bool RevertPosition = true;
		public bool RevertRotation = true;
		public bool RevertScale    = true;

		[Space(10.0f)]
		public float ThresholdPosition = 0.01f;
		public float ThresholdRotation = 0.01f;
		public float ThresholdScale    = 0.01f;

		[Space(10.0f)]
		public Vector3    TargetPosition;
		public Quaternion TargetRotation = Quaternion.identity;
		public Vector3    TargetScale = Vector3.one;

		[SerializeField]
		[HideInInspector]
		private Vector3 expectedPosition;

		[SerializeField]
		[HideInInspector]
		private Quaternion expectedRotation = Quaternion.identity;

		[SerializeField]
		[HideInInspector]
		private Vector3 expectedScale = Vector3.one;

		[SerializeField]
		[HideInInspector]
		private bool reverting;

		private bool PositionChanged
		{
			get
			{
				return Vector3.Distance(transform.localPosition, expectedPosition) > ThresholdPosition;
			}
		}

		private bool RotationChanged
		{
			get
			{
				return Quaternion.Angle(transform.localRotation, expectedRotation) > ThresholdRotation;
			}
		}

		private bool ScaleChanged
		{
			get
			{
				return Vector3.Distance(transform.localScale, expectedScale) > ThresholdScale;
			}
		}

		protected virtual void Start()
		{
			if (RecordOnStart == true)
			{
				RecordTransform();
			}
		}

		[ContextMenu("Revert")]
		public void Revert()
		{
			reverting        = true;
			expectedPosition = transform.localPosition;
			expectedRotation = transform.localRotation;
			expectedScale    = transform.localScale;
		}

		[ContextMenu("Stop Revert")]
		public void StopRevert()
		{
			reverting = false;
		}

		[ContextMenu("Record Transform")]
		public void RecordTransform()
		{
			TargetPosition = transform.localPosition;
			TargetRotation = transform.localRotation;
			TargetScale    = transform.localScale;
		}

		protected virtual void Update()
		{
			if (reverting == true)
			{
				// Transform changed externally?
				if (RevertPosition == true && PositionChanged == true)
				{
					reverting = false; return;
				}

				if (RevertRotation == true && RotationChanged == true)
				{
					reverting = false; return;
				}

				if (RevertScale == true && ScaleChanged == true)
				{
					reverting = false; return;
				}

				// Get t value
				var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

				if (RevertPosition == true)
				{
					transform.localPosition = expectedPosition = Vector3.Lerp(transform.localPosition, TargetPosition, factor);
				}

				if (RevertRotation == true)
				{
					transform.localRotation = expectedRotation = Quaternion.Slerp(transform.localRotation, TargetRotation, factor);
				}

				if (RevertScale == true)
				{
					transform.localScale = expectedScale = Vector3. Lerp(transform.localScale, TargetScale, factor);
				}
			}
		}
	}
}