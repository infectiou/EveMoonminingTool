using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EveMoonminingTool.Models
{
    public class CorpSummary
    {
        public string Corp { get; set; }
        public List<TaxedJob> TJobs;
        public List<PilotSummary> PilotSummarys { get; set; }
        

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TValue { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public float Volume { get; set; } = 0;

        public CorpSummary(string corp, List<TaxedJob> jobs, List<PilotSummary> pilots)
        {
            this.Corp = corp;
            this.TJobs = jobs;
            this.PilotSummarys = pilots;
        }
    }
}
