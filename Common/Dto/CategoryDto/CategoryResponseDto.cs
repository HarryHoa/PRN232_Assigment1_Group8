using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.CategoryDto
{
    public class CategoryResponseDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public bool? IsActive { get; set; }
    }
}
