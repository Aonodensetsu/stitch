using System;

namespace Me.Aonodensetsu.Stitch {
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  internal class ActionAttribute : Attribute {
    public string LocalizationKey;

    public ActionAttribute(string l18nKey) {
      LocalizationKey = l18nKey;
    }
  }
}

