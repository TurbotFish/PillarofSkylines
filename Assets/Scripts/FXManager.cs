using UnityEngine;

public class FXManager : MonoBehaviour {

    public ParticleSystem doubleJumpFX,
                          dashFX;

    #region Singleton
    public static FXManager instance;
    void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
            Destroy(gameObject);
    }
    #endregion
}
