﻿using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;       
    public float m_MaxDamage = 100f;           
    public float m_ExplosionForce = 1000f;     
    public float m_MaxLifeTime = 2f;           
    public float m_ExplosionRadius = 5f;       

    
    private int m_TankMask;                    


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);

        m_TankMask = LayerMask.GetMask("Players");
    }
    


    private void OnTriggerEnter (Collider other)
    {
        // Find all the targets around the explosion and damage them.

		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);
		for (int i = 0; i < colliders.Length; i++) {
			Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
			if(!targetRigidbody)
				continue;

			targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

			if(!targetHealth)
				continue;

			Vector3 explosionToTarget = targetRigidbody.position - transform.position;
			float explosionDistance = explosionToTarget.magnitude;

			float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
			float damage = relativeDistance * m_MaxDamage;

			damage = Mathf.Max (0f, damage);

			targetHealth.Damage(damage);
		}

		m_ExplosionParticles.transform.parent = null;
		m_ExplosionParticles.Play ();
		m_ExplosionAudio.Play ();

		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
		Destroy (gameObject);
    }
}