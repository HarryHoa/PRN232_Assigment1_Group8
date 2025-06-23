using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Interface
{
    
        public interface IAdminCrudAccountService
        {
            IQueryable<AdminCRUDdto> GetAllForOdata();
            Task<ResponseDto> GetAllAsync();
            Task<ResponseDto> GetByIdAsync(short id);
            Task<ResponseDto> CreateAsync(AdminCRUDdto dto);
            Task<ResponseDto> UpdateAsync(short id, AdminCRUDdto dto);
            Task<ResponseDto> DeleteAsync(short id);
            Task<ResponseDto> SearchAsync(string keyword);
       
        }


    


}
