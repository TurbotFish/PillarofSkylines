﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneNames : MonoBehaviour
{
    [SerializeField]
    string openWorldSceneName;
    public string OpenWorldSceneName { get { return this.openWorldSceneName; } }

    [SerializeField]
    string uiSceneName;
    public string UiSceneName { get { return this.uiSceneName; } }
}
