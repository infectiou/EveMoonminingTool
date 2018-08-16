using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EveMoonminingTool.Models
{
    public class TaxedJob : MiningJob
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Taxed Amount")]
        public int TAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Taxed Value")]
        public int TValue { get; set; }

        public TaxedJob(MiningJob job, float tax)
        {
            base.ID = job.ID;
            base.Day = job.Day;
            base.Corp = job.Corp;
            base.Pilot = job.Pilot;
            base.OreType = job.OreType;
            base.Amount = job.Amount;
            base.Volumen = job.Volumen;
            base.EstimatedValue = job.EstimatedValue;
            base.OreID = job.OreID;
            base.SystemID = job.SystemID;
           
            float x = base.Amount * (1 - (tax / 100));
            this.TAmount = (int)x;
            float y = base.EstimatedValue * (1 - (tax / 100));
            this.TValue = (int)y;
        }

        public TaxedJob()
        {
            
        }
    }
}