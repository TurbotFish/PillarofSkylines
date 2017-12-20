using UnityEngine;

public class AlwaysLookAt : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] float lookAtDamp = 10;

    void Update() {
        var rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtDamp);
    }

}
