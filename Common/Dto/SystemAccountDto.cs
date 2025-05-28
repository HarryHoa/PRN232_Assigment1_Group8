using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
  public  class SystemAccountDto
    {
        public short AccountId { get; set; }

        public string AccountName { get; set; }

        public string AccountEmail { get; set; }

        public int? AccountRole { get; set; }

        public string AccountPassword { get; set; }
    }
}
