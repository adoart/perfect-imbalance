using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public float m_DampTime = 0.2f;
	public float m_ScreenEdgeBuffer = 4f;
	public float m_MinSize = 6.5f;
	[HideInInspector] public Transform[] m_Targets;
	
	
	private Camera m_Camera;
	private float m_ZoomSpeed;
	private Vector3 m_MoveVelocity;
	private float m_ConvertDistanceToSize;
	
	
	private void Awake ()
	{
		m_Camera = GetComponentInChildren<Camera> ();
	}
	
	
	private void Start ()
	{
		float relativeVisibleAreaHeight = 1f / Mathf.Sin (Mathf.Deg2Rad * transform.eulerAngles.x);
		float relativeVisibleAreaWidth = m_Camera.aspect;
		
		bool visibleAreaWiderThanTall = relativeVisibleAreaWidth > relativeVisibleAreaHeight;
		
		if (visibleAreaWiderThanTall)
		{
			m_ConvertDistanceToSize = Mathf.Sin (Mathf.Deg2Rad * transform.eulerAngles.x); //1.554
		}
		else
		{
			m_ConvertDistanceToSize = 1f / m_Camera.aspect; //1.764
		}
	}
	
	
	private void FixedUpdate () //updates physics objects...
	{
		Vector3 targetPosition = Move ();
		Zoom (targetPosition);
	}
	
	
	private Vector3 Move()
	{
		Vector3 targetPosition = FindAveragePosition();
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref m_MoveVelocity, m_DampTime);
		
		return targetPosition;
	}
	
	
	private Vector3 FindAveragePosition ()
	{
		Vector3 average = new Vector3 ();
		int numTargets = 0;
		
		for (int i = 0; i < m_Targets.Length; i++)
		{
			if (!m_Targets[i].gameObject.activeSelf)
				continue;
			
			average += m_Targets[i].position;
			numTargets++;
		}
		
		if (numTargets > 0)
			average /= numTargets;
		
		average.y = transform.position.y;
		
		return average;
	}
	
	
	private void Zoom(Vector3 desiredPosition)
	{
		float targetSize = FindRequiredSize(desiredPosition);
		m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, targetSize, ref m_ZoomSpeed, m_DampTime);
	}
	
	
	private float FindRequiredSize(Vector3 desiredPosition)
	{
		float targetDistance = MaxTargetDistance(desiredPosition);

		float newSize = targetDistance * m_ConvertDistanceToSize + m_ScreenEdgeBuffer;
		newSize = Mathf.Max(newSize, m_MinSize);
		
		return newSize;
	}
	
	
	private float MaxTargetDistance (Vector3 desiredPosition)
	{
		float furthestDistance = 0f;
		
		for (int i = 0; i < m_Targets.Length; i++)
		{
			if (!m_Targets[i].gameObject.activeSelf)
				continue;
			
			float targetDistance = (desiredPosition - m_Targets[i].position).magnitude;
			
			if (targetDistance > furthestDistance)
			{
				furthestDistance = targetDistance;
			}
		}
		
		return furthestDistance;
	}
	
	
	public void SetAppropriatePositionAndSize ()
	{
		transform.position = FindAveragePosition ();
		m_Camera.orthographicSize = FindRequiredSize (transform.position);
	}
}