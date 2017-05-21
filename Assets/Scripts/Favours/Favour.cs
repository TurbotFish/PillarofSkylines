using UnityEngine;
using UnityEngine.UI;

public class Favour : MonoBehaviour {

    public FavourState state;
    /// <summary>
    /// The name of the creature who granted this favour.
    /// </summary>
    public string mora;

    public bool Hidden {
        get { return state == FavourState.locked || state == FavourState.sacrificed; }
    }

    GameObject obj;
    FavourManager manager;
    bool dragged;

    void Start() {
        obj = gameObject;
        manager = FavourManager.instance;
        if (state == FavourState.free)
            Unlock();
        else if (Hidden)
            obj.SetActive(false);
    }

    void Update() {
        if (!dragged) return;

        transform.position = Input.mousePosition;
    }

    public void Unlock() {
        state = FavourState.free;
        obj.SetActive(true);
        if (!manager.freeFavours.Contains(this))
            manager.freeFavours.Add(this);
    }

    public void Sacrifice() {
        state = FavourState.sacrificed;
        obj.SetActive(false);
    }

    public void Pick() {
        dragged = true;
    }

    public void Drop() {
        dragged = false;
    }

    #region Editor
    new RectTransform transform;
    new BoxCollider2D collider;
    void OnValidate() {
        if (!transform)
            transform = GetComponent<RectTransform>();
        if (!collider)
            collider = GetComponent<BoxCollider2D>();

        collider.size = transform.sizeDelta;
    }
    #endregion
}

[System.Serializable]
public enum FavourState {
    locked,
    free,
    used,
    sacrificed
}
