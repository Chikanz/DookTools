#if UNITY_EDITOR

using dook.tools;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace dook.tool.shortcuts
{
    public static class DookShortcuts
    {
        private static Transform EditorCamera => SceneView.lastActiveSceneView.camera.transform;
        private const float MoveToCameraDistance = 3;

        [MenuItem("Tools/DookTools/Select None %q")]
        public static void SelectNone()
        {
            Selection.objects = new Object[0];
        }

        [MenuItem("Tools/DookTools/Group Selected %g")]
        private static void GroupSelected()
        {
            if (!Selection.activeTransform) return;
            var go = new GameObject(Selection.activeTransform.name + " Group");
            Undo.RegisterCreatedObjectUndo(go, "Group Selected");
            go.transform.SetParent(Selection.activeTransform.parent, false);

            //position parent in middle of group
            Vector3[] positions = new Vector3[Selection.transforms.Length];
            for (var i = 0; i < Selection.transforms.Length; i++)
            {
                positions[i] = Selection.transforms[i].position;
            }

            go.transform.position = DookTools.GetMeanVector(positions);

            //parent
            foreach (var transform in Selection.transforms)
            {
                Undo.SetTransformParent(transform, go.transform, "Group Selected");
            }

            Selection.activeGameObject = go;
        }

        [Shortcut("DookTools.MoveToCamera")]
        public static void MoveToCamera()
        {
            if (!EditorCamera) return;
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                Undo.RecordObject(Selection.transforms[i], "Move to camera");
                Selection.transforms[i].position = EditorCamera.position + EditorCamera.forward * MoveToCameraDistance;
            }
        }
    }
}

#endif