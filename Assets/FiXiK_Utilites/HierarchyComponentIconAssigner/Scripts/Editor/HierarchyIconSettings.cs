using System;
using System.Collections.Generic;
using UnityEngine;

namespace FiXiK.HierarchyComponentIconAssigner
{
    [Serializable]
    public class HierarchyIconSettings : ScriptableObject
    {
        public const string ComponentIconListPropertyName = nameof(_componentIconList);
        public const string EnablerPropertyName = nameof(_enabled);

        [Tooltip("Включить/выключить отрисовку иконок")]
        [SerializeField] private bool _enabled = true;

        [Tooltip("Выбери иконку, компонент и радуйся =)")]
        [SerializeField] private List<ComponentIcon> _componentIconList;

        public event Action Changed;

        public bool Enabled => _enabled;

        public IReadOnlyList<ComponentIcon> ComponentIconList => _componentIconList;

        private void OnValidate()
        {
            foreach (ComponentIcon componentIcon in _componentIconList)
                componentIcon.UpdateCachedType();

            Changed?.Invoke();
        }
    }

    [Serializable]
    public class ComponentIcon
    {
        public const string IconPropertyName = nameof(_icon);
        public const string TypePropertyName = nameof(_typeName);

        [SerializeField] private Texture2D _icon;
        [SerializeField] private string _typeName;

        private string _cashedTypeName;
        private Type _cachedType;

        public Type Type
        {
            get
            {
                return UpdateCachedType();
            }
            set
            {
                _cachedType = value;
                _typeName = value?.AssemblyQualifiedName;
                _cashedTypeName = _typeName;
            }
        }

        public Texture2D Icon => _icon;

        public Type UpdateCachedType()
        {
            if (_cashedTypeName != _typeName || string.IsNullOrEmpty(_typeName) == false)
            {
                _cashedTypeName = _typeName;                
                _cachedType = string.IsNullOrEmpty(_typeName) ? null : Type.GetType(_typeName);
            }            

            return _cachedType; 
        }

        public void Clear()
        {
            _typeName = null;
            _icon = null;
            _cachedType = null;
        }
    }
}