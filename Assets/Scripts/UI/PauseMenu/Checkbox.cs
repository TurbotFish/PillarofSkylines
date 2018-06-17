using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.PauseMenu
{
    public class Checkbox : MonoBehaviour
    {
        [SerializeField] Sprite activeImg, inactiveImg;

        Image img;

        private void Start()
        {
            img = GetComponent<Image>();
        }

        public void SetState(bool state)
        {
            img.sprite = state ? activeImg : inactiveImg;
        }

    }
}

