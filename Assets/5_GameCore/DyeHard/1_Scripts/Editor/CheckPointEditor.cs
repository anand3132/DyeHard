namespace RedGaint.Games.DyeHard.Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(CheckPoint))]
    public class CheckPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            // Show type list
            EditorGUILayout.LabelField("CheckPoint Types", EditorStyles.boldLabel);
            SerializedProperty typesProp = serializedObject.FindProperty("checkPointTypes");
            EditorGUILayout.PropertyField(typesProp, true);
        
            // Add type button
            if (GUILayout.Button("Add New Type"))
            {
                GenericMenu menu = new GenericMenu();
                foreach (GlobalEnums.CheckPoints type in System.Enum.GetValues(typeof(GlobalEnums.CheckPoints)))
                {
                    menu.AddItem(new GUIContent(type.ToString()), false, () => {
                        ((CheckPoint)target).AddType(type);
                        EditorUtility.SetDirty(target);
                    });
                }
                menu.ShowAsContext();
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}