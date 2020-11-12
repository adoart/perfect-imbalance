using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
	public float m_PitchRange = 0.2f;
	public ParticleSystem m_LeftDustParticles;
    public ParticleSystem m_RightDustParticles;

    
    private string m_MovementAxis;
    private string m_TurnAxis;
    private Rigidbody m_Rigidbody;
    private float m_MovementInput;
    private float m_TurnInput;
    private float m_OriginalPitch;
    

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        m_MovementAxis = "Vertical" + m_PlayerNumber;
        m_TurnAxis = "Horizontal" + m_PlayerNumber;
     
        m_OriginalPitch = m_MovementAudio.pitch;
    }



    private void Update ()
    {
        // Store the players input and make sure the audio for the engine is playing.

		m_MovementInput = Input.GetAxis (m_MovementAxis);
		m_TurnInput = Input.GetAxis (m_TurnAxis);

		EngineAudio ();
    }


    private void EngineAudio ()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

		if (Mathf.Abs (m_MovementInput) < 0.1f && Mathf.Abs (m_TurnInput) < 0.1f) {
			if (m_MovementAudio.clip == m_EngineDriving) {
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		} else {
			if (m_MovementAudio.clip == m_EngineIdling) {
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play ();
			}
		}
    }


    private void FixedUpdate ()
    {
        // Move and turn the tank.
		Move ();
		Turn ();
    }


    private void Move ()
    {
        // Adjust the position of the tank based on the player's input.
		Vector3 movement = transform.forward * m_MovementInput * m_Speed * Time.deltaTime;

		m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
    }


    private void Turn ()
    {
        // Adjust the rotation of the tank based on the player's input.
		float turn = m_TurnInput * m_TurnSpeed * Time.deltaTime;

		Quaternion inputRotation = Quaternion.Euler (0f, turn, 0f);

		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * inputRotation);
    }


    public void SetDefaults()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        m_MovementInput = 0f;
        m_TurnInput = 0f;
        
        m_LeftDustParticles.Clear();
        m_LeftDustParticles.Stop();

        m_RightDustParticles.Clear();
        m_RightDustParticles.Stop();
    }


    public void ReEnableParticles()
    {
        m_LeftDustParticles.Play();
        m_RightDustParticles.Play();
    }
}