using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EveMoonminingTool.Models
{
    public class SummaryViewModel
    {
        public List<CorpSummary> CropSummarys { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy.MM.dd}", ApplyFormatInEditMode = true)]        
        public DateTime StartDay { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy.MM.dd}", ApplyFormatInEditMode = true)]        
        public DateTime EndDay { get; set; }

        public float Tax { get; set; }

        public SummaryViewModel(DateTime sDay, DateTime eDay, float tax, List<CorpSummary> summarys)
        {
            this.CropSummarys = summarys;
            this.Tax = tax;
            this.StartDay = sDay;
            this.EndDay = eDay;
        }

    }
}