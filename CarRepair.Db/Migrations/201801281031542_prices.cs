namespace CarRepair.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prices : DbMigration
    {
        public override void Up()
        {
            var price = 100;
            var brndNum = 4;
            var faultNum = 3;
            for (int i = 1; i <= brndNum; i++)
            {
                for (int j = 1; j <= faultNum; j++)
                {
                    Sql($"Insert into Prices(Value,IdFault,IdBrand) Values ({price}, {j}, {i})");
                    price += 100;
                }
            }
        }
        
        public override void Down()
        {
        }
    }
}
