using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FavourManager : MonoBehaviour {

    public Favour favourPrefab;
    public Slot slotPrefab;
    public List<Favour> allFavours;
    
    [SerializeField]
    GameObject favourMenu;
    [SerializeField]
    GameObject freeFavoursRow;
    [SerializeField]
    GameObject sacrificeMenu;

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
        sacrificeGrid = sacrificeMenu.GetComponentInChildren<GridLayoutGroup>();
	}
	
	void Update() {
		if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleMenu();
        } else if (visible && Input.GetButtonDown("Cancel")) {
            ToggleMenu(false);
        }
	}

    void ToggleMenu() {
        visible ^= true;
        ToggleMenu(visible);
    }

    void ToggleMenu(bool value) {
        visible = value;
        GameState.Pause(value);
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        favourMenu.SetActive(value);

        if (sacrificeMenu.activeSelf) {
            for (int i = 0; i < sacrificeSlots.Length; i++) {
                if (sacrificeSlots[i].item) {
                    PutInFreeRow(sacrificeSlots[i].item.GetComponent<Favour>());
                }
            }
        }
        sacrificeMenu.SetActive(false);
    }

    #region Sacrifice Menu

    Slot[] sacrificeSlots = new Slot[0];
    GridLayoutGroup sacrificeGrid;
    PillarEntrance currentPillarEntrance;

    public void DisplaySacrificeMenu(PillarEntrance pillarEntrance) {
        ToggleMenu(true);
        sacrificeMenu.SetActive(true);
        sacrificeSlots = new Slot[pillarEntrance.favoursToSacrify];
        for (int i = 0; i < pillarEntrance.favoursToSacrify; i++) {
            sacrificeSlots[i] = CreateSacrificeSlot();
        }
        currentPillarEntrance = pillarEntrance;
    }
    
    public void SacrificeFavours() {
        if (AllSacrificeSlotsAreFull()) {
            for (int i = 0; i < sacrificeSlots.Length; i++) {
                sacrificeSlots[i].item.GetComponent<Favour>().Sacrifice();
            }
        }
        currentPillarEntrance.OpenDoor();
    }

    bool AllSacrificeSlotsAreFull() {
        for (int i = 0; i < sacrificeSlots.Length; i++) {
            if (!sacrificeSlots[i].item)
                return false;
        }
        return true;
    }
    
    Slot CreateSacrificeSlot() {
        return Instantiate(slotPrefab, sacrificeGrid.transform);
    }

    #endregion

    public void PutInFreeRow(Favour favour) {
        Slot newSlot = Instantiate(slotPrefab, freeFavoursRow.transform);
        favour.transform.SetParent(newSlot.transform);
        favour.transform.localPosition = Vector2.zero;
    }

    public Favour CreateFavour() {
        Favour newFavour = PrefabUtility.InstantiatePrefab(favourPrefab) as Favour;
        newFavour.transform.SetParent(favourMenu.transform);
        newFavour.transform.localScale = Vector3.one;
        return newFavour;
    }
}
