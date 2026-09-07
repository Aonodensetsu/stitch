using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(SmoothAction), true)]
  internal class SmoothActionDrawer : UnaryActionDrawer {
    internal SerializedProperty type;
    internal SerializedProperty delta;

    internal override bool ValueValidate() => base.ValueValidate() && !float.TryParse(value.stringValue, out _);
    internal virtual bool DeltaValidate() => delta.floatValue > 0 && delta.floatValue < 1;

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      type = property.FindPropertyRelative("type");
      delta = property.FindPropertyRelative("delta");

      var root = base.CreatePropertyGUI(property);
      root.ElementAt(3).style.marginRight = 6 + StitchMenuEditor.Margin;

      root.Add(new PropertyField(type, ""));

      var deltaField = new PropertyField(delta, "") {
        style = {
          flexGrow = 1,
          marginLeft = 6 + StitchMenuEditor.Margin
        }
      };
      root.Add(deltaField);

      ValidateProperty(deltaField, DeltaValidate);
      return root;
    }
  }
}

