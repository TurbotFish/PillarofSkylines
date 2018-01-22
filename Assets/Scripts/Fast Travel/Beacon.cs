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
        destination = otherBeacon.teleportPoint;
    }

    public void Activate()
    {
        activated = true;
        socle.sharedMaterial = matOn;

        if (!isHomeBeacon)
            otherBeacon.Activate();
    }

}
