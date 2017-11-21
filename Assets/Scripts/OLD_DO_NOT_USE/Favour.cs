//using UnityEngine;

//public class Favour : DragHandler {

//    public FavourState state;
//    /// <summary>
//    /// The name of the creature who granted this favour.
//    /// </summary>
//    public string mora;

//    public bool Hidden {
//        get { return state == FavourState.locked || state == FavourState.sacrificed; }
//    }

//    Transform my;
//    GameObject obj;
//    FavourManager manager;
//    FavourSlot currentSlot;
//    bool dragged;

//    void Start() {
//        my = transform;
//        obj = gameObject;
//        manager = FavourManager.instance;

//        if (!manager.allFavours.Contains(this))
//            manager.allFavours.Add(this);

//        if (state == FavourState.free)
//            Unlock();
//        else if (Hidden)
//            obj.SetActive(false);
//    }

//    void Update() {
//        if (!dragged) return;

//        my.position = Input.mousePosition;
//    }

//    void OnTriggerEnter2D(Collider2D col) {
//        print("trigger enter");
//        FavourSlot slot = col.GetComponent<FavourSlot>();
//        if (slot) {
//            currentSlot = slot;
//        }
//    }

//    void OnTriggerExit2D(Collider2D col) {
//        FavourSlot slot = GetComponent<FavourSlot>();
//        if (slot == currentSlot) {
//            currentSlot = null;
//        }
//    }

//    public void Unlock() {
//        state = FavourState.free;
//        obj.SetActive(true);
//        PutInFreeRow();
//    }

//    public void Sacrifice() {
//        state = FavourState.sacrificed;
//        obj.SetActive(false);
//    }

//    public void Pick() {
//        dragged = true;
//    }

//    public void Drop() {
//        dragged = false;
//        if (currentSlot) {
//            my.position = currentSlot.transform.position;
//        }
//    }

//    public void PutInFreeRow() {
//        manager.PutInFreeRow(this);
//    }
//}

//[System.Serializable]
//public enum FavourState {
//    locked,
//    free,
//    used,
//    sacrificed
//}
