using Game.LevelElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientFogController : MonoBehaviour, IInteractable
{
    //##################################################################

    static GradientFogController currentControl;
    Gradient defaultGradient;
    float defaultStart, defaultEnd;

    [SerializeField] float transitionTime = 0.8f;

    [Header("Values")]
    public Gradient gradient;
    public float startDistance = 100;
    public float endDistance = 200;

    [HideInInspector]
    public bool over;

    GradientFog fog;

    //##################################################################

    #region initialization

    private void Start()
    {
        fog = FindObjectOfType<GradientFog>(); // TODO: fix that
        defaultGradient = fog.gradient;
        defaultStart = fog.startDistance;
        defaultEnd = fog.endDistance;
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

    public void Stop()
    {
        StopAllCoroutines();
    }

    public void OnPlayerEnter()
    {
        if (currentControl && currentControl.over)
            currentControl.Stop();

        over = false;
        currentControl = this;
        fog.gradientLerp = 0;
        StartCoroutine(LerpGradientFog(fog.gradient, gradient, fog.startDistance, fog.endDistance, startDistance, endDistance));
    }

    public void OnPlayerExit()
    {
        if (!over)
            StopAllCoroutines();
        if (currentControl == this || currentControl == null)
        {
            fog.gradientLerp = 0;
            StartCoroutine(LerpGradientFog(gradient, defaultGradient, startDistance, endDistance, defaultStart, defaultEnd));
            over = true;
            currentControl = null;
        }
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

    private IEnumerator LerpGradientFog(Gradient From, Gradient To, float fromStart, float fromEnd, float toStart, float toEnd)
    {
        fog.secondaryGradient = To;
        fog.gradient = From;

        for (float elapsed = fog.gradientLerp; elapsed < transitionTime; elapsed += Time.deltaTime)
        {
            float t = elapsed / transitionTime;
            fog.gradientLerp = t;
            fog.startDistance = Mathf.Lerp(fromStart, toStart, t);
            fog.endDistance = Mathf.Lerp(fromEnd, toEnd, t);
            fog.GenerateTexture();
            yield return null;
        }
        fog.gradientLerp = 1;
    }

    #endregion operations

    //##################################################################
}
