using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FavourManager : MonoBehaviour {

    public Favour favourPrefab;
    public List<Favour> favours;

    [SerializeField]
    GameObject favourMenu;

    bool visible;

    #region Singleton
    public static FavourManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }
    #endregion


    void Start () {
        visible = false;
        favourMenu.SetActive(visible);
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab)) {
            visible ^= true;
            GameState.Pause(visible);
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            favourMenu.SetActive(visible);
        }
	}

    public Favour CreateFavour() {
        Favour newFavour = PrefabUtility.InstantiatePrefab(favourPrefab) as Favour;
        newFavour.transform.SetParent(favourMenu.transform);
        return newFavour;
    }
}
