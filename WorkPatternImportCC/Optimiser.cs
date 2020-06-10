using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPatternImportCC
{
    public class Optimiser
    {
        internal List<CrewMember> GetCrewMembers(DataTable workPatterns)
        {
            List<CrewMember> crewMembers = new List<CrewMember>();

            foreach (DataRow patternRow in workPatterns.Rows)
            {
                CrewMember crewMember = new CrewMember
                {
                    ThreeLetter = patternRow[0].ToString().Trim(),
                    PersNr = patternRow[1].ToString().Trim(),
                    Rank = patternRow[2].ToString().Trim(),
                    HomeBase = patternRow[3].ToString().Trim().Substring(0,3) + patternRow[2].ToString().Trim(),
                    MostEfficientWorkPatternIndex = int.Parse(patternRow[4].ToString().Trim())
                };

                crewMembers.Add(crewMember);

                BuildCrewRosters(crewMember, patternRow);
            }
            return crewMembers;
        }

        private void BuildCrewRosters(CrewMember crewMember, DataRow patternRow)
        {

            List<List<RosterDay>> rsbPatterns = RosterDay.BuildRsbPatterns();

            for (int workingRsbPattern = 0; workingRsbPattern < rsbPatterns.Count; workingRsbPattern++)
            {
                List<RosterDay> rosterDays = new List<RosterDay>();
                int RsbPatternDay = 0;
                for (int i = 5; i < patternRow.Table.Columns.Count; i++)
                {
                    RosterDay rosterDay = new RosterDay { Date = DateTime.Parse(patternRow.Table.Columns[i].ColumnName) };
                    string patternResult = patternRow[i].ToString();

                    if (patternResult == "0")
                    {
                        rosterDay.RosterCode = "0";
                        rosterDays.Add(rosterDay);
                    }
                    else if (patternResult == "X")
                    {
                        rosterDay.RosterCode = "X";
                        rosterDays.Add(rosterDay);
                        RsbPatternDay++;
                    }
                    else
                    {
                        rosterDay.RosterCode = rsbPatterns[workingRsbPattern][RsbPatternDay].RosterCode;
                        rosterDays.Add(rosterDay);
                        RsbPatternDay++;
                    }

                    if (RsbPatternDay == rsbPatterns[workingRsbPattern].Count)
                    {
                        RsbPatternDay = 0;
                    }
                }

                crewMember.AddRsbPattern(workingRsbPattern, rosterDays);
            }
        }

        internal void OptimiseByMaxWorkDays(List<CrewMember> crewMembers) 
        {
            for (int crewMemberIndex = 0; crewMemberIndex < crewMembers.Count; crewMemberIndex++)
            {
                CrewMember crewMember = crewMembers[crewMemberIndex];
                int workingDaysCount = 0;

                for (int workPattern = 0; workPattern < crewMember.RosterDays.Count(); workPattern++)
                {
                    var workingDays = crewMember.RosterDays[workPattern].Where(n => n.IsWorkingDay);
                    if (workingDays.Count() > workingDaysCount)
                    {
                        crewMember.MostEfficientWorkPatternIndex = workPattern;
                    }

                    if (workingDays.Count() == workingDaysCount)
                    {
                        int crewMembersOnCurrentPattern = crewMembers.Where(n => n.MostEfficientWorkPatternIndex == crewMember.MostEfficientWorkPatternIndex).Count();
                        int crewMembersOnNewPattern = crewMembers.Where(n => n.MostEfficientWorkPatternIndex == workPattern).Count();
                        if(crewMembersOnCurrentPattern > crewMembersOnNewPattern)
                        {
                            crewMember.MostEfficientWorkPatternIndex = workPattern;
                        }
                    }
                }
            }
        }

        internal void OptimiseByMinAvailability(Base homeBase)
        {
            List<CrewMember> crewMembers = homeBase.CrewMembers;
            bool rosterChanged = false;
            do
            {
                rosterChanged = false;
                for (int i = 0; i < crewMembers.Count; i++)
                {
                    CrewMember crew = crewMembers[i];
                    OptimiseForBase(crew, ref homeBase, crewMembers, ref rosterChanged);
                }
                Console.WriteLine($"Optimised for {homeBase.BaseName} - new low {homeBase.LowestAvailability} new occurences {homeBase.LowestAvailabilityOccurences} new 10% {homeBase.DaysWithinTenPercent} new 1 over {homeBase.DaysOneOverLowest}{rosterChanged}");

                for (int i = crewMembers.Count - 1; i > -1; i--)
                {
                    CrewMember crew = crewMembers[i];
                    OptimiseForBase(crew, ref homeBase, crewMembers, ref rosterChanged);
                }
                Console.WriteLine($"Optimised for {homeBase.BaseName} - new low {homeBase.LowestAvailability} new occurences {homeBase.LowestAvailabilityOccurences} new 10% {homeBase.DaysWithinTenPercent} new 1 over {homeBase.DaysOneOverLowest}{rosterChanged}");


            } while (rosterChanged);
        }

        private void OptimiseForBase(CrewMember crew, ref Base homeBase, List<CrewMember> crewMembers, ref bool rosterChanged)
        {
            for (int workPattern = 0; workPattern < crew.RosterDays.Count(); workPattern++)
            {
                int existingLowestAvailability = homeBase.LowestAvailability;
                int existingLowestOccurences = homeBase.LowestAvailabilityOccurences;
                int existingMostEfficientPattern = crew.MostEfficientWorkPatternIndex;
                int existingDaysWithinTenPercent = homeBase.DaysWithinTenPercent;
                int existingOneOver = homeBase.DaysOneOverLowest;

                crew.MostEfficientWorkPatternIndex = workPattern;
                Base newBase = new Base(crew.HomeBase, crewMembers);

                if(SolutionGotWorse(newBase, existingLowestAvailability, existingLowestOccurences, existingOneOver, existingDaysWithinTenPercent))
                {
                    crew.MostEfficientWorkPatternIndex = existingMostEfficientPattern;
                }

                else if(SolutionGotBetter(newBase, existingLowestAvailability, existingLowestOccurences, existingOneOver, existingDaysWithinTenPercent))
                {
                    homeBase = newBase;
                    rosterChanged = true;
                }
                else
                {
                    homeBase = newBase;
                }
            }
        }

        private bool SolutionGotBetter(Base newBase, int existingLowestAvailability, int existingLowestOccurences, int existingOneOver, int existingDaysWithinTenPercent)
        {
            if (SolutionGotWorse(newBase, existingLowestAvailability, existingLowestOccurences, existingOneOver, existingDaysWithinTenPercent) == false)
            {
                return (
                        newBase.LowestAvailability > existingLowestAvailability ||
                        newBase.LowestAvailabilityOccurences < existingLowestOccurences ||
                        newBase.DaysOneOverLowest < existingOneOver ||
                        newBase.DaysWithinTenPercent < existingDaysWithinTenPercent
                        );
            }
            else return false;
        }

        private bool SolutionGotWorse(Base newBase, int existingLowestAvailability, int existingLowestOccurences, int existingOneOver, int existingDaysWithinTenPercent)
        {
            if(existingLowestAvailability < newBase.LowestAvailability)
            {
                return false;
            }
            else if (existingLowestAvailability == newBase.LowestAvailability && existingLowestOccurences > newBase.LowestAvailabilityOccurences)
            {
                return false;
            }
            if (existingLowestAvailability > newBase.LowestAvailability)
            {
                return true;
            }
            //remove this if part to get the V4 output from V2
            else if (existingLowestAvailability == newBase.LowestAvailability && existingOneOver < newBase.DaysOneOverLowest)
            {
                return true;
            }
            //if lowest availability is the same, but there are more days within 10 percent
            else if (existingLowestAvailability == newBase.LowestAvailability && existingDaysWithinTenPercent < newBase.DaysWithinTenPercent)
            {
                return true;
            }
            //if lowest availability is the same, but there are more occurences of it
            else if (existingLowestAvailability == newBase.LowestAvailability && existingLowestOccurences < newBase.LowestAvailabilityOccurences)
            {
                return true;
            }
            else return false;
        }

        internal void ReduceBdoOnMinimumDays(Base homeBase)
        {
            List<AvailablityDay> minimumDays = homeBase.GetMinimumDays();
            foreach (AvailablityDay day in minimumDays)
            {
                List<CrewMember> crewMembers = homeBase.CrewMembers.Where(n => n.IsDayOff(day.date)).ToList();
                
            }
        }
    }
}
