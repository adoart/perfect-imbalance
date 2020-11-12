using System;
using UnityEngine;

namespace Complete
{
    [Serializable]
    public class TankManager
    {
        // This class is to manage various settings on a tank.
        // It works with the GameManager class to control how the tanks behave
        // and whether or not players have control of their tank in the 
        // different phases of the game.

        public Color m_PlayerColor;               // This is the color this tank will be tinted.
        public Transform m_SpawnPoint;            // The position and direction the tank will have when it spawns.
        [HideInInspector]
        public int m_PlayerNumber;                // This specifies which player this the manager for.
        [HideInInspector]
        public GameObject m_Instance;             // A reference to the instance of the tank when it is created.
        [HideInInspector]
        public GameObject m_TankRenderers;        // The transform that is a parent of all the tank's renderers.  This is deactivated when the tank is dead.
        [HideInInspector]
        public int m_Wins;                        // The number of wins this player has so far.
        

        private TankMovement m_Movement;        // References to various objects for control during the different game phases.
        private TankShooting m_Shooting;
        private TankHealth m_Health;
        private BoxCollider m_Collider;
        private GameObject m_HealthCanvas;
        private GameObject m_AimCanvas;
        private GameObject m_LeftDustTrail;
        private GameObject m_RightDustTrail;


        public void Setup ()
        {
            // Get references to the child objects.
            m_TankRenderers = m_Instance.transform.Find("TankRenderers").gameObject;
            m_HealthCanvas = m_Instance.transform.Find("HealthCanvas").gameObject;
            m_AimCanvas = m_Instance.transform.Find("AimCanvas").gameObject;
            m_LeftDustTrail = m_Instance.transform.Find("LeftDustTrail").gameObject;
            m_RightDustTrail = m_Instance.transform.Find("RightDustTrail").gameObject;

            // Get references to the components.
            m_Movement = m_Instance.GetComponent<TankMovement> ();
            m_Shooting = m_Instance.GetComponent<TankShooting> ();
            m_Health = m_Instance.GetComponent<TankHealth> ();
            m_Collider = m_Instance.GetComponent<BoxCollider> ();

            // Set the player numbers to be consistent across the scripts.
            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerNumber = m_PlayerNumber;

            // Get all of the renderers of the tank.
            Renderer[] renderers = m_TankRenderers.GetComponentsInChildren<Renderer> ();

            // Go through all the renderers...
            for (int i = 0; i < renderers.Length; i++)
            {
                // ... set their material color to the color specific to this tank.
                renderers[i].material.color = m_PlayerColor;
            }
        }


        // Used when the round resets and the tanks need to show up in the scene again.
        public void EnableTank ()
        {
            m_Collider.enabled = true;

            m_TankRenderers.SetActive(true);
            m_HealthCanvas.SetActive (true);
            m_AimCanvas.SetActive (true);
            m_LeftDustTrail.SetActive (true);
            m_RightDustTrail.SetActive (true);
        }


        // Used during the phases of the game where the player shouldn't be able to control their tank.
        public void DisableControl ()
        {
            m_Movement.enabled = false;
            m_Shooting.enabled = false;
        }


        // Used during the phases of the game where the player should be able to control their tank.
        public void EnableControl ()
        {
            m_Movement.enabled = true;
            m_Shooting.enabled = true;

            m_Movement.ReEnableParticles ();
        }


        // Used at the start of each round to put the tank into it's default state.
        public void Reset ()
        {
            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            m_Movement.SetDefaults ();
            m_Shooting.SetDefaults ();
            m_Health.SetDefaults ();
        }
    }
}