using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFix : MonoBehaviour {

    public Vector3 charPosition;
	
	public void ResetCamera (Transform _camera) {


        _camera.DOGoto(100);
        Debug.Log("good");
		
	}

    public void ResetCharacter (Transform _char)
    {
        _char.position = charPosition;
    }
}
