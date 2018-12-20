namespace PkoMessageService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstOne : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageHistories",
                c => new
                    {
                        MessageHistoryId = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false),
                        EmailSubject = c.String(nullable: false),
                        EmailBody = c.String(nullable: false),
                        SentDateTime = c.DateTime(nullable: false),
                        ErrorCode = c.Int(),
                        ErrorMessage = c.String(),
                    })
                .PrimaryKey(t => t.MessageHistoryId);
            
        }
        
        public override void Down()
        {
        }
    }
}
