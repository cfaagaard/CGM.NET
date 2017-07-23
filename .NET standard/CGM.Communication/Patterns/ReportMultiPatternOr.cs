using CGM.Communication;
using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Patterns
{
    public class ReportMultiPatternOr : IReportPattern
    {
        public List<ReportPattern> Patterns { get; set; } = new List<ReportPattern>();
        public bool Evaluate(byte[] bytes)
        {
            //just one should match
            foreach (var pattern in Patterns)
            {
                if (pattern.Evaluate(bytes))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
