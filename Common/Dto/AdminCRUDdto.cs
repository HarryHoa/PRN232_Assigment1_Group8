using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class AdminCRUDdto

    {
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
