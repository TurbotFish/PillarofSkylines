using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDebug : MonoBehaviour
{
    bool active;
    float _x, _y, _z;
    string userX = "-";
    string userY = "-";
    string userZ = "-";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            active = !active;
        }
        else if (Input.GetKeyDown(KeyCode.Return) && active)
        {
            float testX, testY, testZ;

            if (float.TryParse(userX, out testX) && float.TryParse(userY, out testY) && float.TryParse(userZ, out testZ))
            {
                _x = testX;
                _y = testY;
                _z = testZ;
                var eventArgs = new Game.Utilities.EventManager.TeleportPlayerEventArgs(Vector3.zero, new Vector3(_x, _y, _z));
                Game.Utilities.EventManager.SendTeleportPlayerEvent(this, eventArgs);
            }
            else
            {
                Debug.LogWarning("Enter a correct position plz D:");
            }

            active = false;
        }
        else if (Input.GetKeyDown(KeyCode.F4) && active)
        {
            active = false;
        }
    }

    void OnGUI()
    {
        if (active)
        {
            GUI.SetNextControlName("UserX");
            userX = GUI.TextField(new Rect(10, 10, 60, 20), userX, 99);
            GUI.SetNextControlName("UserY");
            userY = GUI.TextField(new Rect(110, 10, 60, 20), userY, 99);
            GUI.SetNextControlName("UserZ");
            userZ = GUI.TextField(new Rect(210, 10, 60, 20), userZ, 99);
            if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
                GUIUtility.keyboardControl = 0;
        }
    }
}
