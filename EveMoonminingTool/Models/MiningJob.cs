using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EveMoonminingTool.Models
{
    public class MiningJob
    {

        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Day (yyyy-MM-dd)")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Day { get; set; }

        [Required]
        public String Corp { get; set; }

        [Required]
        public String Pilot { get; set; }

        [Required]
        public String OreType { get; set; }

        [Required]
        public int Amount { get; set; }

        public float Volumen { get; set; }
        public int EstimatedValue { get; set; }
        public int OreID { get; set; }
        public int SystemID { get; set; }

        public MiningJob()
        {

        }

            public MiningJob (String[] werte)
        {
            
            // This constructor is for generating a Mining job Objects from the Strings that are parsed from the initialy posted text
            // It has to convert the strings into the specific data types
            
             

            String[] datum = werte[0].Split('.');
            //public DateTime(int year, int month, int day);
            this.Day = new DateTime(int.Parse(datum[0]), int.Parse(datum[1]), int.Parse(datum[2]));

            this.Corp = werte[1];
            this.Pilot = werte[2];
            this.OreType = werte[3];
            this.Amount = int.Parse(werte[4]);
            this.Volumen = float.Parse(werte[5]);
            this.EstimatedValue = int.Parse(werte[6]);
            this.OreID = int.Parse(werte[7]);
            this.SystemID = int.Parse(werte[8]);
        }
    }
}
