// ----------------------------------------------------------------------------------------------------------------------------------------
// Author:				Zeminor. Sourced from https://discussions.unity.com/t/best-practice-for-required-references/625133/7
// ----------------------------------------------------------------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace LughNut.CustomAttributes
{
    /// <summary>
    ///  Shows a warning in the inspector if a field with this attribute is null
    /// </summary>
    [CustomPropertyDrawer(typeof(RequiredFieldAttribute))]
	public class RequiredFieldPropertyDrawer : PropertyDrawer
	{
        private const float HELPBOX_HEIGHT = 38.0f;
        private const float TOP_MARGIN = 2.0f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float propHeight = EditorGUI.GetPropertyHeight(property, label);
            position.height = propHeight;

            EditorGUI.PropertyField(position, property, label);

            if (property.objectReferenceValue == null)
            {
                position.y += position.height + TOP_MARGIN;
                position.height = HELPBOX_HEIGHT;
                EditorGUI.HelpBox(position, string.Format("{0} is required!", label.text), MessageType.Error);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label);
            if (property.objectReferenceValue == null)
            {
                height += HELPBOX_HEIGHT + TOP_MARGIN;
            }

            return height;
        }
    }
}
