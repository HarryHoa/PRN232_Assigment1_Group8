using System.ComponentModel.DataAnnotations;

namespace PRNN232_Assigment1_FE.Models
{
    public class SystemAccountModel
    {

        public short AccountId { get; set; }
        [Required(ErrorMessage = "AccountName is required.")]

        public string AccountName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@FUNewsManagement\.org$",
                    ErrorMessage = "Email must be in the format @FUNewsManagement.org")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "AccountRole is required.")]
        [Range(1, 2, ErrorMessage = "AccountRole must be either 1 (Admin) or 2 (Staff).")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string AccountPassword { get; set; }
    }
}
