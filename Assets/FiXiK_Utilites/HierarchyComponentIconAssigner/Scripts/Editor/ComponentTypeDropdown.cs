using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ComponentTypeDropdown : AdvancedDropdown
{
    private const string Tittle = "Выберите компонент";
    private const string NoneNamespacesText = "Без пространства имён";

    private readonly Action<Type> _onSelected;
    private readonly Dictionary<int, Type> _idToTypeMap = new();

    public ComponentTypeDropdown(AdvancedDropdownState state, Action<Type> onSelected) : base(state)
    {
        _onSelected = onSelected;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        _idToTypeMap.Clear();
        AdvancedDropdownItem root = new(Tittle);

        IEnumerable<Type> componentTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try { return assembly.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .Where(type => type.IsSubclassOf(typeof(Component)) && type.IsAbstract == false);

        IOrderedEnumerable<IGrouping<string, Type>> groupedTypes = componentTypes
            .GroupBy(type => type.Namespace ?? NoneNamespacesText)
            .OrderBy(group => group.Key == NoneNamespacesText ? 0 : 1)
            .ThenBy(group => group.Key);

        foreach (IGrouping<string, Type> group in groupedTypes)
        {
            AdvancedDropdownItem groupItem = new(group.Key);

            foreach (Type type in group.OrderBy(type => type.Name))
            {
                string uniqueIdentifier = $"{type.FullName}, {type.Assembly.GetName().Name}";
                int id = uniqueIdentifier.GetHashCode();
                _idToTypeMap[id] = type;
                string typeNamespace = string.IsNullOrEmpty(type.Namespace) ? string.Empty : $"({type.Namespace})"; 
                string displayName = $"{type.Name} {typeNamespace}";
                AdvancedDropdownItem typeItem = new(displayName) { id = id };
                groupItem.AddChild(typeItem);
            }

            root.AddChild(groupItem);
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        if (_idToTypeMap.TryGetValue(item.id, out Type selected))
            _onSelected?.Invoke(selected);
        else
            Debug.LogError($"Тип с идентификатором {item.id} не найден");
    }
}