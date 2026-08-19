using VRC.SDKBase.Editor.BuildPipeline;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;

namespace Me.Aonodensetsu.Stitch {
  internal class Hook : IVRCSDKPreprocessAvatarCallback {
    public int callbackOrder => -19742;
    public static Publisher Instruction = new InstructionPublisher();
    public static Publisher VRCFury = null;

    public bool OnPreprocessAvatar(GameObject avatar) {
      var flag = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/STITCH_PUBLISH_REMINDER.prefab");
      if (flag) {
        Debug.LogWarning("Stitch: No supported publishing method found in project, not rebuilding to allow manual publishing.", flag);
        return true;
      }
      Directory.Delete("Packages/me.aonodensetsu.stitch/Temp", true);
      Directory.CreateDirectory("Packages/me.aonodensetsu.stitch/Temp");
      AssetDatabase.Refresh();

      var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Packages/me.aonodensetsu.stitch/Temp/Stitch.controller");
      controller.AddParameter(new AnimatorControllerParameter {
        name = "1",
        type = AnimatorControllerParameterType.Float,
        defaultFloat = 1f
      });
      controller.CreateBlendTreeInController("Stitch", out var tree);
      tree.blendType = BlendTreeType.Direct;

      var globals = new List<string>();
      foreach (var component in avatar.GetComponentsInChildren<Component>(true)) {
        var act = new Actions(component, controller);
        foreach (var action in component.actions) {
          if (!action.Validate()) {
            Debug.LogWarning("Stitch: Invalid action, skipped.", component);
            continue;
          }
          #if HAS_VF
          if (action is GlobalAction) globals.Add(action.result);
          #endif
          act.Stitch(action);
        }
      }
      tree.children = tree.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();

      if (VRCFury != null) VRCFury.Publish(avatar, controller, globals);
      else Instruction.Publish();
      return true;
    }
  }
}

