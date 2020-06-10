using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.InteropServices;
using GenericParsing;

namespace DataAccess
{
    public static class CsvReader
    {
        public static DataTable GetWorkPatterns()
        {
            DataTable dtResult = new DataTable();
            using (GenericParserAdapter parser = new GenericParserAdapter("C:\\Users\\david.bracken\\OneDrive - TUI\\Documents\\Furlough\\CCMasterRoster.csv"))
            {
                parser.FirstRowHasHeader = true;
                var dsResult = parser.GetDataSet();
                dtResult = dsResult.Tables[0];
            }

            return dtResult;
        }
     }
}
