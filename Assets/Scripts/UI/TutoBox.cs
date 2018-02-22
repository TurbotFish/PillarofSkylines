using UnityEngine;

namespace Game.UI
{
    public class TutoBox : MonoBehaviour
    {
        public eMessageType messageType;

        public string message = "EXAMPLE TUTORIAL MESSAGE";
        [TextArea, ConditionalHide("messageType", 1)]
        public string description = "Example Description.";
        [ConditionalHide("messageType", 2)]
        public float time = 2;
    }
}

