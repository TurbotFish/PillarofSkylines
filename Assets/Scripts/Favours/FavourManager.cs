using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class FavourManager : MonoBehaviour {

    public Favour favourPrefab;
    public Slot slotPrefab;
    public List<Favour> allFavours;
	public GameObject F;

    [SerializeField]
    GameObject favourMenu;
    [SerializeField]
    GameObject freeFavoursRow;
    [SerializeField]
    GameObject sacrificeMenu;

    List<Slot> slotPool = new List<Slot>();
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
        freeFavoursRow.GetComponent<GridLayoutGroup>().cellSize = favourPrefab.GetComponent<RectTransform>().sizeDelta;

        sacrificeGrid = sacrificeMenu.GetComponentInChildren<GridLayoutGroup>();
        sacrificeMenu.SetActive(false);
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

	bool FwasHere;
    void ToggleMenu(bool value) {
		if (F.activeSelf) {
			FwasHere = true;
			F.SetActive (false);
		}
		if (FwasHere && !value) {
			F.SetActive (true);
		}
        visible = value;
        GameState.Pause(value);
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        favourMenu.SetActive(value);
        HideSacrificeMenu();

    }

    #region Sacrifice Menu

    Slot[] sacrificeSlots = new Slot[0];
    GridLayoutGroup sacrificeGrid;
    PillarEntrance currentPillarEntrance;

    public void DisplaySacrificeMenu(PillarEntrance pillarEntrance) {
        if (sacrificeMenu.activeSelf) return;

        ToggleMenu(true);
        sacrificeMenu.SetActive(true);
        sacrificeSlots = new Slot[pillarEntrance.favoursToSacrify];
        for (int i = 0; i < pillarEntrance.favoursToSacrify; i++) {
            sacrificeSlots[i] = CreateSacrificeSlot();
        }
        currentPillarEntrance = pillarEntrance;
    }
    
    void HideSacrificeMenu() {
        if (sacrificeMenu.activeSelf) {
            for (int i = 0; i < sacrificeSlots.Length; i++) {
                if (sacrificeSlots[i].item)
                    PutInFreeRow(sacrificeSlots[i].item.GetComponent<Favour>());
                sacrificeSlots[i].gameObject.SetActive(false);
            }
        }
        sacrificeMenu.SetActive(false);
    }

    public void SacrificeFavours() {
        if (AllSacrificeSlotsAreFull()) {
            for (int i = 0; i < sacrificeSlots.Length; i++) {
                sacrificeSlots[i].item.GetComponent<Favour>().Sacrifice();
            }
            ToggleMenu(false);
            currentPillarEntrance.OpenDoor();
        }
    }

    bool AllSacrificeSlotsAreFull() {
        for (int i = 0; i < sacrificeSlots.Length; i++) {
            if (!sacrificeSlots[i].item)
                return false;
        }
        return true;
    }
    
    Slot CreateSacrificeSlot() {
        return CreateSlotInParent(sacrificeGrid.transform);
    }

    #endregion

    Slot CreateSlotInParent(Transform parent) {
        
        foreach(Slot slot in slotPool) {
            if (!slot.gameObject.activeSelf) {
                slot.transform.SetParent(parent);
                slot.gameObject.SetActive(true);
                return slot;
            }
        }
        Slot newSlot = Instantiate(slotPrefab, parent);
        slotPool.Add(newSlot);
        return newSlot;
    }
    
    public void PutInFreeRow(Favour favour) {
        favour.transform.SetParent(freeFavoursRow.transform);
    }

    public Favour CreateFavour() {
		return favourPrefab;
       /* Favour newFavour = PrefabUtility.InstantiatePrefab(favourPrefab) as Favour;
        newFavour.transform.SetParent(favourMenu.transform);
        newFavour.transform.localScale = Vector3.one;
        return newFavour;*/
    }
}
