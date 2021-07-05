using System.ComponentModel.DataAnnotations;

namespace WebChat.DAL.Domain
{
    public class Messаge
    {
        [Key]
        public int? Id { get; set; }
        [Required]
        public string MessageText { get; set; }
        [Required]
        public Chat Chat { get; set; }
        [Required]
        public string MesegeTime { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public bool CheckRead { get; set; }
    }
}
