using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects, CustomEditor(typeof(GamePadButtonMng), true)]
public class ButtonExEditor : ButtonEditor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        this.serializedObject.Update();
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("DealButton"), true);
        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("PadPushEvent"), true);
        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("HoldOutEvent"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("HoldFinishEvent"), true);
        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Anime"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("ListRecive"), true);
        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("CheckMark"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsHold"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("HoldGauge"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("FinishHoldTime"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsNonActiveHide"), true);

        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("PadAxis"), true);
        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("JpGuid"), true);
        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("EnGuid"), true);
        this.serializedObject.ApplyModifiedProperties();
    }
}

//[CanEditMultipleObjects, CustomEditor(typeof(GamePadListInputtMng), true)]
//public class ButtonExEditorList : ButtonEditor
//{
//    public override void OnInspectorGUI() {
//        base.OnInspectorGUI();
//        this.serializedObject.Update();
//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("DealButton"), true); //一応
//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("CancelEvent"), true);
//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Buttons"), true);
//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsAuto"), true);
//        this.serializedObject.ApplyModifiedProperties();
//    }
//}

#endif

