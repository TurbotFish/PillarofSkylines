using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    //###########################################################

    // -- CONSTANTS

    [SerializeField] private float maxVelocity;
    [SerializeField] private bool autoOnOff;
    [SerializeField] private List<ParticleSystem> particles;
    [SerializeField] private List<Vector3> thresholds;

    //###########################################################

    // -- ATTRIBUTES

    private Vector3 Velocity;
    private bool IsActive;

    //###########################################################

    // -- INITIALIZATION

    //###########################################################

    // -- OPERATIONS

    public void Play()
    {
        if (IsActive)
        {
            return;
        }

        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }

        IsActive = true;
    }

    public void Stop()
    {
        if (!IsActive)
        {
            return;
        }

        foreach (ParticleSystem ps in particles)
        {
            ps.Stop();
        }

        IsActive = false;
    }

    public void SetVelocity(Vector3 v)
    {
        Velocity = v;
    }

    private void Update()
    {
        if (!IsActive)
        {
            return;
        }

        float velocity_magnitude = Velocity.magnitude;
        float velocity_ratio = velocity_magnitude / maxVelocity;

        if (autoOnOff)
        {
            for (int i = 0; i < thresholds.Count; i++)
            {
                if (i < particles.Count)
                {
                    if (velocity_ratio > thresholds[i].x)
                    {
                        if (!particles[i].isPlaying)
                        {
                            particles[i].Play();
                            //Debug.Log ("play : " + i);
                        }
                    }
                    else if (particles[i].isPlaying)
                    {
                        particles[i].Stop();
                        //Debug.Log ("stop : " + i);
                    }
                }
            }
        }

        for (int i = 0; i < thresholds.Count; i++)
        {
            if (i < particles.Count)
            {
                //var emissionModule = particles [i].emission;
                //emissionModule.rateOverTime = Mathf.RoundToInt(Mathf.SmoothStep (thresholds [i].y, thresholds [i].z, _ratio));
                var velocityModule = particles[i].main;
                velocityModule.simulationSpeed = Mathf.SmoothStep(thresholds[i].y, thresholds[i].z, velocity_ratio);
            }
        }
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + Velocity.normalized * 10);
    }
}
