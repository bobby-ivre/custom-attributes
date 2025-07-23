// ----------------------------------------------------------------------------------------------------------------------------------------
// Author:				Bobby Greaney
// ----------------------------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LughNut.CustomAttributes
{
    [CustomPropertyDrawer(typeof(OnChangedCallAttribute))]
    public class OnChangedCallAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                OnChangedCallAttribute at = attribute as OnChangedCallAttribute;
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
                foreach( var target in property.serializedObject.targetObjects)
                {
                    MethodInfo[] methods = target.GetType().GetMethods(flags).Where(m => m.Name == at.methodName).ToArray();
                    for (int i = 0; i < methods.Length; i++)
                    {
                        if (methods[i] != null && methods[i].GetParameters().Count() == 0)
                            methods[i].Invoke(target, null);

                    }
                }
            }
        }
    }
}
