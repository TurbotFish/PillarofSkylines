using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTest2 : MonoBehaviour
{
    [SerializeField] private Camera MainCamera;
    private Transform MainCameraTransform;

    private List<Camera> DuplicationCameraList;
    private List<Transform> DuplicationCameraTransformList;

    // Use this for initialization
    void Start()
    {
        MainCameraTransform = MainCamera.transform;
        transform.position = MainCameraTransform.position;

        DuplicationCameraList = GetComponentsInChildren<Camera>().ToList();
        DuplicationCameraTransformList = new List<Transform>();
        foreach(var camera in DuplicationCameraList)
        {
            DuplicationCameraTransformList.Add(camera.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = MainCameraTransform.position;

        foreach (var camera in DuplicationCameraList)
        {
            camera.fieldOfView = MainCamera.fieldOfView;
        }

        foreach (var camera_transform in DuplicationCameraTransformList)
        {
            camera_transform.rotation = MainCameraTransform.rotation;
        }        
    }
}
