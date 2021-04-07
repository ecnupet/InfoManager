using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    public class InfoContext :DbContext
    {
        public InfoContext(DbContextOptions<InfoContext> options) : base(options)
        {

        }
        public DbSet<RoomProcess> RoomProcesses { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<ChargeProject> ChargeProjects { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
