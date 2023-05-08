using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Trogsoft.Ectobi.Data
{
    public class EctoDb : IdentityDbContext<EctoUser, EctoRole, Guid>
    {
        public EctoDb(DbContextOptions options) : base(options)
        {
        }

        protected EctoDb()
        {
        }

        public virtual DbSet<Schema> Schemas { get; set; }
        public virtual DbSet<SchemaField> SchemaFields { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Value> Values { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<ProcessElement> ProcessElements { get; set; }
        public virtual DbSet<Populator> Populators { get; set; }
        public virtual DbSet<SchemaVersion> SchemaVersions { get; set; }
        public virtual DbSet<SchemaFieldVersion> SchemaFieldVersions { get; set; }
        public virtual DbSet<LookupSet> LookupSets { get; set; }
        public virtual DbSet<LookupSetValue> LookupSetValues { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<Stage> Stages { get; set; }
        public virtual DbSet<WebHookEvent> WebHookEvents { get; set; }
        public virtual DbSet<WebHook> WebHooks {  get; set; }
        public virtual DbSet<UserRefreshToken> RefreshTokens { get; set; } 
        public virtual DbSet<Model> Models { get; set; }

        public string GetTextId<TEntity>(string title, Func<TEntity, bool> qualifier = null!) where TEntity : NamedEntity
        {
            Regex rgx = new Regex("[^a-zA-Z0-9.]");
            title = title.Trim();
            title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title); // Make it title case so that all words are appropriately capitalised
            title = title.Replace(" ", ""); // Replace all spaces with hyphens
            string tid = rgx.Replace(title, ""); // Replace all non-alpha characters with hyphens
            int idx = 1; 
            // Check to see if the text ID exists. If it does, add a hyphen and an index number until it doesn't.
            while (Set<TEntity>().AsQueryable().Any(x => x.TextId == tid))
            {
                tid = rgx.Replace(title, "") + $"{idx}";
                idx++;
            }
            return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(tid);
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
                v.HasOne(x => x.SchemaFieldVersion).WithMany().HasForeignKey(x => x.SchemaFieldVersionId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SchemaFieldVersion>(x =>
            {
                x.HasOne(x => x.SchemaVersion).WithMany(x => x.Fields).HasForeignKey(x => x.SchemaVersionId).OnDelete(DeleteBehavior.Cascade);
                x.HasOne(x => x.SchemaField).WithMany().HasForeignKey(x => x.SchemaFieldId).OnDelete(DeleteBehavior.Restrict);
            });

        }

    }
}