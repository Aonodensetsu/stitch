using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomEditor(typeof(StitchInstruction))]
  internal class StitchInstructionEditor : Editor {
    private GUIStyle desc;

    public override void OnInspectorGUI() {
      desc ??= new GUIStyle(EditorStyles.label) {
        fontSize = 13,
        wordWrap = true,
        richText = true
      };

      EditorGUILayout.LabelField(Strings.Get("support.missingTools"), desc);
      EditorGUILayout.Space(4);
      using (new EditorGUILayout.HorizontalScope()) {
        if (GUILayout.Button(Strings.Get("support.VF"))) {
          Application.OpenURL("vcc://vpm/addRepo?url=https%3A%2F%2Fvcc.vrcfury.com");
        }
        if (GUILayout.Button(Strings.Get("support.MA"))) {
          Application.OpenURL("vcc://vpm/addRepo?url=https://vpm.nadena.dev/vpm.json");
        }
      }
    }
  }
}

