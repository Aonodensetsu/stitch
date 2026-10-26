using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(DriveAction), true)]
  internal class DriveActionDrawer : ActionDrawer {
    internal SerializedProperty min;
    internal SerializedProperty max;
    internal SerializedProperty clip;

    internal static bool IsConstantClip(AnimationClip c) => AnimationUtility.GetCurveBindings(c)
      .Select(b => AnimationUtility.GetEditorCurve(c, b))
      .Where(cv => cv != null)
      .All(cv => cv.keys.All(k => Mathf.Approximately(k.value, cv.keys[0].value)));

    internal virtual bool MaxValidate() => max.floatValue > min.floatValue;
    internal virtual bool ClipValidate() => clip.objectReferenceValue is AnimationClip c && !IsConstantClip(c);

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
      min = property.FindPropertyRelative("min");
      max = property.FindPropertyRelative("max");
      clip = property.FindPropertyRelative("clip");

      var root = new VisualElement();
      root.Add(base.CreatePropertyGUI(property));

      var lineTwo = new VisualElement {
        style = {
          flexDirection = FlexDirection.Row,
          alignItems = Align.Center,
          marginTop = StitchMenuEditor.Margin
        }
      };
      root.Add(lineTwo);

      var minProp = new PropertyField(min, "") {
        style = {
          flexGrow = 1,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      };
      lineTwo.Add(minProp);
      var maxProp = new PropertyField(max, "") {
        style = {
          flexGrow = 1,
          marginRight = 6 + StitchMenuEditor.Margin
        }
      };
      lineTwo.Add(maxProp);
      var clipProp = new PropertyField(clip, "") {
        style = {
          flexGrow = 1
        }
      };
      lineTwo.Add(clipProp);

      ValidateProperty(maxProp, MaxValidate);
      CrossValidateProperty(minProp, maxProp, MaxValidate);

      clipProp.RegisterValueChangeCallback(e => {
        var t = clipProp.Q<ObjectField>();
        if (t == null) return;
        t.style.borderLeftColor = ClipValidate() ? StyleKeyword.Null : Color.yellow;
        t.style.borderLeftWidth = 1;
      });
      return root;
    }
  }
}

