using System.ComponentModel.DataAnnotations;

namespace PRNN232_Assigment1_FE.Models
{
    public class AdminAccountViewModel
    {
        [Required]
        public short? AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; }
        [Required]
        public string? AccountPassword { get; set; }
        [Required]
        public int AccountRole { get; set; }
    }



}
