using System;
using System.ComponentModel.DataAnnotations;

namespace PkoMessageService.Model
{
    public class MessageHistory
    {
        [Key]
        public int MessageHistoryId { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string EmailSubject { get; set; }

        [Required]
        public string EmailBody { get; set; }

        [Required]
        public DateTime SentDateTime { get; set; }

        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
