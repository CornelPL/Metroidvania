using System.Collections.Generic;
using UnityEngine;

public class BloodParticleSplash : MonoBehaviour
{
    [SerializeField] private GameObject splashEffect = null;
    [SerializeField] private float sizeMultiplier = 2f;


    private List<ParticleCollisionEvent> collisionEvents;


    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }


    private void OnParticleCollision( GameObject other )
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        int numCollisionEvents = particleSystem.GetCollisionEvents( other, collisionEvents );

        for ( int i = 0; i < numCollisionEvents; i++ )
        {
            ParticleCollisionEvent collisionEvent = collisionEvents[ i ];

            float angle = -Mathf.Atan2( collisionEvent.normal.y, collisionEvent.normal.x ) * Mathf.Rad2Deg + 90f;

            GameObject instantiated = Instantiate( splashEffect, collisionEvent.intersection, Quaternion.identity, null );

            ParticleSystem.MainModule main = instantiated.GetComponent<ParticleSystem>().main;
            main.startSize = particleSystem.main.startSize.constantMax * sizeMultiplier;

            main.startRotation = angle * Mathf.Deg2Rad;
        }
    }
}
