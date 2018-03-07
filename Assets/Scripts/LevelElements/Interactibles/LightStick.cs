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
    float lightIntensity = 0;

    private void Start()
    {
        tag = "Interactible";
    }
    
    public override void EnterTrigger()
    {
        cube.sharedMaterial = litMaterial;
        sparkParticle.SetActive(true);
        StartCoroutine(LightUp());
    }

    IEnumerator LightUp()
    {
        for(float elapsed = 0; elapsed < timeToLightUp; elapsed+=Time.deltaTime)
        {
            pointLight.intensity = elapsed * maxLightIntensity;
            yield return null;
        }
    }

    public override void ExitTrigger()
    {
        return;
    }
}
