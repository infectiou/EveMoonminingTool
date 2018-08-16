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

        // We need this List to cache out parsed data, because the list can not be transferred back to the controller after showing it in the view
        private static List<MiningJob> JobCollection = new List<MiningJob>();        

        public MoonMiningController(EveMoonminingToolContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index(string message = "Please put the Mining Data into the field below")
        {
            ViewData["Message"] = message;            
            ViewData["LastWeek"] = DateTime.Now.AddDays(-7);
            ViewData["Tomorrow"] = DateTime.Now.AddDays(1);

            return View();
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Summary(DateTime startDay, DateTime endDay, float corpTribute = 20)
        {
            /******************
            //Create a list of summarys, one for each Corp
            //Each corp summary contains a summary for each of it's member pilots
            *******************/
            List<CorpSummary> summaryList = new List<CorpSummary>();

            //convert to TaxedJob Type for the view and group it by Corps
            var tjobs = from m in _context.MiningJob
                        where ((m.Day >= startDay) && (m.Day <= endDay))
                        select new TaxedJob(m, corpTribute);
            
            var corpgroups = from job in tjobs
                             group job by job.Corp;

            foreach (var corpgroup in corpgroups)  //crate the corp summarys           
            {
                string corp = corpgroup.Key;               

                //Create a list of summarys, one for each pilot of this corp
                List<PilotSummary> pilotSummaryList = new List<PilotSummary>();

                var jobsByPilot = from m in corpgroup                                  
                                  group m by m.Pilot;

                foreach (var pilotJobs in jobsByPilot) //create the pilot summarys
                {
                    string pilot = pilotJobs.Key;

                    //create summary and add it to the list
                    PilotSummary psummary = new PilotSummary(pilot, pilotJobs.ToList());
                    psummary.TValue = pilotJobs.Sum(j => j.TValue);
                    psummary.Volume = pilotJobs.Sum(j => j.Volumen);
                    pilotSummaryList.Add(psummary);
                }                

                //calculate value and volume and create corp joblist                
                List<TaxedJob> corpJoblist = new List<TaxedJob>();                

                var jobsByOre = from m in corpgroup
                                group m by m.OreType;

                foreach (var oregroup in jobsByOre)
                {
                    string ore = oregroup.Key;
                    //TaxedJob orejob = new TaxedJob();
                    TaxedJob orejob = new TaxedJob() {
                                     Day = oregroup.First().Day,
                                     Pilot = corp,
                                     OreType = ore,
                                     TAmount = oregroup.Sum(x => x.TAmount),
                                     TValue = oregroup.Sum(x => x.TValue),
                                     Volumen = oregroup.Sum(x => x.Volumen),
                                     Amount = oregroup.Sum(x => x.Amount),
                                     EstimatedValue = oregroup.Sum(x => x.EstimatedValue),
                                 };
                    corpJoblist.Add(orejob);
                }

                //create summary and add it to the list
                CorpSummary summary = new CorpSummary(corp, corpJoblist, pilotSummaryList);
                summary.TValue = corpJoblist.Sum(j => j.TValue);
                summary.Volume = corpJoblist.Sum(j => j.Volumen);

                summaryList.Add(summary);
            }

            SummaryViewModel summaryView = new SummaryViewModel(startDay, endDay, corpTribute, summaryList);

            return View(summaryView);
        }

        // GET: /<controller>/
        //obsolete
        [HttpGet]
        public IActionResult SummaryOld(float corpTribute = 20)
        {
            ViewData["Tax"] = corpTribute;
            //Create a list of summarys, one for each Corp
            List<CorpSummary> summaryList = new List<CorpSummary>();

            // Use LINQ to get list of Corps. (old version)
            //iqueryable<string> corpquery = from m in _context.miningjob
            //                               orderby m.corp
            //                               select m.corp;
            //list<string> corps = corpquery.distinct().tolist();

            //group the jobs by corps
            var groupresult = from job in _context.MiningJob
                              group job by job.Corp;            

            foreach(var corpgroup in groupresult)
            //foreach (string c in corps)
            {
                string c = corpgroup.Key;
                //get a List of all Pilot summarys for that Corp
                List<PilotSummary> pilotSummarys = getPilotSummarys(c, corpTribute);              

                //calculate value and volume and create corp joblist
                int oreValueSum = 0;
                float oreVolumeSum = 0;
                List<TaxedJob> joblist = new List<TaxedJob>();
                foreach (PilotSummary pj in pilotSummarys)
                {                    
                    oreValueSum = oreValueSum + pj.TValue;
                    oreVolumeSum = oreVolumeSum + pj.Volume;
                    joblist.AddRange(pj.TJobs);
                }

                //create summary and add it to the list
                CorpSummary summary = new CorpSummary(c, joblist, pilotSummarys);
                summary.TValue = oreValueSum;
                summary.Volume = oreVolumeSum;

                summaryList.Add(summary);
            }

            return View(summaryList);
        }

        // Helper to create the summary for each pilot of a certain corp
        private List<PilotSummary> getPilotSummarys(string corp, float corpTribute = 20)
        {
            //Create a list of summarys, one for each pilot
            List<PilotSummary> pilotSummaryList = new List<PilotSummary>();
           

            /* 
            //jobsSubSet = jobsSubSet.Where(x => x.Corp == corp);
            /// Use LINQ to get list of pilots.
            //IQueryable<string> pilotQuery = from m in jobsSubSet
            //                                orderby m.Pilot
            //                                select m.Pilot;
            //List<string> pilots = pilotQuery.Distinct().ToList();
            */
            
            //filter for corp's jobs
            var jobsByPilot = from m in _context.MiningJob
                              where m.Corp == corp
                              group m by m.Pilot;

            foreach (var pilotJobs in jobsByPilot)
            {
                string pilot = pilotJobs.Key;
                //filter for pilot's jobs
                var jobs = from m in pilotJobs
                           //where m.Pilot == pilot
                           select new TaxedJob(m, corpTribute);

                //jobs = jobsSubSet.Where(x => x.Pilot == p);
                //List<MiningJob> joblist = jobs.ToList();

                //apply tax and calculate value and volume
                //List<TaxedJob> taxedJobList = new List<TaxedJob>();
                //int oreValueSum = 0;
                //float oreVolumeSum = 0;
                //foreach (TaxedJob j in jobs)
                //{
                //    //TaxedJob tj = new TaxedJob(j, corpTribute);
                //    oreValueSum = oreValueSum + j.TValue;
                //    oreVolumeSum = oreVolumeSum + j.Volumen;
                //    //taxedJobList.Add(j);
                //}               

                //create summary and add it to the list
                PilotSummary summary = new PilotSummary(pilot, jobs.ToList());
                summary.TValue = jobs.Sum(j => j.TValue);
                summary.Volume = jobs.Sum(j => j.Volumen);

                pilotSummaryList.Add(summary);
            }

            return pilotSummaryList;
        }

        // GET: /<controller>/
        // Obsolete
        public IActionResult CorpSummary(float corpTribute = 20)
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

                //create taxed joblist and calculate value and volume
                int oreValueSum = 0;
                float oreVolumeSum = 0;
                List<TaxedJob> tjobs = new List<TaxedJob>();
                foreach (MiningJob j in joblist)
                {
                    TaxedJob tj = new TaxedJob(j, corpTribute);
                    oreValueSum = oreValueSum + tj.TValue;
                    oreVolumeSum = oreVolumeSum + tj.Volumen;
                    tjobs.Add(tj);
                }

                //create summary and add it to the list
                CorpSummary summary = new CorpSummary(c, tjobs, null);
                summary.TValue = oreValueSum;
                summary.Volume = oreVolumeSum;

                summaryList.Add(summary);
            }

            return View(summaryList);
        }

        // GET: /<controller>/
        // Obsolete
        public IActionResult PilotSummary(float corpTribute = 20)
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

                //create taxed joblist and calculate value and volume
                int oreValueSum = 0;
                float oreVolumeSum = 0;
                List<TaxedJob> taxedJobList = new List<TaxedJob>();
                foreach (MiningJob j in joblist)
                {
                    TaxedJob tj = new TaxedJob(j, corpTribute);
                    oreValueSum = oreValueSum + tj.TValue;
                    oreVolumeSum = oreVolumeSum + tj.Volumen;
                    taxedJobList.Add(tj);
                }

                //create summary and add it to the list
                PilotSummary summary = new PilotSummary(p, taxedJobList);
                summary.TValue = oreValueSum;
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
                ViewData["Message"] = "No Data processed. Input was empty or NULL (how is that even possible?)";
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

                //Check for duplicates and add only if there is no entry for that Day, Pilot and Ore Type
                foreach (MiningJob mJob in JobCollection)
                {
                    MiningJob dBjob = _context.MiningJob
                                      .SingleOrDefault(m => (m.Day == mJob.Day)
                                                      && (m.Corp == mJob.Corp)
                                                      && (m.Pilot == mJob.Pilot)
                                                      && (m.OreType == mJob.OreType)
                                                       );
                    if (dBjob == default(MiningJob))
                    {
                        _context.Add(mJob);
                    }
                    else
                    {
                        dBjob.Amount = mJob.Amount;
                        dBjob.Volumen = mJob.Volumen;
                        dBjob.EstimatedValue = mJob.EstimatedValue;
                        _context.Update(dBjob);
                    }

                }

                //_context.AddRange(JobCollection);
                await _context.SaveChangesAsync();

                //Clear the joblist after the work is done
                JobCollection = new List<MiningJob>();
                return RedirectToAction("Summary");
            }
            return RedirectToAction("Index", "MoonMining", new { message = "There seem to have been an Error. The Data couldn't be written to the database" });
        }
    }
}