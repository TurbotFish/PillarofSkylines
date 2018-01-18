using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GuiFollowText : MonoBehaviour
{
    public static GuiFollowText Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    //########################################################

    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 offset = Vector3.up;

    bool clampToScreen = false;
    float clampBorderSize = 0.05f;

    [SerializeField]
    bool useMainCamera = true;

    [SerializeField]
    Camera cameraToUse;

    Camera cam;
    Transform myTransform;
    Transform camTransform;

    Text text;

    // Use this for initialization
    void Start()
    {
        myTransform = transform;
        text = GetComponent<Text>();

        if (useMainCamera)
        {
            cam = Camera.main;
        }
        else
        {
            cam = cameraToUse;
        }

        camTransform = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //myTransform.position = cam.WorldToViewportPoint(target.position + offset);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
