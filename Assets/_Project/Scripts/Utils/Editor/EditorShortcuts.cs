using UnityEngine;
using UnityEditor;

public class EditorShortcuts
{
    [MenuItem("Window/Toggle Inspector Lock %q")] // Ctrl+Q
    public static void ToggleInspectorLock()
    {
        ActiveEditorTracker tracker = ActiveEditorTracker.sharedTracker;
        EditorWindow inspectorWindow = EditorWindow.mouseOverWindow;

        if (inspectorWindow && inspectorWindow.GetType().Name == "InspectorWindow")
        {
            tracker.isLocked = !tracker.isLocked;
            tracker.ForceRebuild();
            inspectorWindow.Repaint();
            Debug.Log($"Inspector Lock: {(tracker.isLocked ? "ON" : "OFF")}");
        }
    }

    [MenuItem("GameObject/Create Child Object %#d")] // Ctrl+Shift+D
    public static void CreateChildObject()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            GameObject newChild = new GameObject("New Child");
            newChild.transform.parent = selectedObject.transform;
            newChild.transform.localPosition = Vector3.zero;
            newChild.transform.localRotation = Quaternion.identity;
            newChild.transform.localScale = Vector3.one;
            Selection.activeGameObject = newChild;
            Undo.RegisterCreatedObjectUndo(newChild, "Create Child Object");
            Debug.Log($"Created child object under {selectedObject.name}");
        }
        else
        {
            Debug.LogWarning("No object selected in Hierarchy");
        }
    }

    [MenuItem("GameObject/Create Child Object %#d", true)]
    static bool ValidateCreateChildObject()
    {
        return Selection.activeGameObject != null;
    }
}
