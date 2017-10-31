using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;
    CanvasGroup canvasGroup;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    #region IBeginDragHandler implementation

    public void OnBeginDrag(PointerEventData eventData) {
        itemBeingDragged = gameObject;
        startPosition = transform.localPosition;
        startParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root);
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag(PointerEventData eventData) {
        itemBeingDragged = null;
        canvasGroup.blocksRaycasts = true;
        if (transform.parent == transform.root) {
            transform.SetParent(startParent);
            transform.localPosition = startPosition;
        }
    }

    #endregion
}