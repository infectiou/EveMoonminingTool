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

        private static List<MiningJob> JobCollection = new List<MiningJob>();

        public MoonMiningController(EveMoonminingToolContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index(string message = "Please put the Mining Data into the field below")
        {
            ViewData["Message"] = message;            

            return View();
        }

        // GET: /<controller>/
        public IActionResult Test()
        {
            return RedirectToAction("Index", "MoonMining", new { message = "Dinkleberg!" });
        }

        public IActionResult Parse(string data)
        {
            //initianlizing counters and error handling
            ViewData["LineCount"] = 0;
            ViewData["ErrorCount"] = 0;
            ViewData["ErrorMessage"] = "There were useless data in Lines: ";
            ViewData["ErrorList"] = new List<int>();

            if (data == null | data == "")
            {
                JobCollection = new List<MiningJob>();
                return View(JobCollection);
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
            JobCollection = new List<MiningJob>();

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

        // POST: MoonMining/AddToDatabase
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoDatabase()
        {
            if (ModelState.IsValid)
            {
                _context.AddRange(JobCollection);
                await _context.SaveChangesAsync();
                JobCollection = new List<MiningJob>();
                return RedirectToAction("Index", "MiningJobs");
            }
            return RedirectToAction("Index", "MoonMining", new { message = "There seem to have been an Error. The Data couldn't be written to the database" });
        }
    }
}