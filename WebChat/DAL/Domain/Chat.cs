using System.ComponentModel.DataAnnotations;

namespace WebChat.DAL.Domain
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Users { get; set; }
    }
}
