#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace FiXiK.HierarchyComponentIconAssigner
{
    [CustomPropertyDrawer(typeof(ComponentIcon))]
    public class ComponentIconDrawer : PropertyDrawer
    {
        private const string NoComponentSelectedText = "Компонент не выбран";
        private const string CommandObjectSelectorUpdated = "ObjectSelectorUpdated";
        private const string SearchFilter = "_";

        private const float TextureSize = 32f;
        private const float SpaceBetweenFields = 4f;
        private const float VerticalPadding = 4f;
        private const float HalfValue = 0.5f;

        private readonly Color _noneTextureColor = new(0.5f, 0.5f, 0.5f, 0.3f);

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty iconProperty = property.FindPropertyRelative(ComponentIcon.IconPropertyName);
            SerializedProperty typeNameProperty = property.FindPropertyRelative(ComponentIcon.TypePropertyName);
            EditorGUI.BeginProperty(rect, label, property);

            Rect textureRect = new(rect.x, rect.y, TextureSize, TextureSize);
            DrawTextureField(textureRect, iconProperty);

            float width = rect.width - TextureSize - SpaceBetweenFields;
            float buttonHeight = EditorGUIUtility.singleLineHeight;
            float xPosition = rect.x + TextureSize + SpaceBetweenFields;
            float yPosition = rect.y + (TextureSize - buttonHeight) * HalfValue;

            Rect componentRect = new(xPosition, yPosition, width, buttonHeight);

            Type currentType = Type.GetType(typeNameProperty.stringValue);
            GUIContent buttonContent = new(currentType != null ? currentType.Name : NoComponentSelectedText);
            DrawComponentField(componentRect, buttonContent, typeNameProperty);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            TextureSize + VerticalPadding;

        private void DrawTextureField(Rect textureRect, SerializedProperty iconProperty)
        {
            Texture2D currentTexture = (Texture2D)iconProperty.objectReferenceValue;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (currentTexture != null)
                GUI.DrawTexture(textureRect, currentTexture, ScaleMode.ScaleToFit);
            else
                EditorGUI.DrawRect(textureRect, _noneTextureColor);

            if (Event.current.type == EventType.MouseDown && textureRect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.ShowObjectPicker<Texture2D>(currentTexture, false, SearchFilter, controlID);
                Event.current.Use();
            }

            if (Event.current.commandName == CommandObjectSelectorUpdated && EditorGUIUtility.GetObjectPickerControlID() == controlID)
            {
                iconProperty.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                iconProperty.serializedObject.ApplyModifiedProperties();

                if (Event.current.type != EventType.Layout)
                    Event.current.Use();
            }
        }

        private void DrawComponentField(Rect rect, GUIContent content, SerializedProperty typeNameProperty)
        {
            if (EditorGUI.DropdownButton(rect, content, FocusType.Keyboard))
            {
                ComponentTypeDropdown dropdown = new(new(), selectedType =>
                {
                    typeNameProperty.stringValue = selectedType.AssemblyQualifiedName;
                    typeNameProperty.serializedObject.ApplyModifiedProperties();
                });

                dropdown.Show(rect);
            }
        }
    }
}
#endif