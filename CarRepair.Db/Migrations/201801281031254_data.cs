namespace CarRepair.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class data : DbMigration
    {
        public override void Up()
        {
            Sql("Insert into Faults(Name) Values ('doors')");
            Sql("Insert into Faults(Name) Values ('engine')");
            Sql("Insert into Faults(Name) Values ('tires')");

            Sql("Insert into Brands(BrandName) Values ('Fiat')");
            Sql("Insert into Brands(BrandName) Values ('Skoda')");
            Sql("Insert into Brands(BrandName) Values ('Toyota')");
            Sql("Insert into Brands(BrandName) Values ('Renault')");

           
        }
        
        public override void Down()
        {
        }
    }
}
