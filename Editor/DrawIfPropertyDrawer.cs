// ----------------------------------------------------------------------------------------------------------------------------------------
// Author:				Bobby Greaney
// ----------------------------------------------------------------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace LughNut.CustomAttributes
{
    /// <summary>
    /// Adds support for only showing fields if certain conditions are met.
    /// </summary>
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        // Reference to the attribute on the property.
        DrawIfAttribute drawIf;

        // Field that is being compared.
        SerializedProperty comparedField;

        private UnityEventDrawer eventDrawer;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DontDraw)
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                if (property.FindPropertyRelative("m_PersistentCalls.m_Calls") != null)
                {
                    //Event, not a regular property
                    if (eventDrawer == null)
                        eventDrawer = new UnityEventDrawer();
                    return eventDrawer.GetPropertyHeight(property, label);
                }
                else if (property.propertyType == SerializedPropertyType.Generic)
                {
                    int numChildren = 0;
                    float totalHeight = 0.0f;

                    IEnumerator children = property.GetEnumerator();

                    while (children.MoveNext())
                    {
                        SerializedProperty child = children.Current as SerializedProperty;

                        GUIContent childLabel = new GUIContent(child.displayName);

                        totalHeight += EditorGUI.GetPropertyHeight(child, childLabel) + EditorGUIUtility.standardVerticalSpacing;
                        numChildren++;
                    }

                    // Remove extra space at end, (we only want spaces between items)
                    totalHeight -= EditorGUIUtility.standardVerticalSpacing;

                    return totalHeight;
                }

                return EditorGUI.GetPropertyHeight(property, label);
            }
        }

        /// <summary>
        /// Errors default to showing the property.
        /// </summary>
        private bool ShowMe(SerializedProperty property)
        {
            drawIf = attribute as DrawIfAttribute;
            // Replace propertyname to the value from the parameter
            string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;

            comparedField = property.serializedObject.FindProperty(path);

            if (comparedField == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return true;
            }

            // get the value & compare based on types
            switch (comparedField.type)
            { // Possible extend cases to support your own type
                case "bool":
                    return comparedField.boolValue.Equals(drawIf.comparedValue);
                case "Enum":
                    return comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
                case "int":
                    return comparedField.intValue.Equals(drawIf.comparedValue);
                case "float":
                    return comparedField.floatValue.Equals(drawIf.comparedValue);
                default:
                    Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                    return true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If the condition is met, simply draw the field.
            if (ShowMe(property))
            {
                if (property.FindPropertyRelative("m_PersistentCalls.m_Calls") != null)
                {
                    //Event, not a regular property
                    if (eventDrawer == null)
                        eventDrawer = new UnityEventDrawer();
                    eventDrawer.OnGUI(position, property, label);
                    return;
                }
                // A Generic type means a custom class...
                if (property.propertyType == SerializedPropertyType.Generic)
                {
                    IEnumerator children = property.GetEnumerator();

                    Rect offsetPosition = position;

                    while (children.MoveNext())
                    {
                        SerializedProperty child = children.Current as SerializedProperty;

                        GUIContent childLabel = new GUIContent(child.displayName);

                        float childHeight = EditorGUI.GetPropertyHeight(child, childLabel);
                        offsetPosition.height = childHeight;

                        EditorGUI.PropertyField(offsetPosition, child, childLabel);

                        offsetPosition.y += childHeight + EditorGUIUtility.standardVerticalSpacing;
                    }

                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                }

            } //...check if the disabling type is read only. If it is, draw it disabled
            else if (drawIf.disablingType == DrawIfAttribute.DisablingType.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
        }
    }
}
