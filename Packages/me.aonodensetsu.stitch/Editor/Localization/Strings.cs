using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Me.Aonodensetsu.Stitch {
  internal class Strings {
    private static Dictionary<string,string> _tl;

    private static Dictionary<string,string> LoadTL() {
      string language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
      string enpath = Path.GetFullPath("Packages/me.aonodensetsu.stitch/Editor/Localization/en.json");
      var tl = JsonConvert.DeserializeObject<Dictionary<string,string>>(File.ReadAllText(enpath)) ?? new Dictionary<string, string>();
      if (language != "en") {
        string langpath = Path.GetFullPath($"Packages/me.aonodensetsu.stitch/Editor/Localization/{language}.json");
        var lang = JsonConvert.DeserializeObject<Dictionary<string,string>>(File.ReadAllText(langpath)) ?? new Dictionary<string, string>();
        foreach (var pair in lang) tl[pair.Key] = pair.Value;
      }
      return tl;
    }

    public static string Get(string localizationKey) {
      _tl ??= LoadTL();
      return _tl.TryGetValue(localizationKey, out var value) ? value : localizationKey;
    }
  }
}

