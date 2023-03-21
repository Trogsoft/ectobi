using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Trogsoft.Ectobi.Data
{
    public class EctoDb : DbContext
    {
        public EctoDb(DbContextOptions options) : base(options)
        {
        }

        protected EctoDb()
        {
        }

        public DbSet<Schema> Schemas { get; set; }
        public DbSet<SchemaField> SchemaFields { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Value> Values { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<ProcessElement> ProcessElements { get; set; }
        public DbSet<Populator> Populators { get; set; }

        public string GetTextId<TEntity>(string title) where TEntity : NamedEntity
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.-]");
            title = title.Trim();
            title = title.Replace(" ", "-"); // Replace all spaces with hyphens
            string tid = rgx.Replace(title, "-"); // Replace all non-alpha characters with hyphens
            tid = Regex.Replace(tid, "-{2,}", "-"); // Replace multiple hypens with one
            tid = tid.Trim('-'); // Remove any trailing hyphens
            int idx = 1; 
            // Check to see if the text ID exists. If it does, add a hyphen and an index number until it doesn't.
            while (Set<TEntity>().AsQueryable().Any(x => x.TextId == tid))
            {
                tid = rgx.Replace(title, "") + $"-{idx}";
                idx++;
            }
            return tid.ToLower(); // Return it as all lowercase. We're assuming the database collation is case insensitive.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Schema>(s =>
            {
                s.HasMany(x => x.SchemaFields).WithOne(x => x.Schema).HasForeignKey(x => x.SchemaId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Value>(v =>
            {
                v.HasOne(x => x.Record).WithMany(x => x.Values).HasForeignKey(x => x.RecordId).OnDelete(DeleteBehavior.Cascade);
                v.HasOne(x => x.SchemaField).WithMany().HasForeignKey(x => x.SchemaFieldId).OnDelete(DeleteBehavior.Restrict);
            });

        }

    }
}