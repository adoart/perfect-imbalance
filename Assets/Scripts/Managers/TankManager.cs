using UnityEngine;
using System;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;
    public Transform m_SpawnPoint;
    [HideInInspector]
    public int m_PlayerNumber;
    [HideInInspector]
    public GameObject m_Instance;
    [HideInInspector]
    public GameObject m_TankRenderers;
    [HideInInspector]
    public int m_Wins;
	public int colorRadius = 10;

    private TankMovement m_Movement;
    private TankShooting m_Shooting;
    private TankHealth m_Health;
    private BoxCollider m_Collider;
    private GameObject m_HealthCanvas;
    private GameObject m_AimCanvas;
    private GameObject m_LeftDustTrail;
    private GameObject m_RightDustTrail;


    public void Setup()
    {
        m_TankRenderers = m_Instance.transform.Find("TankRenderers").gameObject;
        m_HealthCanvas = m_Instance.transform.Find("HealthCanvas").gameObject;
        m_AimCanvas = m_Instance.transform.Find("AimCanvas").gameObject;
        m_LeftDustTrail = m_Instance.transform.Find("LeftDustTrail").gameObject;
        m_RightDustTrail = m_Instance.transform.Find("RightDustTrail").gameObject;
        
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_Health = m_Instance.GetComponent<TankHealth>();
        m_Collider = m_Instance.GetComponent<BoxCollider>();

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        Renderer[] renderers = m_TankRenderers.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void EnableTank()
    {
        m_Collider.enabled = true;

        m_TankRenderers.SetActive(true);
        m_HealthCanvas.SetActive(true);
        m_AimCanvas.SetActive(true);
        m_LeftDustTrail.SetActive(true);
        m_RightDustTrail.SetActive(true);
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_Movement.ReEnableParticles();
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Movement.SetDefaults();
        m_Shooting.SetDefaults();
        m_Health.SetDefaults();
    }

	public void updateColorRadius(float colorAverage) {
		if (m_PlayerColor.grayscale > 0.5f) {
			colorRadius = convertAverageToRadius(colorAverage);
			//Debug.Log("color " + m_PlayerColor.grayscale + " radius:" + colorRadius + " average: " + colorAverage);
		} else {
			colorRadius = convertAverageToRadius(1.0f - colorAverage);
			//Debug.Log("color " + m_PlayerColor.grayscale + " radius:" + colorRadius + " average: " +  (1.0f - colorAverage));
		}
	}

	public void updateHealth(float colorAverage) {
		if (m_PlayerColor.grayscale > 0.5f) {
			m_Health.m_StartingHealth = convertAverageToHealth(colorAverage);			
		} else {
			m_Health.m_StartingHealth = convertAverageToHealth(1.0f - colorAverage);
		}
	}

	public void updateSpeed(float colorAverage) {
		if (m_PlayerColor.grayscale > 0.5f) {
			m_Movement.m_Speed = convertAverageToSpeed(colorAverage);			
		} else {
			m_Movement.m_Speed = convertAverageToSpeed(1.0f - colorAverage);
		}
	}


	private int convertAverageToRadius(float colorAverage) {
		int roundedToInt = Mathf.CeilToInt (colorAverage * 100f);

		if(roundedToInt >= 50 && roundedToInt < 52) {
			return 10;
		} else if (roundedToInt >= 52 && roundedToInt < 55) {
			return 15;
		} else if (roundedToInt >= 55 && roundedToInt < 60) {
			return 25;
		} else if (roundedToInt >= 60 && roundedToInt < 80) {
			return 30;
		} else if (roundedToInt >= 80 && roundedToInt < 100) {
			return 50;
		}

		//shoulnd't happen...
		return 10;
	}

	private int convertAverageToHealth(float colorAverage) {
		int roundedToInt = Mathf.CeilToInt (colorAverage * 100f);
		
		if(roundedToInt >= 50 && roundedToInt < 52) {
			return 100;
		} else if (roundedToInt >= 52 && roundedToInt < 55) {
			return 150;
		} else if (roundedToInt >= 55 && roundedToInt < 60) {
			return 250;
		} else if (roundedToInt >= 60 && roundedToInt < 80) {
			return 300;
		} else if (roundedToInt >= 80 && roundedToInt < 100) {
			return 500;
		}
		
		//shoulnd't happen...
		return 100;
	}

	private float convertAverageToSpeed(float colorAverage) {
		int roundedToInt = Mathf.CeilToInt (colorAverage * 100f);
		
		if(roundedToInt >= 50 && roundedToInt < 52) {
			return 10f;
		} else if (roundedToInt >= 52 && roundedToInt < 55) {
			return 15f;
		} else if (roundedToInt >= 55 && roundedToInt < 60) {
			return 20f;
		} else if (roundedToInt >= 60 && roundedToInt < 80) {
			return 25f;
		} else if (roundedToInt >= 80 && roundedToInt < 100) {
			return 30f;
		}
		
		//shoulnd't happen...
		return 10f;
	}



}