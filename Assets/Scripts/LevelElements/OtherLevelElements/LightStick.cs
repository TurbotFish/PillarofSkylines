using Game.LevelElements;
using System.Collections;
using UnityEngine;

public class LightStick : MonoBehaviour, IInteractable
{
    //##################################################################

    [SerializeField] Renderer cube;
    [SerializeField] Material litMaterial;
    [SerializeField] GameObject sparkParticle;
    [SerializeField] Light pointLight;
    [SerializeField] float maxLightIntensity = 4;
    [SerializeField] float timeToLightUp = 1;

    bool lit;

    //##################################################################

    #region initialization

    private void Start()
    {
        tag = "Interactible";
    }

    #endregion initialization

    //##################################################################

    #region inquiries

    public Transform Transform { get { return transform; } }

    public bool IsInteractable()
    {
        return false;
    }

    #endregion inquiries

    //##################################################################

    #region operations

    public void OnPlayerEnter()
    {
        if (!lit)
        {
            lit = true;
            cube.sharedMaterial = litMaterial;
            sparkParticle.SetActive(true);
            StartCoroutine(LightUp());
        }
    }

    public void OnPlayerExit()
    {
    }   

    public void OnHoverBegin()
    {
    }

    public void OnHoverEnd()
    {
    }

    public void OnInteraction()
    {
    }

    private IEnumerator LightUp()
    {
        for (float elapsed = 0; elapsed < timeToLightUp; elapsed += Time.deltaTime)
        {
            pointLight.intensity = elapsed * maxLightIntensity;
            yield return null;
        }
    }

    #endregion operations

    //##################################################################
}
