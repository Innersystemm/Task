using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlToDB.Models
{
    public class TypedContext: DbContext
    {
        public TypedContext(string connectionString): base(connectionString)
        {
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new DbInitializer());
        }

        public DbSet<Items> Items { get; set; }
        public DbSet<AvailableItems> AvailableItems { get; set; }
        public DbSet<Manufacturers> Manufacturers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class DbInitializer : CreateDatabaseIfNotExists<TypedContext>
    {
        protected override void Seed(TypedContext context)
        {
            context.Manufacturers.AddRange(new List<Manufacturers>()
            {
                new Manufacturers() {ManufacturerID = 0, ManufacturerName = "mName0"},
                new Manufacturers() {ManufacturerID = 1, ManufacturerName = "mName1"},
                new Manufacturers() {ManufacturerID = 2, ManufacturerName = "mName2"}
            });
            context.Items.AddRange(new List<Items>()
            {
                new Items(){ItemID = 0, ItemName = "iName0", Manufacturer = 0, Price = 100},
                new Items(){ItemID = 1, ItemName = "iName1", Manufacturer = 1, Price = 1100},
                new Items(){ItemID = 2, ItemName = "iName2", Manufacturer = 2, Price = 1200}
            });
            context.AvailableItems.AddRange(new List<AvailableItems>()
            {
                new AvailableItems(){Id = 0, ItemID = 0, ItemsCount = 1221},
                new AvailableItems(){Id = 1, ItemID = 1, ItemsCount = 1321},
                new AvailableItems(){Id = 2, ItemID = 2, ItemsCount = 1221},
                new AvailableItems(){Id = 3, ItemID = 0, ItemsCount = 11121},
            });
            base.Seed(context);
        }
    }
}
