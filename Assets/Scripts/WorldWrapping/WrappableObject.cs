using UnityEngine;

public class WrappableObject : MonoBehaviour {

    [SerializeField]
    Transform follower;
    Vector3 followOffset;

    WorldWrapper wrapper;
    Transform my;
    bool teleporting;

    void Start() {
        wrapper = FindObjectOfType<WorldWrapper>();
        my = transform;
    }

	void Update () {
        if (!wrapper) return;

        Vector3 pos = my.position;
        Vector3 wrapPos = wrapper.transform.position;
        Vector3 worldSize = wrapper.worldSize;
        if (follower)
            followOffset = follower.position - pos;
        teleporting = false;

        if (wrapper.repeatAxes.x) {
            if (pos.x > wrapPos.x + worldSize.x / 2) {
                pos.x -= worldSize.x;
                teleporting = true;
            } else if (pos.x < wrapPos.x - worldSize.x / 2) {
                pos.x += worldSize.x;
                teleporting = true;
            }
        }

        if (wrapper.repeatAxes.y) {
            if (pos.y > wrapPos.y + worldSize.y / 2) {
                pos.y -= worldSize.y;
                teleporting = true;
            } else if (pos.y < wrapPos.y - worldSize.y / 2) {
                pos.y += worldSize.y;
                teleporting = true;
            }
        }

        if (wrapper.repeatAxes.z) {
            if (pos.z > wrapPos.z + worldSize.z / 2) {
                pos.z -= worldSize.z;
                teleporting = true;
            } else if (pos.z < wrapPos.z - worldSize.z / 2) {
                pos.z += worldSize.z;
                teleporting = true;
            }
        }

        if (teleporting)
            SetPosition(pos);
	}

    public virtual void SetPosition(Vector3 pos) {
        my.position = pos;
        if (follower)
            follower.position = pos + followOffset;
    }
}
