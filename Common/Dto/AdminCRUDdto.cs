using System.ComponentModel.DataAnnotations;

namespace Common.Dto
{
    public class AdminCRUDdto

    {
        [Key]
        public short? AccountId { get; set; } 

        [Required]
        public string AccountName { get; set; }

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; }

        [Required]
        [Range(1, 3)]
        public int? AccountRole { get; set; }

        public string? AccountPassword { get; set; } 
    }

}
