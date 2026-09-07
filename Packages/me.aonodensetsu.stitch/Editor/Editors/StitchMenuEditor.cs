using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomEditor(typeof(StitchMenu))]
  internal class StitchMenuEditor : Editor {
    internal static float Margin = 3;

    internal static void mkDescription(string text, VisualElement parent) {
      parent.Add(new Label(text) {
        style = {
          fontSize = 13,
          whiteSpace = WhiteSpace.Normal,
          marginTop = Margin * 2,
          marginBottom = Margin * 2
        }
      });
    }

    internal static void mkError(string text, VisualElement parent) {
      var h = new HelpBox(text, HelpBoxMessageType.Error) {
        style = {
          marginTop = Margin * 2,
          marginBottom = Margin * 2
        }
      };
      parent.Add(h);
      var l = h.Q<Label>();
      l.style.fontSize = 13;
      l.style.whiteSpace = WhiteSpace.Normal;
    }

    internal static VisualElement mkToolbar(VisualElement parent) {
      var toolbar = new VisualElement {
        style = {
          flexDirection = FlexDirection.Row,
          alignItems = Align.Center,
          marginBottom = Margin * 2
        }
      };
      parent.Add(toolbar);
      return toolbar;
    }

    internal ListView mkView(VisualElement parent, SerializedProperty actionsProperty) {
      var actionsView = new ListView {
        reorderable = true,
        showBoundCollectionSize = false,
        virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
        reorderMode = ListViewReorderMode.Animated,
        fixedItemHeight = 17,
        usageHints = UsageHints.GroupTransform,
        style = {
          marginBottom = Margin * 2
        }
      };
      actionsView.BindProperty(actionsProperty);
      actionsView.makeItem = () => {
        var row = new VisualElement();
        row.Add(new PropertyField {
          style = {
            marginTop = Margin,
            marginBottom = Margin,
            marginRight = Margin
          }
        });
        return row;
      };
      actionsView.bindItem = (el, i) => {
        el.Q<PropertyField>().BindProperty(actionsProperty.GetArrayElementAtIndex(i));
      };
      parent.Add(actionsView);
      return actionsView;
    }

    internal Button mkAddBtn(VisualElement parent, SerializedProperty actionsProperty, ListView actionsView) {
      var addBtn = new Button {
        text = Strings.Get("support.addAction"),
        style = {
          minWidth = 24,
          minHeight = 24,
          paddingLeft = Margin * 2,
          paddingRight = Margin * 2
        }
      };
      parent.Add(addBtn);

      addBtn.clicked += () => {
        var menu = new GenericMenu();

        var entries = new List<(Type type, string name)>();
        foreach (var type in TypeCache.GetTypesDerivedFrom<Action>()) {
          if (type.IsAbstract) continue;

          var attribute = (ActionAttribute)Attribute.GetCustomAttribute(type, typeof(ActionAttribute));
          if (attribute == null) continue;

          string name = Strings.Get(attribute.LocalizationKey);
          entries.Add((type, name));
        }

        entries.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));

        foreach (var item in entries) {
          menu.AddItem(new GUIContent(item.name), false, () => {
            serializedObject.Update();

            int index = actionsProperty.arraySize;
            actionsProperty.InsertArrayElementAtIndex(index);

            var element = actionsProperty.GetArrayElementAtIndex(index);
            element.managedReferenceValue = Activator.CreateInstance(item.type);

            serializedObject.ApplyModifiedProperties();

            actionsView.SetSelection(actionsProperty.arraySize - 1);
          });
        }

        menu.ShowAsContext();
      };
      return addBtn;
    }

    internal Button mkRmBtn(VisualElement parent, SerializedProperty actionsProperty, ListView actionsView) {
      var rmBtn = new Button {
        text = Strings.Get("support.removeAction"),
        style = {
          minWidth = 24,
          minHeight = 24,
          paddingLeft = Margin * 2,
          paddingRight = Margin * 2
        }
      };
      parent.Add(rmBtn);

      rmBtn.clicked += () => {
        var index = actionsView.selectedIndex;
        if (index < 0) return;
        serializedObject.Update();

        actionsProperty.DeleteArrayElementAtIndex(index);

        serializedObject.ApplyModifiedProperties();

        if (index > 0) actionsView.SetSelection(index - 1);
        else if (actionsProperty.arraySize > 0) actionsView.SetSelection(0);
        else rmBtn.SetEnabled(false);
      };

      if (actionsProperty.arraySize > 0) actionsView.SetSelection(actionsProperty.arraySize - 1);
      else rmBtn.SetEnabled(false);
      return rmBtn;
    }

    public override VisualElement CreateInspectorGUI() {
      var root = new VisualElement();

      mkDescription(Strings.Get("support.description"), root);
      #if !HAS_VF && !HAS_MA
      mkError(Strings.Get("support.menuMissingTools"), root);
      #endif
      var toolbar = mkToolbar(root);

      serializedObject.Update();
      var actionsProperty = serializedObject.FindProperty("actions");

      var actionsView = mkView(root, actionsProperty);

      var addBtn = mkAddBtn(toolbar, actionsProperty, actionsView);
      var rmBtn = mkRmBtn(toolbar, actionsProperty, actionsView);

      toolbar.Add(new Label(Math.Abs(
        ((Component)serializedObject.targetObject).gameObject.GetInstanceID()
      ).ToString("X")) {
        style = {
          marginLeft = StyleKeyword.Auto
        }
      });

      addBtn.clicked += () => {
        rmBtn.SetEnabled(true);
      };

      return root;
    }
  }
}

