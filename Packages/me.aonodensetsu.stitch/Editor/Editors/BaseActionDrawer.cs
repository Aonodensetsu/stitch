using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomPropertyDrawer(typeof(BinaryAction), true)]
  internal class BaseActionDrawer : PropertyDrawer {
    internal GUIStyle boldCenter;

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
      boldCenter ??= new GUIStyle(EditorStyles.boldLabel) {
        alignment = TextAnchor.MiddleCenter
      };
    }

    internal static string GetActionName(SerializedProperty property) {
      var action = property.managedReferenceValue;
      var attribute = System.Attribute.GetCustomAttribute(action.GetType(), typeof(ActionAttribute)) as ActionAttribute;

      return Strings.Get(attribute?.LocalizationKey) ?? action.GetType().Name;
    }
  }
}

