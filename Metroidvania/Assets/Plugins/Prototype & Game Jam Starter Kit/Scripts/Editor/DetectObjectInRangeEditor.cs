/**
 * Description: Custom inspector GUI for DectectObjectInRange class.
 * Authors: Wojciech Bruski, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DetectObjectInRange))]
public class DetectObjectInRangeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIContent eventWithParams = new GUIContent("Object In Range");
        GUIContent eventWithoutParams = new GUIContent("Objects In Range");

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("detectionType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("invokeDelay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("anyTag"));

        if (serializedObject.FindProperty("detectionType").enumValueIndex == (int)DetectObjectInRange.DetectionType.Distance)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rangeOfDetection"));
        }

        if (serializedObject.FindProperty("invokeDelay").enumValueIndex == (int)DetectObjectInRange.InvokeDelay.Time)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("invokeDelayTime"));
        }

        if (serializedObject.FindProperty("anyTag").boolValue == false)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("detectionTags"), true);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Events - when object enter");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("onObjectsInRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectsInRangeWithParams"), new GUIContent("Objects In Range"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Events - when object exit");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("onObjectOutOfRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectOutOfRangeWithParams"), new GUIContent("Objects Out Of Range"));

        serializedObject.ApplyModifiedProperties();
    }
}
