using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.CategoryDTO
{
    public class CategoryCreateDto
    {
        public string CategoryName { get; set; }
        public string CategoryDesciption { get; set; }
        public short? ParentCategoryId { get; set; }
        // Không cần IsActive ở đây vì mặc định sẽ là true
    }

}
