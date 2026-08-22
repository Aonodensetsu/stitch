using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomEditor(typeof(StitchMenu))]
  internal class StitchMenuEditor : Editor {
    private UnityEditorInternal.ReorderableList actions;
    private SerializedProperty actionsProperty;
    private GUIStyle desc;

    private void OnEnable() {
      actionsProperty = serializedObject.FindProperty("actions");
      actions = new UnityEditorInternal.ReorderableList(serializedObject, actionsProperty, true, false, false, false);

      actions.drawElementCallback = DrawElement;
      actions.elementHeightCallback = GetElementHeight;
    }

    public override void OnInspectorGUI() {
      desc ??= new GUIStyle(EditorStyles.label) {
        fontSize = 13,
        wordWrap = true,
        richText = true
      };

      serializedObject.Update();
      var id = serializedObject.FindProperty("id");
      if (string.IsNullOrEmpty(id.stringValue)) id.stringValue = Guid.NewGuid().ToString("N")[..7];
      EditorGUILayout.LabelField(Strings.Get("support.description"), desc);
      EditorGUILayout.Space(4);
      DrawActionButtons();
      actions.DoLayoutList();
      #if !HAS_VF && !HAS_MA
      EditorGUILayout.HelpBox(Strings.Get("support.menuMissingTools"), MessageType.Error);
      #endif
      serializedObject.ApplyModifiedProperties();
    }

    private void DrawActionButtons() {
      using (new EditorGUILayout.HorizontalScope()) {
        var add = Strings.Get("support.addAction");
        var remove = Strings.Get("support.removeAction");
        var id = ((StitchMenu)target).id;
        var addSize = GUI.skin.button.CalcSize(new GUIContent(add));
        var removeSize = GUI.skin.button.CalcSize(new GUIContent(remove));
        var idSize = EditorStyles.miniLabel.CalcSize(new GUIContent(id));
        if (GUILayout.Button(add, GUILayout.Width(addSize.x + 4f), GUILayout.Height(addSize.y + 2f))) ShowAddMenu();
        using (new EditorGUI.DisabledGroupScope(actions.index < 0)) {
          if (GUILayout.Button(remove, GUILayout.Width(removeSize.x + 4f), GUILayout.Height(removeSize.y + 2f))) RemoveAction();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(id, EditorStyles.miniLabel, GUILayout.Width(idSize.x));
      }
    }

    private void ShowAddMenu() {
      var menu = new GenericMenu();

      var entries = new List<(Type type, string name)>();
      foreach (var type in TypeCache.GetTypesDerivedFrom<Action>()) {
        if (type.IsAbstract) continue;

        var attribute = Attribute.GetCustomAttribute(type, typeof(ActionAttribute)) as ActionAttribute;
        if (attribute == null) continue;

        string name = Strings.Get(attribute.LocalizationKey);
        entries.Add((type, name));
      }

      entries.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));

      foreach (var item in entries) {
        menu.AddItem(new GUIContent(item.name), false, () => AddAction(item.type));
      }

      menu.ShowAsContext();
    }

    private void AddAction(Type type) {
      serializedObject.Update();

      int index = actionsProperty.arraySize;
      actionsProperty.InsertArrayElementAtIndex(index);

      var property = actionsProperty.GetArrayElementAtIndex(index);
      property.managedReferenceValue = Activator.CreateInstance(type);

      serializedObject.ApplyModifiedProperties();

      actions.index = index;
    }

    private void RemoveAction() {
      if (actions.index < 0) return;
      serializedObject.Update();

      actionsProperty.DeleteArrayElementAtIndex(actions.index);

      serializedObject.ApplyModifiedProperties();

      actions.index = Mathf.Clamp(actions.index, -1, actionsProperty.arraySize - 1);
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused) {
      var property = actionsProperty.GetArrayElementAtIndex(index);
      EditorGUI.PropertyField(rect, property, GUIContent.none);
    }

    private float GetElementHeight(int index) {
      var property = actionsProperty.GetArrayElementAtIndex(index);
      return EditorGUI.GetPropertyHeight(property, GUIContent.none, true) + 4f;
    }
  }
}

