using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f; 
    public float m_EndDelay = 3f;   
    public CameraControl m_CameraControl;
    public Text m_MessageText;
	public TerrainBuilder terrain;
	public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;
	public Slider powerSlider;



    private int m_RoundNumber;            
    private WaitForSeconds m_StartWait;   
    private WaitForSeconds m_EndWait;     
    private TankManager m_RoundWinner;   
    private TankManager m_GameWinner;    


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }

	void Update() {
		terrain.updateMap (m_Tanks);
		powerSlider.value = terrain.colorAverage;
	}

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_TankRenderers.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
		terrain.resetMap ();

        m_CameraControl.SetAppropriatePositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_MessageText.text = string.Empty;



        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;



        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_TankRenderers.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_TankRenderers.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = "<color=#" + m_RoundWinner.m_PlayerColor.ToString()//.ToHexStringRGB () 
				+ ">PLAYER " + m_RoundWinner.m_PlayerNumber + "</color> WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += "<color=#" + m_Tanks[i].m_PlayerColor.ToString()//.ToHexStringRGB() 
				+ ">PLAYER " + m_Tanks[i].m_PlayerNumber + "</color>: " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = "<color=#" + m_GameWinner.m_PlayerColor.ToString()//.ToHexStringRGB() 
				+ ">PLAYER " + m_GameWinner.m_PlayerNumber + "</color> WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableTank();
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}