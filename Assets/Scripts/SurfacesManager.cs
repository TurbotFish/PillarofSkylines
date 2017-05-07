using UnityEngine;

public class SurfacesManager : MonoBehaviour {

    [SerializeField]
    Transform westSurface;
    [SerializeField]
    Transform eastSurface;

    public float distance;
    
    void OnValidate() {

        Vector3 westPos = westSurface.localPosition;
        westPos.x = -distance;
        westSurface.localPosition = westPos;

        Vector3 eastPos = eastSurface.localPosition;
        eastPos.x = distance;
        eastSurface.localPosition = eastPos;
    }
    
}
