using UnityEngine;
using UnityEditor;
using AKIRA.Behaviour;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(IStateMachine), true)]
public class StateMachineInspector : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        try {
            // interface value
            // managedReferenceValue only available for field with [serializereference]
            var referenceValue = property.managedReferenceValue as IStateMachine;

            // machine list
            var machineElement = DrawMachineElement(referenceValue, property.displayName);

            return machineElement;
        } catch {
            return new HelpBox("Use [SerializeReference] with field", HelpBoxMessageType.Info);
        }
    }

    private VisualElement DrawMachineElement(IStateMachine machine, string name, float board = 0) {
        if (machine == null)
            return new Label($"{name} is null....".Colorful(System.Drawing.Color.GreenYellow));

        // reflection flags
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        // set element name
        name += $"<{machine.GetType().Name}>";

        ListView view;
        if (machine is IHFStateMachine) {
            // draw hfsm
            var map = (Dictionary<string, IStateMachine>)typeof(HierarchyFiniteStateMachine).GetField("machines", flags).GetValue(machine);
            view = new ListView(map.Values.ToArray(), makeItem : () => {
                return new VisualElement();
            }, bindItem : (e, i) => {
                var kvp = map.ElementAt(i);
                e.Add(DrawMachineElement(kvp.Value, kvp.Key, board + 10));
            }) { showFoldoutHeader = true,
                 showBorder = true,
                 headerTitle = name.Colorful(System.Drawing.Color.GreenYellow),
                 // https://forum.unity.com/threads/how-do-i-get-the-height-of-an-item-inside-the-listview.1075555/
                 virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                 focusable = false,
                 style = {
                    paddingLeft = board,
                    paddingRight = board,
                 } };
        } else {
            // draw fsm
            var map = (Dictionary<Type, IState>)typeof(StateMachine).GetField("states", flags).GetValue(machine);
            var curState = (IState)typeof(StateMachine).GetProperty("State", flags).GetValue(machine);
            view = new ListView(map.Values.ToArray(), 20f, () => {
                return new Label();
            }, (e, i) => {
                (e as Label).text = map.ElementAt(i).Key.Name;
            }) { showFoldoutHeader = true,
                 showBorder = true,
                 headerTitle = name.Colorful(System.Drawing.Color.Yellow),
                 focusable = false,
                 style = {
                    paddingLeft = board,
                    paddingRight = board,
                 } };
        }

        view.Q<Foldout>().value = false;
        return view;
    }
}