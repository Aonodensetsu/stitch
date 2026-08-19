using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Me.Aonodensetsu.Stitch {
  internal class Actions {
    private Component Comp;
    private AnimatorController Controller;

    public Actions(Component component, AnimatorController controller) {
      Comp = component;
      Controller = controller;
    }

    internal BlendTree GetRoot() {
      return (BlendTree)Controller.layers[0].stateMachine.states[0].state.motion;
    }

    internal AnimationClip GetOrCreateClip(string name, float value) {
      var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"Packages/me.aonodensetsu.stitch/Temp/{name}_{value}.anim");
      if (clip != null) return clip;

      clip = new AnimationClip { name = name };
      AnimationUtility.SetEditorCurve(
        clip,
        EditorCurveBinding.FloatCurve("", typeof(Animator), name),
        new AnimationCurve(new Keyframe(9f, value))
      );

      AssetDatabase.CreateAsset(clip, $"Packages/me.aonodensetsu.stitch/Temp/{name}_{value}.anim");
      AssetDatabase.SaveAssets();
      return clip;
    }

    internal void MakeParameters(string[] s) {
      foreach (var param in s) {
        if (!Controller.parameters.Any(p => p.name == param)) Controller.AddParameter(param, AnimatorControllerParameterType.Float);
      }
    }

    public void Stitch(AddAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var negative = GetOrCreateClip(a.result, -100f);
      var positive = GetOrCreateClip(a.result, 100f);

      var plus = root.CreateBlendTreeChild(0);
      plus.blendType = BlendTreeType.Direct;

      var secondary = plus.CreateBlendTreeChild(0);
      secondary.blendParameter = a.left;
      secondary.minThreshold = -100;
      secondary.maxThreshold = 100;
      secondary.AddChild(negative);
      secondary.AddChild(positive);

      var tertiary = plus.CreateBlendTreeChild(0);
      tertiary.blendParameter = a.right;
      tertiary.minThreshold = -100;
      tertiary.maxThreshold = 100;
      tertiary.AddChild(negative);
      tertiary.AddChild(positive);

      plus.children = plus.children.Select(c => { c.directBlendParameter = "Weight"; return c; }).ToArray();
    }

    public void Stitch(AndAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var zero = GetOrCreateClip(a.result, 0f);

      var and = root.CreateBlendTreeChild(0);
      and.blendParameter = a.left;
      and.AddChild(zero);

      var secondary = and.CreateBlendTreeChild(1);
      secondary.blendParameter = a.right;
      secondary.AddChild(zero);
      secondary.AddChild(GetOrCreateClip(a.result, 1f));
    }

    public void Stitch(DefaultAction a) {
      var parameter = Controller.parameters.FirstOrDefault(p => p.name == a.result);
      if (parameter != null) {
        parameter.defaultFloat = a.value;
        return;
      }

      Controller.AddParameter(new AnimatorControllerParameter {
        name = a.result,
        type = AnimatorControllerParameterType.Float,
        defaultFloat = a.value
      });
    }

    #if HAS_VF
    public void Stitch(GlobalAction a) {
      MakeParameters(new[] { a.result });
    }
    #endif

    public void Stitch(MultiplyAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var zero = GetOrCreateClip(a.result, 0f);

      var times = root.CreateBlendTreeChild(0);
      times.blendParameter = a.left;
      times.maxThreshold = 10;
      times.AddChild(zero);

      var secondary = times.CreateBlendTreeChild(0);
      secondary.blendParameter = a.right;
      secondary.maxThreshold = 10;
      secondary.AddChild(zero);
      secondary.AddChild(GetOrCreateClip(a.result, 100f));
    }

    public void Stitch(NotAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.value });

      var not = root.CreateBlendTreeChild(0);
      not.blendParameter = a.value;
      not.AddChild(GetOrCreateClip(a.result, 1f));
      not.AddChild(GetOrCreateClip(a.result, 0f));
    }

    public void Stitch(OrAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var one = GetOrCreateClip(a.result, 1f);

      var or = root.CreateBlendTreeChild(0);
      or.blendParameter = a.left;

      var secondary = or.CreateBlendTreeChild(0);
      secondary.blendParameter = a.right;
      secondary.AddChild(GetOrCreateClip(a.result, 0f));
      secondary.AddChild(one);

      or.AddChild(one);
    }

    public void Stitch(SubtractAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var negative = GetOrCreateClip(a.result, -100f);
      var positive = GetOrCreateClip(a.result, 100f);

      var minus = root.CreateBlendTreeChild(0);
      minus.blendType = BlendTreeType.Direct;

      var secondary = minus.CreateBlendTreeChild(0);
      secondary.blendParameter = a.left;
      secondary.minThreshold = -100;
      secondary.maxThreshold = 100;
      secondary.AddChild(negative);
      secondary.AddChild(positive);

      var tertiary = minus.CreateBlendTreeChild(0);
      tertiary.blendParameter = a.right;
      tertiary.minThreshold = -100;
      tertiary.maxThreshold = 100;
      tertiary.AddChild(positive);
      tertiary.AddChild(negative);

      minus.children = minus.children.Select(c => { c.directBlendParameter = "Weight"; return c; }).ToArray();
    }

    // overload dispatcher
    public void Stitch(Action a) {
      switch (a) {
        case AddAction add: Stitch(add); break;
        case AndAction and: Stitch(and); break;
        case DefaultAction def: Stitch(def); break;
        #if HAS_VF
        case GlobalAction glo: Stitch(glo); break;
        #endif
        case MultiplyAction mul: Stitch(mul); break;
        case NotAction not: Stitch(not); break;
        case OrAction or: Stitch(or); break;
        case SubtractAction sub: Stitch(sub); break;
      }
    }
  }
}

