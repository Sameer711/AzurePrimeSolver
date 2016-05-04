namespace PrimeSolverRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrimeNumberCandidates",
                c => new
                    {
                        Number = c.Int(nullable: false),
                        IsPrime = c.Boolean(),
                    })
                .PrimaryKey(t => t.Number);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PrimeNumberCandidates");
        }
    }
}
