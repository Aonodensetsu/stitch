using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  [CustomEditor(typeof(StitchInstruction))]
  internal class StitchInstructionEditor : Editor {
    internal void mkLinkBtn(string text, string url, VisualElement parent) {
      var btn = new Button {
        text = text,
        style = {
          minHeight = 24,
          flexGrow = 1
        }
      };
      parent.Add(btn);

      btn.clicked += () => {
        Application.OpenURL(url);
      };
    }

    public override VisualElement CreateInspectorGUI() {
      var root = new VisualElement();

      StitchMenuEditor.mkDescription(Strings.Get("support.missingTools"), root);
      var toolbar = StitchMenuEditor.mkToolbar(root);

      mkLinkBtn(Strings.Get("support.VF"), "vcc://vpm/addRepo?url=https://vcc.vrcfury.com", toolbar);
      mkLinkBtn(Strings.Get("support.MA"), "vcc://vpm/addRepo?url=https://vpm.nadena.dev/vpm.json", toolbar);

      return root;
    }
  }
}

