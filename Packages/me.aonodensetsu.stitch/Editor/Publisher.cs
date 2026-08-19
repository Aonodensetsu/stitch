using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;

namespace Me.Aonodensetsu.Stitch {
  internal interface Publisher {
    void Publish(GameObject avatar = null, AnimatorController controller = null, List<string> globals = null);
  }

  internal class InstructionPublisher : Publisher {
    public void Publish(GameObject avatar = null, AnimatorController controller = null, List<string> globals = null) {
      var flag = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/STITCH_PUBLISH_REMINDER.prefab");
      if (flag == null) {
        var controllerfile = AssetDatabase.LoadAssetAtPath<AnimatorController>("Packages/me.aonodensetsu.stitch/Temp/Stitch.controller");

        var obj = new GameObject("STITCH_PUBLISH_REMINDER");
        obj.tag = "EditorOnly";
        UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(obj.transform, false);

        var component = obj.AddComponent<StitchPublishReminder>();
        component.controller = controllerfile;
        var asset = PrefabUtility.SaveAsPrefabAsset(obj, "Assets/STITCH_PUBLISH_REMINDER.prefab");
        Object.DestroyImmediate(obj);

        Debug.LogError("Stitch: No supported auto-publishing method found in project, deploy the built controller manually.", asset);
      }
    }
  }
}

