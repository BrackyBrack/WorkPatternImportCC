using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPatternImportCC
{
    class RosterDay
    {
        public DateTime Date { get; set; }
        public string RosterCode { get; set; }
        public bool IsWorkingDay { get { return (RosterCode == "RSB" || RosterCode == "RCT"); } }

        internal static List<List<RosterDay>> BuildRsbPatterns()
        {
            return new List<List<RosterDay>>
            {
                new List<RosterDay>
                {
                    new RosterDay{RosterCode = "RSB"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "BDO"},
                    new RosterDay{RosterCode = "BDO"}
                },
                new List<RosterDay>
                {
                    new RosterDay{RosterCode = "BDO"},
                    new RosterDay{RosterCode = "RSB"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "RCT"},
                    new RosterDay{RosterCode = "BDO"}
                },
                new List<RosterDay>
                {
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "RSB" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" }
                },
                new List<RosterDay>
                {
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "RSB" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" }
                },
                new List<RosterDay>
                {
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "RSB" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" }
                },
                new List<RosterDay>
                {
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "RSB" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" }
                },
                new List<RosterDay>
                {
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "RSB" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" }
                },
                new List<RosterDay>
                {
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "RSB" },
                    new RosterDay { RosterCode = "RCT" }
                },
                new List<RosterDay>
                {
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "RCT" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "BDO" },
                    new RosterDay { RosterCode = "RSB" }
                }
            };
        }
    }
}
