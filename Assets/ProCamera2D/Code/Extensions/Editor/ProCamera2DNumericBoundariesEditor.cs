using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DNumericBoundaries))]
    public class ProCamera2DNumericBoundariesEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            ProCamera2DEditorHelper.AssignProCamera2D(target as BasePC2D);

            _script = MonoScript.FromMonoBehaviour((ProCamera2DNumericBoundaries)target);
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DNumericBoundaries = (ProCamera2DNumericBoundaries)target;
            if (proCamera2DNumericBoundaries.ProCamera2D == null)
            {
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
                return;
            }

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ProCamera2D"), _tooltip);

            _tooltip = new GUIContent("Use Numeric Boundaries", "Should the camera position be constrained by position?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseNumericBoundaries"), _tooltip);

            if (proCamera2DNumericBoundaries.UseNumericBoundaries)
            {
                EditorGUI.indentLevel = 1;
                            
                EditorGUILayout.BeginHorizontal();
                _tooltip = new GUIContent("Use Top", "Prevent camera movement beyond this point");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTopBoundary"), _tooltip);
            
                if (proCamera2DNumericBoundaries.UseTopBoundary)
                {
                    _tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TopBoundary"), _tooltip);
                }
            
                if (proCamera2DNumericBoundaries.UseBottomBoundary && proCamera2DNumericBoundaries.TopBoundary < proCamera2DNumericBoundaries.BottomBoundary)
                    proCamera2DNumericBoundaries.TopBoundary = proCamera2DNumericBoundaries.BottomBoundary;
            
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                _tooltip = new GUIContent("Use Bottom", "Prevent camera movement beyond this point");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseBottomBoundary"), _tooltip);
            
                if (proCamera2DNumericBoundaries.UseBottomBoundary)
                {
                    _tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BottomBoundary"), _tooltip);
                }
            
                if (proCamera2DNumericBoundaries.UseTopBoundary && proCamera2DNumericBoundaries.BottomBoundary > proCamera2DNumericBoundaries.TopBoundary)
                    proCamera2DNumericBoundaries.BottomBoundary = proCamera2DNumericBoundaries.TopBoundary;
            
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                _tooltip = new GUIContent("Use Left", "Prevent camera movement beyond this point");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseLeftBoundary"), _tooltip);
            
                if (proCamera2DNumericBoundaries.UseLeftBoundary)
                {
                    _tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("LeftBoundary"), _tooltip);
                }
            
                if (proCamera2DNumericBoundaries.UseRightBoundary && proCamera2DNumericBoundaries.LeftBoundary > proCamera2DNumericBoundaries.RightBoundary)
                    proCamera2DNumericBoundaries.LeftBoundary = proCamera2DNumericBoundaries.RightBoundary;
            
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.BeginHorizontal();
                _tooltip = new GUIContent("Use Right", "Prevent camera movement beyond this point");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseRightBoundary"), _tooltip);
            
                if (proCamera2DNumericBoundaries.UseRightBoundary)
                {
                    _tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("RightBoundary"), _tooltip);
                }
            
                if (proCamera2DNumericBoundaries.UseLeftBoundary && proCamera2DNumericBoundaries.RightBoundary < proCamera2DNumericBoundaries.LeftBoundary)
                    proCamera2DNumericBoundaries.RightBoundary = proCamera2DNumericBoundaries.LeftBoundary;
            
                EditorGUILayout.EndHorizontal();
            
                if ((proCamera2DNumericBoundaries.UseTopBoundary && proCamera2DNumericBoundaries.UseBottomBoundary && proCamera2DNumericBoundaries.BottomBoundary == proCamera2DNumericBoundaries.TopBoundary) ||
                    (proCamera2DNumericBoundaries.UseLeftBoundary && proCamera2DNumericBoundaries.UseRightBoundary && proCamera2DNumericBoundaries.LeftBoundary == proCamera2DNumericBoundaries.RightBoundary))
                    EditorGUILayout.HelpBox("Same axis boundaries can't have the same value!", MessageType.Error, true);
                                
                EditorGUI.indentLevel = 0;
                EditorGUILayout.Space();
            }


            _tooltip = new GUIContent("Use Elastic Boundaries", "If enabled, the camera will not stop instantly at the boundaries, but instead, it will ease from the current value to the defined boundaries.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseElasticBoundaries"), _tooltip);

            if (proCamera2DNumericBoundaries.UseElasticBoundaries)
            {
                EditorGUI.indentLevel = 1;

                _tooltip = new GUIContent("Horizontal Elasticity Duration", "How long it should take the camera to reach boundaries");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HorizontalElasticityDuration"), _tooltip);

                _tooltip = new GUIContent("Horizontal Elasticity Size", "How far can the camera go beyond the boundaries");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HorizontalElasticitySize"), _tooltip);

                _tooltip = new GUIContent("Vertical Elasticity Duration", "How long it should take the camera to reach boundaries");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VerticalElasticityDuration"), _tooltip);

                _tooltip = new GUIContent("Vertical Elasticity Size", "How far can the camera go beyond the boundaries");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VerticalElasticitySize"), _tooltip);

                _tooltip = new GUIContent("Elasticity Ease Type", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ElasticityEaseType"), _tooltip);

                EditorGUI.indentLevel = 0;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}