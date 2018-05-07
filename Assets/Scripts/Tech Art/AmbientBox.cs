using Game.LevelElements;
using System.Collections;
using UnityEngine;

public class AmbientBox : MonoBehaviour, IInteractable
{
    //##################################################################

    [SerializeField] Color color;
    [SerializeField] float fadeSpeed = 2;

    Color defaultColor;

    //##################################################################

    #region initialization

    private void Start()
    {
        defaultColor = RenderSettings.ambientLight;
    }

    #endregion initialization

    //##################################################################

    #region inquiries

    public Vector3 Position { get { return transform.position; } }

    public bool IsInteractable()
    {
        return false;
    }

    #endregion inquiries

    //################################################################## 

    #region operations

    public void OnPlayerEnter()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAmbient(color));
    }

    public void OnPlayerExit()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAmbient(defaultColor));
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

    private IEnumerator FadeAmbient(Color goal)
    {
        while (RenderSettings.ambientLight != goal)
        {
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, goal, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }

    #endregion operations

    //##################################################################
}
