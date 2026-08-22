using VRC.SDKBase.Editor.BuildPipeline;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

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
      if (publisher is InstructionPublisher p) {
        p.Publish(avatar);
        return true;
      }

      foreach (var (obj, actions) in avatar
        .GetComponentsInChildren<StitchMenu>(true)
        .GroupBy(c => c.gameObject)
        .Select(g => ( obj: g.Key, actions: g.SelectMany(c => c.actions) ))
      ) {
        var f = obj.GetComponentsInChildren<StitchMenu>().First();
        var controller = new AnimatorController();
        var act = new Actions(controller);
        var globals = new List<string>();

        controller.AddLayer(new AnimatorControllerLayer {
          stateMachine = new AnimatorStateMachine()
        });
        act.Stitch(new DefaultAction { result = "1", value = 1 });

        controller.CreateBlendTreeInController($"Stitch_{f.id}", out var tree);
        foreach (var action in actions) {
          if (!action.Validate()) {
            Debug.LogWarning($"Stitch: {Strings.Get("log.invalidAction")}", obj);
            continue;
          }
          if (action is GlobalAction) globals.Add(action.result);
          act.Stitch(action);
        }
        tree.children = tree.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();
        foreach (var param in controller.parameters) {
          if (Actions.VRCGlobals.Contains(param.name)) globals.Add(param.name);
        }
        publisher.Publish(obj, controller, globals);
      }
      return true;
    }
  }
}

