using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPatternImportCC
{
    class CrewMember
    {
        public string PersNr { get; set; }
        public string ThreeLetter { get; set; }
        public string Rank { get; set; }
        public string HomeBase { get; set; }
        public List<RosterDay> MostEfficientRosterDays { get { return RosterDays[MostEfficientWorkPatternIndex]; } }

        public List<RosterDay>[] RosterDays = new List<RosterDay>[9];
        public int MostEfficientWorkPatternIndex { get; set; }
        public bool IsWorkDay(DateTime date)
        {
            RosterDay rosterDay = MostEfficientRosterDays.FirstOrDefault(n => n.Date == date);

            if (rosterDay != null && rosterDay.IsWorkingDay)
            {
                return true;
            }

            return false;
        }

        internal void AddRsbPattern(int index, List<RosterDay> rosterDays)
        {
            RosterDays[index] = rosterDays;
        }

        public bool IsDayOff(DateTime date)
        {
            return MostEfficientRosterDays.FirstOrDefault(n => n.Date == date).RosterCode == "BDO";
        }
    }
}
