//using UnityEngine;
//using UnityEngine.EventSystems;

//public class Slot : MonoBehaviour, IDropHandler {
//    public GameObject item {
//        get {
//            if (transform.childCount > 0) {
//                return transform.GetChild(0).gameObject;
//            }
//            return null;
//        }
//    }

//    #region IDropHandler implementation
//    public void OnDrop(PointerEventData eventData) {
//        if (!item) {
//            DragHandler.itemBeingDragged.transform.SetParent(transform);
//            DragHandler.itemBeingDragged.transform.localPosition = Vector2.zero;
//        }
//    }
//    #endregion
//}