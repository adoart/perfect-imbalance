using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;         
    public Slider m_Slider;                       
    public Image m_FillImage;                           
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;    
    public AudioClip m_TankExplosion;             
    public ParticleSystem m_ExplosionParticles;   
    public GameObject m_TankRenderers;            
    public GameObject m_HealthCanvas;
    public GameObject m_LeftDustTrail;
    public GameObject m_RightDustTrail;
	public GameObject m_AimCanvas;

    
    private float m_CurrentHealth;                
    private bool m_ZeroHealthHappened;              
    private BoxCollider m_Collider;               
    

    void Awake()
    {
        m_Collider = GetComponent<BoxCollider>();
    }
    

    public void Damage(float amount)
    {
        // Deduct the amount from the current health and check the tank has health left.
		m_CurrentHealth -= amount;

		SetHealthUI ();

		if (m_CurrentHealth <= 0f && !m_ZeroHealthHappened) {
			OnZeroHealth();
		}
    }


    private void SetHealthUI()
    {
        // Change the size and color of the health bar based on the current health.
		m_Slider.value = m_CurrentHealth;
		m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnZeroHealth ()
    {
        // Activate the explosion effects and turn off everything that should be off when the tank has no health left.
		m_ZeroHealthHappened = true;
		m_ExplosionParticles.Play ();
		AudioSource.PlayClipAtPoint (m_TankExplosion, transform.position);

		m_Collider.enabled = false;
		m_TankRenderers.SetActive (false);
		m_HealthCanvas.SetActive (false);
		m_AimCanvas.SetActive (false);
		m_LeftDustTrail.SetActive (false);
		m_RightDustTrail.SetActive (false);
    }


    public void SetDefaults()
    {
        m_CurrentHealth = m_StartingHealth;
        m_ZeroHealthHappened = false;

        SetHealthUI();
    }
}