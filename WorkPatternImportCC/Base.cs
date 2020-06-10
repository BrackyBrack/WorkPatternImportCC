using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPatternImportCC
{
    class Base
    {
        public string BaseName { get; set; }
        private DateTime _optimisationDate = new DateTime(2020, 7, 1);
        public List<CrewMember> CrewMembers { get; set; }
        public List<AvailablityDay> AvailablityDays { get; set; }
        //public double AverageAvailability { get { return AvailablityDays.Select(n => n.AvailableCrew).DefaultIfEmpty(0).Average(); } }
        //public double AverageRsb { get { return AvailablityDays.Select(n => n.RsbCrew).DefaultIfEmpty(0).Average(); } }
        //public double AverageRct { get { return AvailablityDays.Select(n => n.RctCrew).DefaultIfEmpty(0).Average(); } }
        public int LowestAvailability { get { return AvailablityDays.Where(n => n.date >= _optimisationDate).Min(n => n.AvailableCrew); } }
        public int LowestAvailabilityOccurences { get { return AvailablityDays.Where(n => n.date >= _optimisationDate).Where(n => n.AvailableCrew == LowestAvailability).Count(); } }
        //public int LowestRsb { get { return AvailablityDays.Min(n => n.RsbCrew); } }
        //public int LowestRsbOccurences { get { return AvailablityDays.Where(n => n.RsbCrew == LowestRsb).Count(); } }
        //public int LowestRct { get { return AvailablityDays.Min(n => n.RctCrew); } }
        //public int LowestRctOccurences { get { return AvailablityDays.Where(n => n.RctCrew == LowestRct).Count(); } }
        //public int HighestAvailability { get { return AvailablityDays.Max(n => n.AvailableCrew); } }
        //public int HighestAvailabilityOccurences { get { return AvailablityDays.Where(n => n.AvailableCrew == HighestAvailability).Count(); } }
        public int DaysWithinTenPercent { get { return AvailablityDays.Where(n => n.date >= _optimisationDate).Where(n => n.AvailableCrew < (LowestAvailability * 1.1)).Count(); } }
        public int DaysOneOverLowest { get { return AvailablityDays.Where(n => n.date >= _optimisationDate).Where(n => n.AvailableCrew == LowestAvailability + 1).Count(); } }

        public Base(string baseName, List<CrewMember> crewMembers)
        {
            this.BaseName = baseName;
            this.CrewMembers = crewMembers.Where(n => n.HomeBase == baseName).ToList();
            AvailablityDays = new List<AvailablityDay>();

            CrewMember crewMember = crewMembers[0];
            foreach (RosterDay rosterDay in crewMember.MostEfficientRosterDays)
            {
                AvailablityDays.Add(new AvailablityDay(rosterDay.Date, this.CrewMembers));
            }

        }

        internal void UpdateCrewRoster()
        {
            AvailablityDays = new List<AvailablityDay>();

            CrewMember crewMember = CrewMembers[0];
            foreach (RosterDay rosterDay in crewMember.MostEfficientRosterDays)
            {
                AvailablityDays.Add(new AvailablityDay(rosterDay.Date, this.CrewMembers));
            }
        }

        internal List<AvailablityDay> GetMinimumDays()
        {
            return AvailablityDays.Where(n => n.AvailableCrew == LowestAvailability).ToList();
        }
    }
}
