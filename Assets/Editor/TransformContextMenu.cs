﻿using UnityEngine;
using UnityEditor;

/// <summary>
/// Extends the Transform context menu to provide support for copying, pasting
/// and pushing position/rotation/scale data around, on either local or world scale.
///
/// Original Code by Zach Aikman - zachaikman@gmail.com
/// Local extentions by Conor MacKeigan - conormackeigan@gmail.com
/// </summary>
public class TransformContextMenu {
    private class TransformClipboard {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public bool isPositionSet = false;
        public bool isRotationSet = false;
        public bool isScaleSet = false;
    }

    private static TransformClipboard clipboard = new TransformClipboard();

    #region Copy Methods
    [MenuItem("CONTEXT/Transform/Copy Local Transform", false, 150)]
    static void CopyLocalTransform() {
        CopyLocalPosition();
        CopyLocalRotation();
        CopyLocalScale();
    }

    [MenuItem("CONTEXT/Transform/Copy Local Position", false, 151)]
    static void CopyLocalPosition() {
        clipboard.position = Selection.activeTransform.localPosition;
        clipboard.isPositionSet = true;
    }

    [MenuItem("CONTEXT/Transform/Copy Local Rotation", false, 152)]
    static void CopyLocalRotation() {
        clipboard.rotation = Selection.activeTransform.localRotation;
        clipboard.isRotationSet = true;
    }

    [MenuItem("CONTEXT/Transform/Copy Local Scale", false, 153)]
    static void CopyLocalScale() {
        clipboard.scale = Selection.activeTransform.localScale;
        clipboard.isScaleSet = true;
    }

    [MenuItem("CONTEXT/Transform/Copy World Transform", false, 154)]
    static void CopyWorldTransform() {
        CopyWorldPosition();
        CopyWorldRotation();
        CopyWorldScale();
    }

    [MenuItem("CONTEXT/Transform/Copy World Position", false, 155)]
    static void CopyWorldPosition() {
        clipboard.position = Selection.activeTransform.position;
        clipboard.isPositionSet = true;
    }

    [MenuItem("CONTEXT/Transform/Copy World Rotation", false, 156)]
    static void CopyWorldRotation() {
        clipboard.rotation = Selection.activeTransform.rotation;
        clipboard.isRotationSet = true;
    }

    [MenuItem("CONTEXT/Transform/Copy World Scale", false, 157)]
    static void CopyWorldScale() {
        clipboard.scale = Selection.activeTransform.lossyScale;
        clipboard.isScaleSet = true;
    }
    #endregion

    #region Paste Methods
    [MenuItem("CONTEXT/Transform/Paste Transform", false, 200)]
    static void PasteTransform() {
        PastePosition();
        PasteRotation();
        PasteScale();
    }

    [MenuItem("CONTEXT/Transform/Paste Position", false, 201)]
    static void PastePosition() {
        Undo.RegisterCompleteObjectUndo(Selection.activeTransform, "Paste Position");
        Selection.activeTransform.localPosition = clipboard.position;
    }

    [MenuItem("CONTEXT/Transform/Paste Rotation", false, 202)]
    static void PasteRotation() {
        Undo.RegisterCompleteObjectUndo(Selection.activeTransform, "Paste Rotation");
        Selection.activeTransform.localRotation = clipboard.rotation;
    }

    [MenuItem("CONTEXT/Transform/Paste Scale", false, 203)]
    static void PasteScale() {
        Undo.RegisterCompleteObjectUndo(Selection.activeTransform, "Paste Scale");
        Selection.activeTransform.localScale = clipboard.scale;
    }
    #endregion

    #region Validation
    [MenuItem("CONTEXT/Transform/Paste Transform", true)]
    static bool ValidatePasteTransform() {
        return ValidatePastePosition() && ValidatePasteRotation() && ValidatePasteScale();
    }

    [MenuItem("CONTEXT/Transform/Paste Position", true)]
    static bool ValidatePastePosition() {
        return clipboard.isPositionSet;
    }

    [MenuItem("CONTEXT/Transform/Paste Rotation", true)]
    static bool ValidatePasteRotation() {
        return clipboard.isRotationSet;
    }

    [MenuItem("CONTEXT/Transform/Paste Scale", true)]
    static bool ValidatePasteScale() {
        return clipboard.isScaleSet;
    }

    [MenuItem("CONTEXT/Transform/Push To Children", true)]
    static bool ValidatePushToChildren() {
        return Selection.activeTransform.childCount > 0;
    }

    [MenuItem("CONTEXT/Transform/Push To Parent", true)]
    static bool ValidatePushToParent() {
        return Selection.activeTransform.parent != null;
    }
    #endregion
}