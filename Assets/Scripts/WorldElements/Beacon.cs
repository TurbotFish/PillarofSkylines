using UnityEngine;

public class Beacon : MonoBehaviour {

    public bool isHomeBeacon;
    public Beacon otherBeacon;

    public Transform teleportPoint;

    public bool activated;

    [Header("Rendering")]
    public Renderer socle;
    public Material matOn, matOff;

    [HideInInspector] public Transform destination;

    private void Awake()
    {
        if (otherBeacon != null)
            destination = otherBeacon.teleportPoint;
    }

    public void Activate()
    {
        activated = true;
        socle.sharedMaterial = matOn;

        if (!isHomeBeacon)
            otherBeacon.Activate();
    }

    private void OnValidate()
    {
        if (otherBeacon && !otherBeacon.otherBeacon)
            otherBeacon.otherBeacon = this;

    }

}
