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

    void Start() {
        obj = gameObject;
        manager = FavourManager.instance;
        if (!manager.favours.Contains(this))
            manager.favours.Add(this);

        if (Hidden)
            obj.SetActive(false);
    }

    public void Unlock() {
        state = FavourState.free;
        obj.SetActive(true);
    }

    public void Sacrifice() {
        state = FavourState.sacrificed;
        obj.SetActive(false);
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
