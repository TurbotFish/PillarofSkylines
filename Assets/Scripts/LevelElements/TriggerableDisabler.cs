using UnityEngine;

public class TriggerableDisabler : TriggerableObject
{
    [Header("Disabler")]
    [SerializeField] bool disabledByDefault;

    [SerializeField] GameObject[] objects;

    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        foreach (GameObject go in objects)
            go.SetActive(!disabledByDefault);
    }

    protected override void Activate()
    {
        foreach (GameObject go in objects)
            go.SetActive(disabledByDefault);
    }

    protected override void Deactivate()
    {
        foreach (GameObject go in objects)
            go.SetActive(!disabledByDefault);
    }
}
