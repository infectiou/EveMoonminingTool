﻿using System;
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

        // We need this List too cache out parsed data, because the list can not be transferred back to the controller after showing it in the view
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
        [HttpGet]
        public IActionResult Summary(float corpTribute = 20)
        {
            ViewData["corpTribute"] = corpTribute/100;
            
            // Use LINQ to get list of Corps.
            IQueryable<string> corpQuery = from m in _context.MiningJob
                                           orderby m.Corp
                                           select m.Corp;
            List<string> corps = corpQuery.Distinct().ToList();

            //Create a list of summarys, one for each Corp
            List<CorpSummary> summaryList = new List<CorpSummary>();

            foreach (string c in corps)
            {
                //get a List of all Pilot summarys for that Corp
                List<PilotSummary> pilotSummarys = getPilotSummarys(c);               

                //calculate value and volume and create corp joblist
                int oreValueSum = 0;
                float oreVolumeSum = 0;
                List<MiningJob> joblist = new List<MiningJob>();
                foreach (PilotSummary pj in pilotSummarys)
                {                    
                    oreValueSum = oreValueSum + pj.Value;
                    oreVolumeSum = oreVolumeSum + pj.Volume;
                    joblist.AddRange(pj.Jobs);
                }

                //create summary and add it to the list
                CorpSummary summary = new CorpSummary(c, joblist, pilotSummarys);
                summary.Value = oreValueSum;
                summary.Volume = oreVolumeSum;

                summaryList.Add(summary);
            }

            return View(summaryList);
        }

        // Helper to create the summary for each pilot of a certain corp
        private List<PilotSummary> getPilotSummarys(string corp)
        {
            //filter for corp's jobs
            var jobsSubSet = from m in _context.MiningJob
                             select m;

            jobsSubSet = jobsSubSet.Where(x => x.Corp == corp);

            // Use LINQ to get list of pilots.
            IQueryable<string> pilotQuery = from m in jobsSubSet
                                            orderby m.Pilot
                                            select m.Pilot;
            List<string> pilots = pilotQuery.Distinct().ToList();

            //Create a list of summarys, one for each pilot
            List<PilotSummary> summaryList = new List<PilotSummary>();

            foreach (string p in pilots)
            {
                //filter for pilot's jobs
                var jobs = from m in jobsSubSet
                           select m;

                jobs = jobsSubSet.Where(x => x.Pilot == p);
                List<MiningJob> joblist = jobs.ToList();

                //calculate value and volume
                int oreValueSum = 0;
                float oreVolumeSum = 0;
                foreach (MiningJob j in joblist)
                {
                    oreValueSum = oreValueSum + j.EstimatedValue;
                    oreVolumeSum = oreVolumeSum + j.Volumen;
                }

                //create summary and add it to the list
                PilotSummary summary = new PilotSummary(p, joblist);
                summary.Value = oreValueSum;
                summary.Volume = oreVolumeSum;

                summaryList.Add(summary);
            }

            return summaryList;
        }

        // GET: /<controller>/
        // Obsolete
        public IActionResult CorpSummary()
        {
            // Use LINQ to get list of Corps.
            IQueryable<string> corpQuery = from m in _context.MiningJob
                                           orderby m.Corp
                                           select m.Corp;
            List<string> corps = corpQuery.Distinct().ToList();

            //Create a list of summarys, one for each Corp
            List<CorpSummary> summaryList = new List<CorpSummary>();

            foreach (string c in corps)
            {
                //filter for corp's jobs
                var jobs = from m in _context.MiningJob
                           select m;

                jobs = jobs.Where(x => x.Corp == c);
                List<MiningJob> joblist = jobs.ToList();

                //calculate value and volume
                int oreValueSum = 0;
                float oreVolumeSum = 0;
                foreach (MiningJob j in joblist)
                {
                    oreValueSum = oreValueSum + j.EstimatedValue;
                    oreVolumeSum = oreVolumeSum + j.Volumen;
                }

                //create summary and add it to the list
                CorpSummary summary = new CorpSummary(c, joblist, null);
                summary.Value = oreValueSum;
                summary.Volume = oreVolumeSum;

                summaryList.Add(summary);
            }

            return View(summaryList);
        }

        // GET: /<controller>/
        // Obsolete
        public IActionResult PilotSummary()
        {
            // Use LINQ to get list of pilots.
            IQueryable<string> pilotQuery = from m in _context.MiningJob
                                            orderby m.Pilot
                                            select m.Pilot;
            List<string> pilots = pilotQuery.Distinct().ToList();

            //Create a list of summarys, one for each pilot
            List<PilotSummary> summaryList = new List<PilotSummary>();

            foreach (string p in pilots)
            {
                //filter for pilot's jobs
                var jobs = from m in _context.MiningJob
                           select m;

                jobs = jobs.Where(x => x.Pilot == p);
                List<MiningJob> joblist = jobs.ToList();

                //calculate value and volume
                int oreValueSum = 0;
                float oreVolumeSum = 0;
                foreach (MiningJob j in joblist)
                {
                    oreValueSum = oreValueSum + j.EstimatedValue;
                    oreVolumeSum = oreVolumeSum + j.Volumen;
                }

                //create summary and add it to the list
                PilotSummary summary = new PilotSummary(p, joblist);
                summary.Value = oreValueSum;
                summary.Volume = oreVolumeSum;

                summaryList.Add(summary);
            }

            return View(summaryList);
        }

        //// GET: /<controller>/
        //public IActionResult Test()
        //{
        //    return RedirectToAction("Index", "MoonMining", new { message = "Dinkleberg!" });
        //}

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
                return RedirectToAction("Summary");
            }
            return RedirectToAction("Index", "MoonMining", new { message = "There seem to have been an Error. The Data couldn't be written to the database" });
        }
    }
}