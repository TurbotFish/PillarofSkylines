using System.Collections;
using UnityEngine;

public class LightStick : Interactible
{
    [SerializeField] Renderer cube;
    [SerializeField] Material litMaterial;
    [SerializeField] GameObject sparkParticle;
    [SerializeField] Light pointLight;
    [SerializeField] float maxLightIntensity = 4;
    [SerializeField] float timeToLightUp = 1;

    bool lit;

    private void Start()
    {
        tag = "Interactible";
    }
    
    public override void EnterTrigger(Transform player)
    {
        if (!lit)
        {
            lit = true;
            cube.sharedMaterial = litMaterial;
            sparkParticle.SetActive(true);
            StartCoroutine(LightUp());
        }
    }

    IEnumerator LightUp()
    {
        for(float elapsed = 0; elapsed < timeToLightUp; elapsed+=Time.deltaTime)
        {
            pointLight.intensity = elapsed * maxLightIntensity;
            yield return null;
        }
    }

    public override void ExitTrigger(Transform player)
    {
        return;
    }
}
