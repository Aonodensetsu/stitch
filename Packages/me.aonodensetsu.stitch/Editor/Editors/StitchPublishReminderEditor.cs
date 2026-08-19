using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomEditor(typeof(StitchPublishReminder))]
  internal class StitchPublishReminderEditor : Editor {
    private GUIStyle desc;

    public override void OnInspectorGUI() {
      desc ??= new GUIStyle(EditorStyles.label) {
        fontSize = 13,
        wordWrap = true,
        richText = true
      };

      var model = (StitchPublishReminder)target;
      EditorGUILayout.LabelField(Strings.Get("support.manualpublish"), desc);
      EditorGUILayout.Space(2);
      using (new EditorGUI.DisabledGroupScope(true)) {
        EditorGUILayout.ObjectField("Controller (clickable link)", model.controller, typeof(AnimatorController), false);
      }
    }
  }
}

