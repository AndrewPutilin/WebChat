using System;
using System.ComponentModel.DataAnnotations;

namespace WebChat.DAL.Domain
{
    public class CheckLiveBackUser
    {
        [Key]
        public int? Id { get; set; }
        [Required]
        public Chat Chat { get; set; }
        [Required]
        public string UserEmail { get; set; }       
        public DateTime? TimeLeave { get; set; }
        [Required]
        public DateTime TimeComeIn { get; set; }
    }
}
