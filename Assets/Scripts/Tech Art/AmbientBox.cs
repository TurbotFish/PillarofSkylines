using System.Collections;
using UnityEngine;

public class AmbientBox : Interactible {

    [SerializeField] Color color;
    [SerializeField] float fadeSpeed = 2;

    Color defaultColor;

    private void Start()
    {
        defaultColor = RenderSettings.ambientLight;
    }


    public override void EnterTrigger(Transform player)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAmbient(color));
    }

    public override void ExitTrigger(Transform player)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAmbient(defaultColor));
    }
    
    IEnumerator FadeAmbient(Color goal)
    {
        while(RenderSettings.ambientLight != goal)
        {
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, goal, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }

}
