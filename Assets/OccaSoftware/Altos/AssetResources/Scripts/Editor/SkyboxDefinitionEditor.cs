using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OccaSoftware.Altos.Runtime;

namespace OccaSoftware.Altos.Editor
{
    [CustomEditor(typeof(Runtime.SkyDefinition))]
    [CanEditMultipleObjects]
    public class SkyboxDefinitionEditor : UnityEditor.Editor
    {
        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

        SerializedProperty periodsOfDay;
        SerializedProperty initialTime;
        SerializedProperty dayNightCycleDuration;
        SerializedProperty environmentLightingExposure;
        SerializedProperty environmentLightingSaturation;

        SkyDefinition skyDefinition;

        private bool showTimeSettings = true;
        private bool showPeriodOfDayKeyframes = true;
        private bool showEnvironmentLighting = true;

        private void OnEnable()
        {
            skyDefinition = (SkyDefinition)serializedObject.targetObject;

            periodsOfDay = serializedObject.FindProperty("periodsOfDay");
            initialTime = serializedObject.FindProperty("initialTime");
            dayNightCycleDuration = serializedObject.FindProperty("dayNightCycleDuration");
            environmentLightingExposure = serializedObject.FindProperty("environmentLightingExposure");
            environmentLightingSaturation = serializedObject.FindProperty("environmentLightingSaturation");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            if (Event.current.type == EventType.MouseUp)
            {
                skyDefinition.SortPeriodsOfDay();
            }
            
            Draw();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeSettings()
		{
            // Time Settings
            showTimeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showTimeSettings, new GUIContent("Time Options"));
			EditorGUI.indentLevel++;
			EditorGUI.indentLevel++;
            GUIContent dayContent = new GUIContent("Day " + skyDefinition.CurrentDay, "Represents the current in-game day. Always initializes as 0. Not settable.");
			EditorGUILayout.LabelField(dayContent, EditorStyles.boldLabel);
            System.TimeSpan timeActive = System.TimeSpan.FromHours(skyDefinition.CurrentTime);
            GUIContent timeContent = new GUIContent("Time " + timeActive.ToString("hh':'mm':'ss"), "Represents the current in-game time for a particular day.");
            EditorGUILayout.LabelField(timeContent, EditorStyles.boldLabel);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(initialTime);
            EditorGUILayout.PropertyField(dayNightCycleDuration, new GUIContent("Day-Night Cycle Duration (h)", "The duration of each full day-night cycle (in hours). Set to 0 to disable the automatic progression of time."));
            EditorGUI.indentLevel--;
			EditorGUILayout.Space();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawKeyframeSettings()
		{
			showPeriodOfDayKeyframes = EditorGUILayout.BeginFoldoutHeaderGroup(showPeriodOfDayKeyframes, new GUIContent("Periods of Day Key Frames", "Periods of day are treated as keyframes. The sky will linearly interpolate between the current period's colorset and the next period's colorset."));
            EditorGUI.indentLevel++;
            for (int i = 0; i < periodsOfDay.arraySize; i++)
            {
                EditorGUILayout.Space(5f);
                SerializedProperty periodOfDay = periodsOfDay.GetArrayElementAtIndex(i);

                SerializedProperty description_Prop = periodOfDay.FindPropertyRelative("description");
                SerializedProperty startTime_Prop = periodOfDay.FindPropertyRelative("startTime");
                SerializedProperty horizonColor_Prop = periodOfDay.FindPropertyRelative("horizonColor");
                SerializedProperty zenithColor_Prop = periodOfDay.FindPropertyRelative("zenithColor");
                SerializedProperty groundColor_Prop = periodOfDay.FindPropertyRelative("groundColor");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(description_Prop, new GUIContent("Name", "(Optional) Descriptive name"));

                if (GUILayout.Button("-", EditorStyles.miniButtonRight, miniButtonWidth))
                {
                    periodsOfDay.DeleteArrayElementAtIndex(i);
                }
                else
                {
                    EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;

					EditorGUILayout.PropertyField(startTime_Prop, new GUIContent("Start Time", "Defines the start time for the values associated with this key frame. The previous key frame linearly interpolates to these values from the previous key frame."));
                    EditorGUILayout.PropertyField(zenithColor_Prop, new GUIContent("Sky Color"));
                    EditorGUILayout.PropertyField(horizonColor_Prop, new GUIContent("Equator Color"));
                    EditorGUILayout.PropertyField(groundColor_Prop, new GUIContent("Ground Color"));
                    EditorGUI.indentLevel--;
				}
            }
            EditorGUILayout.Space();
            Rect r = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
            if (GUI.Button(r, "+"))
            {
                periodsOfDay.arraySize += 1;
            }

            EditorGUI.indentLevel--;
			EditorGUILayout.Space();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawEnvironmentLightingSettings()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginFoldoutHeaderGroup(showEnvironmentLighting, new GUIContent("Environment Lighting", "Control how the sky color translates to environmental lighting properties."));
            EditorGUILayout.PropertyField(environmentLightingExposure, new GUIContent("Exposure"));
            EditorGUILayout.PropertyField(environmentLightingSaturation, new GUIContent("Saturation"));
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			EditorGUILayout.EndFoldoutHeaderGroup();
		}



		private void Draw()
        {
            DrawTimeSettings();
            DrawKeyframeSettings();
            DrawEnvironmentLightingSettings();
        }
    }

}
