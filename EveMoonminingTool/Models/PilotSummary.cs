using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EveMoonminingTool.Models
{
    public class PilotSummary
    {
        public string Pilot { get; set; }
        public List<MiningJob> Jobs;

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Value { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public float Volume { get; set; } = 0;

        public PilotSummary(string pilot, List<MiningJob> jobs)
        {
            this.Pilot = pilot;
            this.Jobs = jobs;
        }
    }
}