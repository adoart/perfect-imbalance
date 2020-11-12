﻿using UnityEngine;

namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f;                   // Approximate time for the camera to refocus.
        public float m_ScreenEdgeBuffer = 4f;             // Space between the top/bottom most target and the screen edge (multiplied by aspect for left and right).
        public float m_MinSize = 6.5f;                    // The smallest orthographic size the camera can be.
        [HideInInspector] public Transform[] m_Targets;   // All the targets the camera needs to encompass.


        private Camera m_Camera;                        // Used for referencing the camera.
        private float m_ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
        private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
        private float m_ConvertDistanceToSize;          // Used to multiply by the offset of the rig to the furthest target.


        private void Awake ()
        {
            m_Camera = GetComponentInChildren<Camera> ();
        }


        private void Start ()
        {
			// We need to know whether the tanks can go off the top/bottom or left/right of the screen first
			// as their distances from the centre of the visible area increases.
			// To find out we find the 'relative' height and width of the visible area.
			float relativeVisibleAreaHeight = 1f / Mathf.Sin (Mathf.Deg2Rad * transform.eulerAngles.x);
			float relativeVisibleAreaWidth = m_Camera.aspect;

			// We now can compare the relative height and width to find out whether the visible area is
			// landscape or portrait shape.
			bool visibleAreaWiderThanTall = relativeVisibleAreaWidth > relativeVisibleAreaHeight;

			// If the visible area is wider than it is tall...
			if (visibleAreaWiderThanTall)
			{
				// ... the convertion from distance to size should be based on Sin X.
				m_ConvertDistanceToSize = Mathf.Sin (Mathf.Deg2Rad * transform.eulerAngles.x);
			}
			else
			{
				// If the visible area is taller than it is wide the convertion should be based on the aspect.
				m_ConvertDistanceToSize = 1f / m_Camera.aspect;
			}
        }


        private void FixedUpdate ()
        {
            // The camera is moved towards a target position which is returned.
            Vector3 targetPosition = Move ();

            // The size is changed based on where the camera is going to be.
            Zoom (targetPosition);
        }


        private Vector3 Move()
        {
            // Find the average position of the targets and smoothly transition to that position.
            Vector3 targetPosition = FindAveragePosition();
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref m_MoveVelocity, m_DampTime);

            return targetPosition;
        }

        
        private Vector3 FindAveragePosition ()
        {
            Vector3 average = new Vector3 ();
            int numTargets = 0;

            // Go through all the targets and add their positions together.
            for (int i = 0; i < m_Targets.Length; i++)
            {
                // If the target isn't active, go on to the next one.
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // Add to the average and increment the number of targets in the average.
                average += m_Targets[i].position;
                numTargets++;
            }

            // If there are targets divide the sum of the positions by the number of them to find the average.
            if (numTargets > 0)
                average /= numTargets;

            // Keep the same y value.
            average.y = transform.position.y;

            return average;
        }


        private void Zoom(Vector3 desiredPosition)
        {
            // Find the required size based on the desired position
            float targetSize = FindRequiredSize(desiredPosition);
            m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, targetSize, ref m_ZoomSpeed, m_DampTime);
        }


        private float FindRequiredSize(Vector3 desiredPosition)
        {
            // Find how far from the rig to the furthest target.
            float targetDistance = MaxTargetDistance(desiredPosition);

            // Calculate the size based on the previously found ratio and buffer.
            float newSize = targetDistance * m_ConvertDistanceToSize + m_ScreenEdgeBuffer;

            // Restrict the new size so that it's not smaller than the minimum size.
            newSize = Mathf.Max(newSize, m_MinSize);

            return newSize;
        }


        private float MaxTargetDistance (Vector3 desiredPosition)
        {
            // Default furthest distance is no distance at all.
            float furthestDistance = 0f;

            // Go through all the targets and if they are further away use that distance instead.
            for (int i = 0; i < m_Targets.Length; i++)
            {
                // If the target isn't active, on to the next one.
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // Find the distance from the camera's desired position to the target.
                float targetDistance = (desiredPosition - m_Targets[i].position).magnitude;

                // If it's greater than the current furthest distance, it's the furthest distance.
                if (targetDistance > furthestDistance)
                {
                    furthestDistance = targetDistance;
                }
            }

            // Return the distance to the target that is furthest away.
            return furthestDistance;
        }


        public void SetAppropriatePositionAndSize ()
        {
            // Set orthographic size and position without damping.
            transform.position = FindAveragePosition ();
            m_Camera.orthographicSize = FindRequiredSize (transform.position);
        }
    }
}