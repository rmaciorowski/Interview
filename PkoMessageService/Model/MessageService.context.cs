using System.Data;
using System.Data.Entity;

namespace PkoMessageService.Model
{
    
	public class MessageService : DbContext
    {
        public MessageService()
            :base("name=MessageService") {}
	    public DbSet<MessageHistory> Messages { get; set; }
    }
}
