// ------------------------------------------------------------
//  Statistics.cs (не меняется)
// ------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace LogAnalyzer.Core.MenuSettingsFormation.AsyncMainMethods.Analizer
{
    public class Statistics
    {
        public Dictionary<string, int> LinePatternCount { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> FileLineCounters { get; set; }
        public Dictionary<string, int> WordTotalCount { get; set; }
        public Dictionary<string, Dictionary<string, int>> FileWordRegistry { get; set; }
        public Dictionary<string, List<MatchDetail>> FileMatchDetails { get; set; }
            = new Dictionary<string, List<MatchDetail>>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, List<MatchDetail>> MatchesByWord { get; set; }

        public Statistics()
        {
            FileLineCounters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            WordTotalCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            FileWordRegistry = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
            FileMatchDetails = new Dictionary<string, List<MatchDetail>>(StringComparer.OrdinalIgnoreCase);
            MatchesByWord = new Dictionary<string, List<MatchDetail>>(StringComparer.OrdinalIgnoreCase);
        }

        public class MatchDetail
        {
            public int LineNumber { get; set; }
            public string Line { get; set; }
            public string FilePath { get; set; }
            public List<string> FoundWords { get; set; }
        }
    }
}