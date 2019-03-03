/**
 * Description: Custom inspector for a helper class (class for calling screen shake from the inspector).
 * Authors: Michał Wildanger, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
**/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CallScreenShake))]
public class CallScreenShakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CallScreenShake myTarget = (CallScreenShake)target;
        myTarget.Edition = (CallScreenShake.EditionType)EditorGUILayout.EnumPopup(myTarget.Edition);

        myTarget.Duration = EditorGUILayout.FloatField("Duration of shaking: ", myTarget.Duration);

        if (myTarget.Edition == CallScreenShake.EditionType.Advanced)
        {
            myTarget.ChosedShakeType = (ScreenShake.ShakeType)EditorGUILayout.EnumPopup(myTarget.ChosedShakeType);

            if (myTarget.ChosedShakeType == ScreenShake.ShakeType.TwoD)
            {
                myTarget.Move = EditorGUILayout.Vector3Field("Strength of move: ", myTarget.Move);
                myTarget.Rotation = new Vector3(0, 0, EditorGUILayout.FloatField("Strength of rotation in Z axis: ", myTarget.Rotation.z));
            }
            else if (myTarget.ChosedShakeType == ScreenShake.ShakeType.ThreeD)
            {
                myTarget.Rotation = EditorGUILayout.Vector3Field("Strength of rotation: ", myTarget.Rotation);
            }
            else
            {
                myTarget.Move = EditorGUILayout.Vector3Field("Strength of move: ", myTarget.Move);
                myTarget.Rotation = EditorGUILayout.Vector3Field("Strength of rotation: ", myTarget.Rotation);
            }

            myTarget.RecoilDirection = EditorGUILayout.Vector2Field("Direction of the recoil: ", new Vector2(Mathf.Clamp(myTarget.RecoilDirection.x, -1, 1), Mathf.Clamp(myTarget.RecoilDirection.y, -1, 1)));
            myTarget.RecoilStrength = EditorGUILayout.FloatField("Strength of the recoil: ", myTarget.RecoilStrength);
        }
        else
        {
            myTarget.GeneralStrength = EditorGUILayout.FloatField("Strength of Shaking: ", myTarget.GeneralStrength);
            myTarget.BasicSettings(myTarget.GeneralStrength);
        }
    }
}
