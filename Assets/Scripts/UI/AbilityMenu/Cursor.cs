using UnityEngine;

namespace Game.UI.AbilityMenu
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        Vector3 targetPosition;

        public void GoTo(Vector3 position)
        {
            targetPosition = position;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        }

    }
}