using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Test
{
    public class TestBase
    {

        public TestBase()
        {
            this.PathToTestFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ectobi", "Tests");
            this.PathToEctoDb = Path.Combine(PathToTestFolder, "ecto.db");

            if (!Directory.Exists(PathToTestFolder))
                Directory.CreateDirectory(PathToTestFolder);
        }

        [SetUp]
        public virtual void Setup()
        {
            File.Delete(PathToEctoDb);
        }

        protected string PathToTestFolder { get; }
        protected string PathToEctoDb { get; }

        private bool dbRequested = false;
        private EctoDb? db;

        protected EctoDb GetDatabase()
        {
            var opts = new DbContextOptionsBuilder<EctoDb>().UseSqlite($"Data Source={PathToEctoDb}").Options;
            this.db = new EctoDb(opts);
            db.Database.EnsureCreated();
            dbRequested = true;
            return db;
        }

        [TearDown]
        protected virtual void TearDown()
        {
            if (dbRequested)
            {
                db?.Database.EnsureDeleted();
                db?.Database.CloseConnection();
                File.Delete(PathToEctoDb);
            }
        }

    }
}
