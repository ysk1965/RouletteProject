using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
using Object = UnityEngine.Object;

[CustomEditor(typeof(UEButton), true)]
[CanEditMultipleObjects]
public class UEButtonEditor : ButtonEditor
{
    private SerializedProperty _onPressProperty;
    private SerializedProperty _onLongPressProperty;
    private SerializedProperty _onPressEndProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        _onPressProperty = serializedObject.FindProperty("_onPress");
        _onLongPressProperty = serializedObject.FindProperty("_onLongPress");
        _onPressEndProperty = serializedObject.FindProperty("_onPressEnd");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UEButton button = (UEButton)target;

        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(_onPressProperty);

        UEReactionType oldType = button.ReactionType;
        UEReactionType newType = (UEReactionType)EditorGUILayout.EnumPopup("Reaction Type", button.ReactionType);

        if (newType != oldType)
        {
            foreach (Object obj in targets)
            {
                button = (UEButton)obj;
                button.ReactionType = newType;
                EditorUtility.SetDirty(obj);
            }
        }

        UEButtonSoundType oldSoundType = button.SoundType;
        UEButtonSoundType newSoundType = (UEButtonSoundType)EditorGUILayout.EnumPopup("Click Sound", button.SoundType);

        if (newSoundType != oldSoundType)
        {
            foreach (Object obj in targets)
            {
                button = (UEButton)obj;
                button.SoundType = newSoundType;
                EditorUtility.SetDirty(obj);
            }
        }

        bool curValue = button.UseContinueClick;
        bool newValue = EditorGUILayout.Toggle("Use Continue Click", curValue);

        if (curValue != newValue)
        {
            foreach (Object obj in targets)
            {
                button = (UEButton)obj;
                button.UseContinueClick = newValue;
                EditorUtility.SetDirty(obj);
            }
        }
        
        if (newValue)
        {
            EditorGUILayout.PropertyField(_onPressEndProperty);
        }
        

        bool curUseLongClick = button.UseLongClick;
        bool newUseLongClick = EditorGUILayout.Toggle("Use Long Click", curUseLongClick);

        if (curUseLongClick != newUseLongClick)
        {
            foreach (Object obj in targets)
            {
                button = (UEButton)obj;
                button.UseLongClick = newUseLongClick;
                EditorUtility.SetDirty(obj);
            }
        }
        
        if (newUseLongClick)
        {
            float curLongTime = button.LongClickTime;
            float newLongTime = EditorGUILayout.FloatField("Long Click Time", curLongTime);
        
            if (Math.Abs(curLongTime - newLongTime) > 0.01f)
            {
                foreach (Object obj in targets)
                {
                    button = (UEButton)obj;
                    button.LongClickTime = newLongTime;
                    EditorUtility.SetDirty(obj);
                }
            }
            EditorGUILayout.PropertyField(_onLongPressProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}