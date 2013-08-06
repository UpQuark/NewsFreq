using System.Collections.Generic;

namespace NewsLibrarySearch
{
    public static class NlSearchTargets
    {
        private static readonly Dictionary<string, string> TargetsFromKeys = new Dictionary<string, string>
                          {
                              {"All Text", ""},
                              {"Lede", "lead"},
                              {"Headline", "title"},
                              {"Index Terms", "Topics"},
                              {"Author", "Author"},
                              {"Section", "Section"},
                              {"Edition", "Edition"},
                              {"Caption", "CapGraph"},
                              {"Page", "Page"},
                              {"Source", "Source"}
                          };

        private static readonly Dictionary<string, string> KeysFromTargets = new Dictionary<string, string>
                          {
                              {"", "All Text"},
                              {"lead", "Lede"},
                              {"title", "Headline"},
                              {"Topics", "Index Terms"},
                              {"Author", "Author"},
                              {"Section", "Section"},
                              {"Edition", "Edition"},
                              {"CapGraph", "Caption"},
                              {"Page", "Page"},
                              {"Source", "Source"}
                          };

        public static string Target(string key)
        {
            return TargetsFromKeys[key];
        }

        public static string Key(string target)
        {
            return KeysFromTargets[target];
        }
    }
}
