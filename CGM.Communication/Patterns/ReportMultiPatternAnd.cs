using CGM.Communication;
using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Patterns
{ 
    public class ReportMultiPatternAnd : IReportPattern
    {

        public List<ReportPattern> Patterns { get; set; } = new List<ReportPattern>();
        public bool Evaluate(byte[] bytes)
        {
            //all should match
            foreach (var pattern in Patterns)
            {
                if (!pattern.Evaluate(bytes))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
