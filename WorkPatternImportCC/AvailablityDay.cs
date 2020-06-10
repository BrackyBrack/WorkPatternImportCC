using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPatternImportCC
{
    class AvailablityDay
    {
        private DateTime _date;
        public DateTime date { get { return _date; } }

        private List<CrewMember> _onCrewMembers; 
        public List<CrewMember> OnCrewMembers { get {return _onCrewMembers; } }

        private List<CrewMember> _offCrewMembers;
        public List<CrewMember> OffCrewMembers { get { return _offCrewMembers; } }
        public int AvailableCrew { get { return OnCrewMembers.Count(); } }
        public int RsbCrewCount { get { return _onRosterDays.Where(n => n.RosterCode == "RSB").Count(); } }
        public int RctCrewCount { get { return _onRosterDays.Where(n => n.RosterCode == "RCT").Count(); } }
        public int BDOCrewCount { get { return _offRosterDays.Where(n => n.RosterCode == "BDO").Count(); } }
        public int PreAssignedCrewCount { get { return _offRosterDays.Where(n => n.RosterCode == "X").Count(); } }
        public int OffCrewCount { get { return _offRosterDays.Where(n => n.RosterCode == "0").Count(); } }

        private List<RosterDay> _onRosterDays;

        private List<RosterDay> _offRosterDays;

        public AvailablityDay(DateTime date, List<CrewMember> crewMembers)
        {
            _onCrewMembers = crewMembers.Where( n => n.IsWorkDay(date)).ToList();
            _date = date;

            _onRosterDays = crewMembers.SelectMany(n => n.MostEfficientRosterDays).Where(n => n.Date == date && n.IsWorkingDay).ToList();
            _offRosterDays = crewMembers.SelectMany(n => n.MostEfficientRosterDays).Where(n => n.Date == date && n.IsWorkingDay == false).ToList();
        }
    }
}
