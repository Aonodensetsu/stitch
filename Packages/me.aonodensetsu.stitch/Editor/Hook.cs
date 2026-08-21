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
    #if HAS_VF
    public static Publisher publisher = new VRCFuryPublisher();
    #elif HAS_MA
    public static Publisher publisher = new ModularAvatarPublisher();
    #else
    public static Publisher publisher = new InstructionPublisher();
    #endif

    public bool OnPreprocessAvatar(GameObject avatar) {
      var obj = new GameObject("Stitch");
      obj.transform.parent = avatar.transform;

      if (publisher is InstructionPublisher p) {
        var flag = p.FlagObj();
        if (flag != null) {
          Debug.LogWarning($"Stitch: {Strings.Get("log.noRebuild")}", flag);
          return true;
        }
      }

      Directory.Delete("Packages/me.aonodensetsu.stitch/Temp", true);
      Directory.CreateDirectory("Packages/me.aonodensetsu.stitch/Temp");
      AssetDatabase.Refresh();

      var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Packages/me.aonodensetsu.stitch/Temp/Stitch.controller");
      controller.layers = controller.layers.Select(l => { l.name = "Stitch"; return l; }).ToArray();
      controller.AddParameter(new AnimatorControllerParameter {
        name = "1",
        type = AnimatorControllerParameterType.Float,
        defaultFloat = 1f
      });
      controller.CreateBlendTreeInController("Stitch", out var tree);
      tree.blendType = BlendTreeType.Direct;

      var globals = new List<string>();
      foreach (var component in avatar.GetComponentsInChildren<StitchMenu>(true)) {
        var act = new Actions(component, controller);
        foreach (var action in component.actions) {
          if (!action.Validate()) {
            Debug.LogWarning($"Stitch: {Strings.Get("log.invalidAction")}.", component);
            continue;
          }
          #if HAS_VF || HAS_MA
          if (action is GlobalAction) globals.Add(action.result);
          #endif
          act.Stitch(action);
        }
      }
      tree.children = tree.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();

      publisher.Publish(obj, controller, globals);
      return true;
    }
  }
}

