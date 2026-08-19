using System;

namespace Me.Aonodensetsu.Stitch {
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public sealed class ActionAttribute : Attribute {
    public string LocalizationKey { get; }

    public ActionAttribute(string localizationKey) {
      LocalizationKey = localizationKey;
    }
  }
}

