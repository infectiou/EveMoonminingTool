using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EveMoonminingTool.Models;
using System.Text.RegularExpressions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EveMoonminingTool.Controllers
{
    public class MoonMiningController : Controller
    {
        private readonly EveMoonminingToolContext _context;

        public MoonMiningController(EveMoonminingToolContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;

            return View();
        }        

        public IActionResult Parse(string data)
        {
            //initianlizing counters
            ViewData["LineCount"] = 0;            

            if (data == null | data == "")
            {                
                return View();
            }
            
            // We split the whole dump into seperate lines and count them,
            // with each line representing an entry for day, pilot and ore, they are seperated by new lines
            char[] delimiterChars = {'\n'};
            string[] lines = data.Split(delimiterChars);    // Each Line from the mining ledger dump with the first possibly being useless
            
            ViewData["LineCount"] = lines.Length;
            ViewData["Message"] = "Der Dump hatte " + ViewData["LineCount"] + " Zeilen";

            // Split each entry into it's single data points
            // There are supposed to be 9 Data Points per entry, seperated by tabs:
            string[][] entrys = new string[lines.Length][];

            //for (int i = 0; i < lines.Length; i++)
            //{                
            //    entrys[i] = lines[i].Split('\t');
            //}


            // Strip each line of the HTML and try to convert the entrys into a Mining Job
            List<MiningJob> JobCollection = new List<MiningJob>();
            ViewData["ErrorCount"] = 0;
            ViewData["ErrorMessage"] = "There were useless data in Lines: ";            
            ViewData["ErrorList"] = new List<int>();


            for (int i = 0; i < entrys.Length; i++)
            {
                entrys[i] = lines[i].Split('\t');
                string[] entry = entrys[i];
                ViewData["EntryCount" + i] = entry.Length;

                // Strip
                for (int j = 0; j < entry.Length; j++)
                {
                    entry[j] = Regex.Replace(entry[j], "<.*?>", String.Empty);
                    ViewData["Line" + i + ";" + j] = entry[j]; //nur zu testzwecken
                }

                MiningJob job = null;
                // Try to convert
                try
                {
                    job = new MiningJob(entry);
                }                             
                catch
                {
                    ViewData["ErrorCount"] = (int)ViewData["ErrorCount"] + 1;
                    ViewData["ErrorMessage"] = (string)ViewData["ErrorMessage"] + i +", ";
                    List<int> errorlist = (List<int>)ViewData["ErrorList"];
                    errorlist.Add(i);
                    ViewData["ErrorList"] = errorlist;
                }
                if (job != null)
                {
                    JobCollection.Add(job);
                }
            }

            ViewData["ListLenght"] = JobCollection.Count();

            return View(JobCollection);
        }
    }
}