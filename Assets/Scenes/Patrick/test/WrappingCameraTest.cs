using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingCameraTest : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 lookDirection;
    [SerializeField] private Vector3 worldSize;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera myCamera;

    [SerializeField] private Shader wrappingShader;
    [SerializeField] private Renderer wrappingRenderer;    

    [SerializeField] private Material wrappingMaterial;
    private RenderTexture renderTexture;

    // Use this for initialization
    void Start()
    {
        Move();

        renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        renderTexture.Create();
        myCamera.forceIntoRenderTexture = true;
        myCamera.targetTexture = renderTexture;

        //wrappingMaterial = new Material(wrappingShader);
        wrappingMaterial.mainTexture = renderTexture;
        wrappingRenderer.sharedMaterial = wrappingMaterial;        
    }

    // Update is called once per frame
    void Update()
    {
        if (!wrappingRenderer.isVisible)
        {
            myCamera.enabled = false;
        }
        else
        {
            myCamera.enabled = true;
        }

        Move();

        wrappingMaterial.mainTexture = renderTexture;
    }

    private void Move()
    {
        Vector3 newPos = mainCamera.transform.position + Vector3.Scale(-lookDirection, worldSize);
        transform.position = newPos;

        myCamera.fieldOfView = mainCamera.fieldOfView;

        //transform.rotation = mainCamera.transform.rotation;
    }
}
