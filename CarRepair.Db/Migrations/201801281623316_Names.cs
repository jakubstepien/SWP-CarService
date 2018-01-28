namespace CarRepair.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Names : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Names",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Surnames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            var names = new string[] { "Adrian", "Evan", "Jordan", "Andrew", "Max", "Brandon", "Tyler", "Jason", "Chase", "Adam", "Justin", "Robert", "Parker", "Brian", "Ken", "Gavin", "Richard", "Jeremy", "Matt", "Dominic", "Chase", "Zachary", "Luis", "Tristan", "Alexander", "Jack", "Charlie", "Luke", "Frank", "John", "Victor", "Daniel", "Jacob", "William", "Conrad", "Charles", "Pastor", "Samuel", "Lily", "Barbara", "Rose", "Cadence", "Eve", "Grace", "Vera", "Allison", "Sarah", "Lucy", "Alexis", "Taylor", "Gianna", "Alice", "Jasmine", "Sophie", "Lydia", "Claudia", "Mila", "Caroline", "Gabriella", "Audrey", "Madison", "Charlotte", "Mia", "Ava", "Emma", "Olivia", "Harper", "Camila", "Kelly", "Peggie", "Ellie", "Brooke", "Lynda", "Cindy", "Naomi", };
            foreach (var name in names)
            {
                Sql($"Insert into Names (Value) Values ('{name}')");
            }
            var surnames = new string[] { "Elev", "Rain", "Rasac", "Bein", "Smith", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson", "Martinez", "Anderson", "Taylor", "Thomas", "Hernandez", "Moore", "Martin", "Jackson", "Thompson", "White", "Obama", "Sulivan" };
            foreach (var name in surnames)
            {
                Sql($"Insert into Surnames (Value) Values ('{name}')");
            }
        }
        
        public override void Down()
        {
            DropTable("dbo.Surnames");
            DropTable("dbo.Names");
        }
    }
}
