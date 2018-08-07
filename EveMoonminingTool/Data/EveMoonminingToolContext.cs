using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EveMoonminingTool.Models
{
    public class EveMoonminingToolContext : DbContext
    {
        public EveMoonminingToolContext (DbContextOptions<EveMoonminingToolContext> options)
            : base(options)
        {
        }

        public DbSet<EveMoonminingTool.Models.MiningJob> MiningJob { get; set; }
    }
}
