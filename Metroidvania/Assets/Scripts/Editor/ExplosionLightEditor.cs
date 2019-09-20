using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ExplosionLight))]
public class ExplosionLightEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ExplosionLight myScript = (ExplosionLight)target;


        if ( GUILayout.Button( "Test" ) )
        {
            myScript.Start();
        }


        myScript.fadeIn = GUILayout.Toggle( myScript.fadeIn, "Fade In" );

        using ( var group = new EditorGUILayout.FadeGroupScope( Convert.ToSingle( myScript.fadeIn ) ) )
        {
            if ( group.visible )
            {
                EditorGUI.indentLevel++;

                myScript.fadeInTime = EditorGUILayout.FloatField( "Fade In Time", myScript.fadeInTime );

                myScript.fadeInOuterRadius = GUILayout.Toggle( myScript.fadeInOuterRadius, "Fade In Outer Radius" );

                using ( var group2 = new EditorGUILayout.FadeGroupScope( Convert.ToSingle( myScript.fadeInOuterRadius ) ) )
                {
                    if ( group2.visible )
                    {
                        EditorGUI.indentLevel++;

                        myScript.minInOuterRadius = EditorGUILayout.FloatField( "Min In Outer Radius", myScript.minInOuterRadius );

                        myScript.maxInOuterRadius = EditorGUILayout.FloatField( "Max In Outer Radius", myScript.maxInOuterRadius );

                        EditorGUI.indentLevel--;
                    }
                }

                myScript.fadeInIntensity = GUILayout.Toggle( myScript.fadeInIntensity, "Fade In Intensity" );

                using ( var group2 = new EditorGUILayout.FadeGroupScope( Convert.ToSingle( myScript.fadeInIntensity ) ) )
                {
                    if ( group2.visible )
                    {
                        EditorGUI.indentLevel++;

                        myScript.minInIntensity = EditorGUILayout.FloatField( "Min In Intensity", myScript.minInIntensity );

                        myScript.maxInIntensity = EditorGUILayout.FloatField( "Max In Intensity", myScript.maxInIntensity );

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }


        myScript.fadeOut = GUILayout.Toggle( myScript.fadeOut, "Fade Out" );

        using ( var group = new EditorGUILayout.FadeGroupScope( Convert.ToSingle( myScript.fadeOut ) ) )
        {
            if ( group.visible )
            {
                EditorGUI.indentLevel++;

                myScript.fadeOutTime = EditorGUILayout.FloatField( "Fade Out Time", myScript.fadeOutTime );

                myScript.fadeOutOuterRadius = GUILayout.Toggle( myScript.fadeOutOuterRadius, "Fade Out Outer Radius" );

                using ( var group2 = new EditorGUILayout.FadeGroupScope( Convert.ToSingle( myScript.fadeOutOuterRadius ) ) )
                {
                    if ( group2.visible )
                    {
                        EditorGUI.indentLevel++;

                        myScript.minOutOuterRadius = EditorGUILayout.FloatField( "Min Out Outer Radius", myScript.minOutOuterRadius );

                        myScript.maxOutOuterRadius = EditorGUILayout.FloatField( "Max Out Outer Radius", myScript.maxOutOuterRadius );

                        EditorGUI.indentLevel--;
                    }
                }

                myScript.fadeOutIntensity = GUILayout.Toggle( myScript.fadeOutIntensity, "Fade Out Intensity" );

                using ( var group2 = new EditorGUILayout.FadeGroupScope( Convert.ToSingle( myScript.fadeOutIntensity ) ) )
                {
                    if ( group2.visible )
                    {
                        EditorGUI.indentLevel++;

                        myScript.minOutIntensity = EditorGUILayout.FloatField( "Min Out Intensity", myScript.minOutIntensity );

                        myScript.maxOutIntensity = EditorGUILayout.FloatField( "Max Out Intensity", myScript.maxOutIntensity );

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        myScript.destroyOnEnd = GUILayout.Toggle( myScript.destroyOnEnd, "Destroy On End" );
    }
}
