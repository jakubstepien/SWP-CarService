namespace CarRepair.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initalcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BrandName = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.BrandName, unique: true);
            
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductionYear = c.String(nullable: false, maxLength: 4),
                        IdClient = c.Int(nullable: false),
                        IdBrand = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.IdBrand, cascadeDelete: false)
                .ForeignKey("dbo.Clients", t => t.IdClient, cascadeDelete: false)
                .Index(t => t.IdClient)
                .Index(t => t.IdBrand);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pin = c.String(nullable: false, maxLength: 5),
                        Name = c.String(nullable: false, maxLength: 64),
                        Surname = c.String(nullable: false, maxLength: 64),
                        Phone = c.String(nullable: false, maxLength: 9),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Pin);
            
            CreateTable(
                "dbo.Faults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdFault = c.Int(nullable: false),
                        IdBrand = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.IdBrand, cascadeDelete: false)
                .ForeignKey("dbo.Faults", t => t.IdFault, cascadeDelete: false)
                .Index(t => t.IdFault)
                .Index(t => t.IdBrand);
            
            CreateTable(
                "dbo.Repairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdClient = c.Int(nullable: false),
                        IdFault = c.Int(),
                        RepairDate = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdCar = c.Int(nullable: false),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cars", t => t.IdCar, cascadeDelete: false)
                .ForeignKey("dbo.Clients", t => t.IdClient, cascadeDelete: false)
                .ForeignKey("dbo.Faults", t => t.IdFault)
                .Index(t => t.IdClient)
                .Index(t => t.IdFault)
                .Index(t => t.IdCar);            
        }

        public override void Down()
        {
            DropForeignKey("dbo.Repairs", "IdFault", "dbo.Faults");
            DropForeignKey("dbo.Repairs", "IdClient", "dbo.Clients");
            DropForeignKey("dbo.Repairs", "IdCar", "dbo.Cars");
            DropForeignKey("dbo.Prices", "IdFault", "dbo.Faults");
            DropForeignKey("dbo.Prices", "IdBrand", "dbo.Brands");
            DropForeignKey("dbo.Cars", "IdClient", "dbo.Clients");
            DropForeignKey("dbo.Cars", "IdBrand", "dbo.Brands");
            DropIndex("dbo.Repairs", new[] { "IdCar" });
            DropIndex("dbo.Repairs", new[] { "IdFault" });
            DropIndex("dbo.Repairs", new[] { "IdClient" });
            DropIndex("dbo.Prices", new[] { "IdBrand" });
            DropIndex("dbo.Prices", new[] { "IdFault" });
            DropIndex("dbo.Faults", new[] { "Name" });
            DropIndex("dbo.Clients", new[] { "Pin" });
            DropIndex("dbo.Cars", new[] { "IdBrand" });
            DropIndex("dbo.Cars", new[] { "IdClient" });
            DropIndex("dbo.Brands", new[] { "BrandName" });
            DropTable("dbo.Repairs");
            DropTable("dbo.Prices");
            DropTable("dbo.Faults");
            DropTable("dbo.Clients");
            DropTable("dbo.Cars");
            DropTable("dbo.Brands");
        }
    }
}
