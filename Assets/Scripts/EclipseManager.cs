﻿using UnityEngine;
using System.Collections;

public class EclipseManager : MonoBehaviour {

    [TestButton("Start Eclipse", "StartEclipse", isActiveInEditor = false)]
    [TestButton("Stop Eclipse", "StopEclipse", isActiveInEditor = false)]
    public bool isEclipseActive;

    public float rotationDuration = 1;

    [SerializeField]
    Transform pillar;

    EchoManager echoes;
    Transform player;
    Needle needle;

    #region Singleton
    public static EclipseManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    void Start () {
        echoes = EchoManager.instance;
        needle = FindObjectOfType<Needle>();
        needle.transform.parent = pillar;
        player = FindObjectOfType<ThirdPersonController>().transform;
	}
	
    public void StartEclipse() {
        Time.timeScale = 0;
        PlaceAllObjectsInPillar();
        echoes.FreezeAll();
        isEclipseActive = true;
        RotatePillar(player.position, Vector3.forward, -90);
        Time.timeScale = 1;
    }

    public void StopEclipse() {
        Time.timeScale = 0;
        PlaceAllObjectsInPillar();
        echoes.UnfreezeAll();
        isEclipseActive = false;
        needle.gameObject.SetActive(true);
        RotatePillar(player.position, Vector3.forward, 90);
        Time.timeScale = 1;
    }

    void PlaceAllObjectsInPillar() {
        pillar.DetachChildren();
        pillar.position = player.position;

        GameObject[] allRootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject go in allRootObjects) {
            if (go.GetComponent<Camera>()) continue; //not the camera nor the player
            go.transform.parent = pillar;
        }
    }
    
    void RotatePillar(Vector3 pivot, Vector3 axis, float angle) {
        StartCoroutine(_RotatePillar(pivot, axis, angle));
    }

    IEnumerator _RotatePillar(Vector3 pivot, Vector3 axis, float angle) {

        float trueAngle = angle / rotationDuration;
        for (float elapsed = 0; elapsed < rotationDuration; elapsed += Time.unscaledDeltaTime) {

            pillar.Rotate(axis * trueAngle * Time.unscaledDeltaTime);

            //pillar.RotateAround(pivot, axis, trueAngle * Time.unscaledDeltaTime);
            yield return new WaitForEndOfFrame();
        }
        float zRot = pillar.rotation.eulerAngles.z;

        zRot /= 10;
        zRot = Mathf.Round(zRot);
        zRot *= 10;

        pillar.eulerAngles = new Vector3(0, 0, zRot);
    }

}
