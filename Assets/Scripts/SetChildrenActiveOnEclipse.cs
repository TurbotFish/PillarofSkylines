﻿using UnityEngine;

public class SetChildrenActiveOnEclipse : MonoBehaviour {

    [SerializeField] bool setActive;
    GameObject[] children;

    private void OnEnable() {
        children = new GameObject[transform.childCount];
        int j = 0;
        foreach(Transform child in transform) {
            children[j] = child.gameObject;
            child.gameObject.SetActive(!setActive);
            j++;
        }
        
        Game.Utilities.EventManager.EclipseEvent += OnEclipseEventHandler;
    }

    private void OnDisable() {
        Game.Utilities.EventManager.EclipseEvent -= OnEclipseEventHandler;
    }

    void OnEclipseEventHandler(object sender, Game.Utilities.EventManager.EclipseEventArgs args) {

        for (int i = 0; i < children.Length; i++)
            children[i].SetActive(setActive == args.EclipseOn);
    }
}