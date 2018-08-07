using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EveMoonminingTool.Models;

namespace EveMoonminingTool.Controllers
{
    public class MiningJobsController : Controller
    {
        private readonly EveMoonminingToolContext _context;

        public MiningJobsController(EveMoonminingToolContext context)
        {
            _context = context;
        }

        // GET: MiningJobs
        public async Task<IActionResult> Index()
        {
            return View(await _context.MiningJob.ToListAsync());
        }

        // GET: MiningJobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miningJob = await _context.MiningJob
                .SingleOrDefaultAsync(m => m.ID == id);
            if (miningJob == null)
            {
                return NotFound();
            }

            return View(miningJob);
        }

        // GET: MiningJobs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MiningJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Day,Corp,Pilot,OreType,Amount,Volumen,EstimatedValue,OreID,SystemID")] MiningJob miningJob)
        {
            if (ModelState.IsValid)
            {
                _context.Add(miningJob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(miningJob);
        }     

        // GET: MiningJobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miningJob = await _context.MiningJob.SingleOrDefaultAsync(m => m.ID == id);
            if (miningJob == null)
            {
                return NotFound();
            }
            return View(miningJob);
        }

        // POST: MiningJobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Day,Corp,Pilot,OreType,Amount,Volumen,EstimatedValue,OreID,SystemID")] MiningJob miningJob)
        {
            if (id != miningJob.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(miningJob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MiningJobExists(miningJob.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(miningJob);
        }

        // GET: MiningJobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miningJob = await _context.MiningJob
                .SingleOrDefaultAsync(m => m.ID == id);
            if (miningJob == null)
            {
                return NotFound();
            }

            return View(miningJob);
        }

        // POST: MiningJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var miningJob = await _context.MiningJob.SingleOrDefaultAsync(m => m.ID == id);
            _context.MiningJob.Remove(miningJob);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MiningJobExists(int id)
        {
            return _context.MiningJob.Any(e => e.ID == id);
        }
    }
}
