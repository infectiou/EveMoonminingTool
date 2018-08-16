﻿using System;
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

        public List<TaxedJob> TJobs;

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TValue { get; set; } = 0;

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public float Volume { get; set; } = 0;

        public PilotSummary(string pilot, List<TaxedJob> jobs)
        {
            this.Pilot = pilot;
            this.TJobs = jobs;
        }     

    }
}