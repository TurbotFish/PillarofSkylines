﻿using System.Collections;
using UnityEngine;

public class IlluminatingLamp : Interactible
{
    [SerializeField] public float damp = 0.2f;
    [SerializeField] Renderer cube;
    [SerializeField] Material litMaterial;
    [SerializeField] GameObject sparkParticle;
    [SerializeField] Light pointLight;
    [SerializeField] float maxLightIntensity = 4;
    [SerializeField] float timeToLightUp = 1;

    bool lit;


    bool here;
    Transform player;

    void Start()
    {
        tag = "Interactible";
    }

    public override void EnterTrigger(Transform player)
    {
        //here = true;
        //this.player = player;
        if (!lit)
        {
            lit = true;
            cube.sharedMaterial = litMaterial;
            sparkParticle.SetActive(true);
            StartCoroutine(LightUp());
        }

    }

    public override void ExitTrigger(Transform player)
    {
        return;
    }

    IEnumerator LightUp()
    {
        for (float elapsed = 0; elapsed < timeToLightUp; elapsed += Time.deltaTime)
        {
            pointLight.intensity = elapsed * maxLightIntensity;
            yield return null;
        }
    }


}