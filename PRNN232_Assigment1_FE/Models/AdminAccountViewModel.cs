using Newtonsoft.Json;
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
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@FUNewsManagement\.org$", ErrorMessage = "Email must be in the format *@FUNewsManagement.org")]
        public string AccountEmail { get; set; }
        [Required]
        public string? AccountPassword { get; set; }
        [Required]
        public int AccountRole { get; set; }
    }
    public class ODataResponse<T>
    {
        [JsonProperty("@odata.count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public T Value { get; set; }
    }



}
