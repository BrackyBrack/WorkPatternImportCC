using DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPatternImportCC
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable workPatterns = CsvReader.GetWorkPatterns();

            DateTime startDate = DateTime.Parse(workPatterns.Columns[5].ColumnName);
            DateTime endDate = DateTime.Parse(workPatterns.Columns[workPatterns.Columns.Count - 2].ColumnName);
            double totalDays = (endDate - startDate).TotalDays;

            Optimiser optimiser = new Optimiser();
            List<CrewMember> crewMembers = optimiser.GetCrewMembers(workPatterns);

            //Optimise(crewMembers, startDate, totalDays, optimiser);

            GenerateMasterFile(crewMembers, startDate, totalDays+1);

        }

        private static void GenerateMasterFile(List<CrewMember> crewMembers, DateTime startDate, double totalDays)
        {
            StringBuilder sb = new StringBuilder();
            SetHeaders(sb, startDate, totalDays);
            List<List<RosterDay>> rsbPatterns = RosterDay.BuildRsbPatterns();

            foreach (CrewMember crew in crewMembers)
            {
                List<RosterDay> rsbPattern = rsbPatterns[crew.MostEfficientWorkPatternIndex];
                sb.Append($"{crew.ThreeLetter},{crew.PersNr},{crew.Rank},{crew.HomeBase},{crew.MostEfficientWorkPatternIndex}");
                int rsbPatternIndex = 0;
                foreach (RosterDay day in crew.MostEfficientRosterDays)
                {
                    if(day.RosterCode == "0")
                    {
                        sb.Append(",0");
                    }
                    else
                    {
                        sb.Append($",{rsbPattern[rsbPatternIndex].RosterCode}");
                        rsbPatternIndex++;
                    }

                    if(rsbPatternIndex == rsbPattern.Count())
                    {
                        rsbPatternIndex = 0;
                    }
                }
                sb.AppendLine();
            }

            System.IO.File.WriteAllText("C:\\Users\\david.bracken\\OneDrive - TUI\\Documents\\Furlough\\CCMasterRoster.csv", sb.ToString());
        }

        private static void Optimise(List<CrewMember> crewMembers, DateTime startDate, double totalDays, Optimiser optimiser)
        {
            

            BuildRosterFile("MaxWorkDays_Final\\Optimised", crewMembers, startDate, totalDays);

            var baseNames = crewMembers.Select(n => n.HomeBase).Distinct();
            List<Base> bases = new List<Base>();

            foreach (var homeBase in baseNames)
            {
                bases.Add(new Base(homeBase, crewMembers));
            }
            BuildStatsFile("MaxWorkDays_Final\\Optimised Stats", bases);

            foreach (Base @base in bases)
            {
                BuildBaseDayFile($"MaxWorkDays_Final\\{@base.BaseName}AvailabilityByDay", @base);
            }

            for (int i = 0; i < bases.Count; i++)
            {

                optimiser.OptimiseByMinAvailability(bases[i]);
                bases[i].UpdateCrewRoster();
                BuildBaseDayFile($"BaseAndRank_Final\\{bases[i].BaseName}AvailabilityByDay", bases[i]);
            }

            BuildRosterFile("BaseAndRank_Final\\OptimisedBy", crewMembers, startDate, totalDays);
            BuildStatsFile("BaseAndRank_Final\\OptimisedByBaseAndRank Stats", bases);

            for (int i = 0; i < bases.Count; i++)
            {

                optimiser.ReduceBdoOnMinimumDays(bases[i]);
                bases[i].UpdateCrewRoster();
                BuildBaseDayFile($"Reduce_BDOs\\{bases[i].BaseName}AvailabilityByDay", bases[i]);
            }

            BuildRosterFile("Reduce_BDOs\\OptimisedBy", crewMembers, startDate, totalDays);
            BuildStatsFile("Reduce_BDOs\\OptimisedByBaseAndRank Stats", bases);
        }

        private static void BuildBaseDayFile(string fileName, Base @base)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Date,");
            foreach (AvailablityDay day in @base.AvailablityDays)
            {
                sb.Append($"{day.date.ToShortDateString()},");
            }
            sb.AppendLine();
            sb.Append("Available,");
            foreach (AvailablityDay day in @base.AvailablityDays)
            {
                sb.Append($"{day.AvailableCrew},");
            }
            sb.AppendLine();
            sb.Append("RSB,");
            foreach (AvailablityDay day in @base.AvailablityDays)
            {
                sb.Append($"{day.RsbCrewCount},");
            }
            sb.AppendLine();
            sb.Append("RCT,");
            foreach (AvailablityDay day in @base.AvailablityDays)
            {
                sb.Append($"{day.RctCrewCount},");
            }
            sb.AppendLine();
            sb.Append("BDO,");
            foreach (AvailablityDay day in @base.AvailablityDays)
            {
                sb.Append($"{day.BDOCrewCount},");
            }
            sb.AppendLine();
            sb.Append("Pre-Assigned,");
            foreach (AvailablityDay day in @base.AvailablityDays)
            {
                sb.Append($"{day.PreAssignedCrewCount},");
            }

            System.IO.File.WriteAllText($"C:\\Users\\david.bracken\\OneDrive - TUI\\Documents\\Furlough\\Optimised\\{fileName}.csv", sb.ToString());
        }

        private static void BuildStatsFile(string fileName, List<Base> bases)
        {
            StringBuilder sbStats = new StringBuilder();
            //sbStats.AppendLine("Base, Average Available, Min Available, Min Occurences, Average RSB, Min RSB, Min RSB Occurences, Average RCT, Min RCT, Min RCT Occurences");
            //foreach (var homeBase in bases)
            //{
            //    sbStats.AppendLine($"{homeBase.BaseName},{homeBase.AverageAvailability},{homeBase.LowestAvailability},{homeBase.LowestAvailabilityOccurences}," +
            //        $"{homeBase.AverageRsb},{homeBase.LowestRsb},{homeBase.LowestRsbOccurences},{homeBase.AverageRct},{homeBase.LowestRct},{homeBase.LowestRctOccurences}");
            //}

            sbStats.AppendLine("Base,Lowest Available,Lowest Available Occurences");
            foreach (Base homebase in bases)
            {
                sbStats.AppendLine($"{homebase.BaseName},{homebase.LowestAvailability},{homebase.LowestAvailabilityOccurences}");
            }

            System.IO.File.WriteAllText($"C:\\Users\\david.bracken\\OneDrive - TUI\\Documents\\Furlough\\Optimised\\{fileName}.csv", sbStats.ToString());
        }

        private static void BuildRosterFile(string fileName, List<CrewMember> crewMembers, DateTime startDate, double totalDays)
        {
            StringBuilder sb = new StringBuilder();
            SetHeaders(sb, startDate, totalDays);

            foreach (CrewMember crewMember in crewMembers)
            {
                sb.Append($"{crewMember.ThreeLetter}, {crewMember.PersNr}, {crewMember.Rank}, {crewMember.HomeBase}, {crewMember.MostEfficientWorkPatternIndex},");
                foreach (var rosterDay in crewMember.MostEfficientRosterDays)
                {
                    sb.Append($"{rosterDay.RosterCode},");
                }
                sb.AppendLine();
            }

            System.IO.File.WriteAllText($"C:\\Users\\david.bracken\\OneDrive - TUI\\Documents\\Furlough\\Optimised\\{fileName}.csv", sb.ToString());
        }

        private static void SetHeaders(StringBuilder sb, DateTime startDate, double totalDays)
        {
            sb.Append("ThreeLetter,PersNr, Rank, HomeBase, PatternIndex,");
            for (int i = 0; i < totalDays +1; i++)
            {
                sb.Append(startDate.AddDays(i).ToShortDateString() + ",");
            }
            sb.AppendLine();
            
        }

        private static List<DateTime> GetCrewWorkDays(string persNr, DataTable workDays)
        {
            var crewWorkRows = workDays.Select($"CLG_P_PERS_NR = '{persNr}'");
            List<DateTime> result = new List<DateTime>();

            foreach (DataRow row in crewWorkRows)
            {
                DateTime startDate = DateTime.Parse(row[1].ToString());
                DateTime endDate = DateTime.Parse(row[2].ToString()).AddMinutes(-1).Date;

                result.Add(startDate);
                if (startDate != endDate)
                {
                    result.Add(endDate);
                }
            }

            return result;
        }
    }
}
