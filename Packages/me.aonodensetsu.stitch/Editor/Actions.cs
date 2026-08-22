using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace Me.Aonodensetsu.Stitch {
  internal class Actions {
    private AnimatorController Controller;
    private readonly Dictionary<(string name, float value), AnimationClip> clips = new();

    public static readonly HashSet<string> VRCGlobals = new HashSet<string> {
      "IsLocal",
      "PreviewMode",
      "Viseme",
      "Voice",
      "GestureLeft",
      "GestureRight",
      "GestureLeftWeight",
      "GestureRightWeight",
      "AngularY",
      "VelocityX",
      "VelocityY",
      "VelocityZ",
      "VelocityMagnitude",
      "Upright",
      "Grounded",
      "Seated",
      "AFK",
      "TrackingType",
      "VRMode",
      "MuteSelf",
      "InStation",
      "Earmuffs",
      "IsOnFriendsList",
      "AvatarVersion",
      "IsAnimatorEnabled",
      "ScaleModified",
      "ScaleFactor",
      "ScaleFactorInverse",
      "EyeHeightAsMeters",
      "EyeHeightAsPercent"
    };

    public Actions(AnimatorController controller) {
      Controller = controller;
    }

    internal BlendTree GetRoot() {
      return (BlendTree)Controller.layers[0].stateMachine.states[0].state.motion;
    }

    internal AnimationClip GetOrCreateClip(string name, float value = 0f) {
      var key = (name, value);
      if (clips.TryGetValue(key, out var clip)) return clip;
      clip = new AnimationClip { name = $"{name}_{value}", frameRate = 60f };
      AnimationUtility.SetEditorCurve(
        clip,
        EditorCurveBinding.FloatCurve("", typeof(Animator), name),
        new AnimationCurve(new Keyframe(0f, value), new Keyframe(1f / 60f, value))
      );
      clips.Add(key, clip);
      return clip;
    }

    internal string InternParam() {
      return $"StitchInternal_{Guid.NewGuid().ToString("N")}";
    }

    internal void MakeParameters(string[] s) {
      foreach (var param in s) {
        if (!Controller.parameters.Any(p => p.name == param)) Controller.AddParameter(new AnimatorControllerParameter {
          name = param,
          type = AnimatorControllerParameterType.Float,
          defaultFloat = float.TryParse(param, out float val) ? val : 0f
        });
      }
    }

    public void Stitch(AddAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var negative = GetOrCreateClip(a.result, -100f);
      var positive = GetOrCreateClip(a.result, 100f);

      var plus = root.CreateBlendTreeChild(0);
      plus.name = $"{a.result} = {a.left} + {a.right}";
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

      plus.children = plus.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();
    }

    public void Stitch(AndAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var zero = GetOrCreateClip(a.result);

      var and = root.CreateBlendTreeChild(0);
      and.name = $"{a.result} = {a.left} ∧ {a.right}";
      and.blendParameter = a.left;
      and.AddChild(zero);

      var secondary = and.CreateBlendTreeChild(1);
      secondary.blendParameter = a.right;
      secondary.AddChild(zero);
      secondary.AddChild(GetOrCreateClip(a.result, 1f));
    }

    public void Stitch(DefaultAction a) {
      var parameters = Controller.parameters;
      var parameter = parameters.FirstOrDefault(p => p.name == a.result);
      if (parameter != null) {
        parameter.defaultFloat = a.value;
        Controller.parameters = parameters;
        return;
      }

      Controller.AddParameter(new AnimatorControllerParameter {
        name = a.result,
        type = AnimatorControllerParameterType.Float,
        defaultFloat = a.value
      });
    }

    public void Stitch(GateAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });

      var gate = root.CreateBlendTreeChild(0);
      gate.name = $"{a.result} = Gate {a.left} {a.right}";
      gate.blendParameter = a.left;

      var secondary = gate.CreateBlendTreeChild(0);
      secondary.blendParameter = a.right;
      secondary.AddChild(GetOrCreateClip(a.result, a.zeroZero));
      secondary.AddChild(GetOrCreateClip(a.result, a.zeroOne));

      var tertiary = gate.CreateBlendTreeChild(0);
      tertiary.blendParameter = a.right;
      tertiary.AddChild(GetOrCreateClip(a.result, a.oneZero));
      tertiary.AddChild(GetOrCreateClip(a.result, a.oneOne));
    }

    public void Stitch(GlobalAction a) {
      MakeParameters(new[] { a.result });
    }

    public void Stitch(MultiplyAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var zero = GetOrCreateClip(a.result);

      var times = root.CreateBlendTreeChild(0);
      times.name = $"{a.result} = {a.left} * {a.right}";
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
      not.name = $"{a.result} = ¬ {a.value}";
      not.blendParameter = a.value;
      not.AddChild(GetOrCreateClip(a.result, 1f));
      not.AddChild(GetOrCreateClip(a.result));
    }

    public void Stitch(OrAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var one = GetOrCreateClip(a.result, 1f);

      var or = root.CreateBlendTreeChild(0);
      or.name = $"{a.result} = {a.left} ∨ {a.right}";
      or.blendParameter = a.left;

      var secondary = or.CreateBlendTreeChild(0);
      secondary.blendParameter = a.right;
      secondary.AddChild(GetOrCreateClip(a.result));
      secondary.AddChild(one);

      or.AddChild(one);
    }

    public void Stitch(RemapAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.value });

      var remap = root.CreateBlendTreeChild(0);
      remap.name = $"{a.result} = Remap {a.value} ({a.lowIn}-{a.highIn} -> {a.lowOut}-{a.highOut})";
      remap.blendParameter = a.value;
      remap.minThreshold = a.lowIn;
      remap.maxThreshold = a.highIn;
      remap.AddChild(GetOrCreateClip(a.result, a.lowOut));
      remap.AddChild(GetOrCreateClip(a.result, a.highOut));
    }

    public void Stitch(SmoothAction a) {
      BlendTree root = GetRoot();
      string delta = a.delta.ToString();
      MakeParameters(new[] { a.result, a.value, delta });

      var negative = GetOrCreateClip(a.result, -100f);
      var positive = GetOrCreateClip(a.result, 100f);

      var smooth = root.CreateBlendTreeChild(0);

      var secondary = smooth.CreateBlendTreeChild(0);
      secondary.minThreshold = -100f;
      secondary.maxThreshold = 100f;
      secondary.blendParameter = a.value;

      var tertiary = smooth.CreateBlendTreeChild(0);
      tertiary.minThreshold = -100f;
      tertiary.maxThreshold = 100f;
      tertiary.blendParameter = a.result;

      switch (a.type) {
        case SmoothAction.SmoothType.Exponential:
          smooth.name = $"{a.result} = Smooth {a.value} EXP {a.delta}";
          smooth.blendParameter = delta;

          secondary.AddChild(negative);
          secondary.AddChild(positive);

          tertiary.AddChild(negative);
          tertiary.AddChild(positive);
          break;
        case SmoothAction.SmoothType.Linear:
          smooth.name = $"{a.result} = Smooth {a.value} LIN {a.delta}";
          smooth.blendType = BlendTreeType.Direct;

          var r = InternParam();
          MakeParameters(new[] { r });
          var negativer = GetOrCreateClip(r, -100f);
          var positiver = GetOrCreateClip(r, 100f);

          secondary.AddChild(negativer);
          secondary.AddChild(positiver);

          tertiary.AddChild(positiver);
          tertiary.AddChild(negativer);

          var quarternary = smooth.CreateBlendTreeChild(0);
          quarternary.minThreshold = -100f;
          quarternary.maxThreshold = 100f;
          quarternary.blendParameter = a.result;
          quarternary.AddChild(negative);
          quarternary.AddChild(positive);

          var quinary = smooth.CreateBlendTreeChild(0);
          quinary.minThreshold = -0.1f;
          quinary.maxThreshold = 0.1f;
          quinary.blendParameter = r;
          quinary.AddChild(GetOrCreateClip(a.result, -1f));
          quinary.AddChild(GetOrCreateClip(a.result, 0f));
          quinary.AddChild(GetOrCreateClip(a.result, 1f));

          smooth.children = smooth.children.Select((c, ix) => { c.directBlendParameter = ix == 3 ? delta : "1"; return c; }).ToArray();
          break;
      }
    }

    public void Stitch(SubtractAction a) {
      BlendTree root = GetRoot();
      MakeParameters(new[] { a.result, a.left, a.right });
      var negative = GetOrCreateClip(a.result, -100f);
      var positive = GetOrCreateClip(a.result, 100f);

      var minus = root.CreateBlendTreeChild(0);
      minus.name = $"{a.result} = {a.left} - {a.right}";
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

      minus.children = minus.children.Select(c => { c.directBlendParameter = "1"; return c; }).ToArray();
    }

    // overload dispatcher
    public void Stitch(Action a) {
      switch (a) {
        case AddAction add: Stitch(add); break;
        case AndAction and: Stitch(and); break;
        case DefaultAction def: Stitch(def); break;
        case GateAction g: Stitch(g); break;
        case GlobalAction glo: Stitch(glo); break;
        case MultiplyAction mul: Stitch(mul); break;
        case NotAction not: Stitch(not); break;
        case OrAction or: Stitch(or); break;
        case RemapAction rem: Stitch(rem); break;
        case SmoothAction sm: Stitch(sm); break;
        case SubtractAction sub: Stitch(sub); break;
      }
    }
  }
}

