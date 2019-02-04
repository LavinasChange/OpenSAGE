﻿using OpenSage.Data.Ini.Parser;

namespace OpenSage.Data.Ini
{
    [AddedIn(SageGame.Bfme2)]
    public sealed class ScoredKillEvaAnnouncer
    {
        internal static ScoredKillEvaAnnouncer Parse(IniParser parser)
        {
            return parser.ParseTopLevelNamedBlock(
                (x, name) => x.Name = name,
                FieldParseTable);
        }

        private static readonly IniParseTable<ScoredKillEvaAnnouncer> FieldParseTable = new IniParseTable<ScoredKillEvaAnnouncer>
        {
            { "EvaEvent", (parser, x) => x.EvaEvent = parser.ParseAssetReference() },
            { "CountOnlyKillsByLocalPlayer", (parser, x) => x.CountOnlyKillsByLocalPlayer = parser.ParseBoolean() },
            { "CountOnlyKillsAgainstLocalPlayer", (parser, x) => x.CountOnlyKillsAgainstLocalPlayer = parser.ParseBoolean() },
            { "MinimumCountForAnnouncement", (parser, x) => x.MinimumCountForAnnouncement = parser.ParseInteger() },
            { "MaximumTimeForAnnouncementMS", (parser, x) => x.MaximumTimeForAnnouncementMS = parser.ParseInteger() },
            { "ObjectFilter", (parser, x) => x.ObjectFilter = ObjectFilter.Parse(parser) }
        };

        public string Name { get; private set; }

        public string EvaEvent { get; private set; }
        public bool CountOnlyKillsByLocalPlayer { get; private set; }
        public bool CountOnlyKillsAgainstLocalPlayer { get; private set; }
        public int MinimumCountForAnnouncement { get; private set; }
        public int MaximumTimeForAnnouncementMS { get; private set; }
        public ObjectFilter ObjectFilter { get; private set; }
    }
}
