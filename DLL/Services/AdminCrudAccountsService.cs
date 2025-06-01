using AutoMapper;
using Common.Dto;
using DAL.Models;
using DAL.Repository;
using DLL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Services
{
    public class AdminCrudAccountService : IAdminCrudAccountService
    {
        private readonly IGenericRepository<SystemAccount> _repo;
        private readonly IMapper _mapper;

        public AdminCrudAccountService(IGenericRepository<SystemAccount> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                var entities = await _repo.GetAllAsync();
                var result = _mapper.Map<List<AdminCRUDdto>>(entities);
                return new ResponseDto(200, "Success", true, result);
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"Error: {ex.Message}", false, null);
            }
        }

        public async Task<ResponseDto> GetByIdAsync(short id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return new ResponseDto(404, "Account not found", false, null);

                var result = _mapper.Map<AdminCRUDdto>(entity);
                return new ResponseDto(200, "Success", true, result);
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"Error: {ex.Message}", false, null);
            }
        }




        public async Task<ResponseDto> CreateAsync(AdminCRUDdto dto)
        {
            try
            {
                var entity = _mapper.Map<SystemAccount>(dto);
                await _repo.InsertAsync(entity);
                await _repo.SaveAsync();

                var result = _mapper.Map<AdminCRUDdto>(entity);
                return new ResponseDto(201, "Account created successfully", true, result);
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"Error: {ex.Message}", false, null);
            }
        }


        public async Task<ResponseDto> UpdateAsync(short id, AdminCRUDdto dto)
        {
            try
            {
                var account = await _repo.GetByIdAsync(id);
                if (account == null)
                    return new ResponseDto(404, "Account not found", false, null);

                _mapper.Map(dto, account);

                await _repo.UpdateAsync(account);
                await _repo.SaveAsync();

                var result = _mapper.Map<AdminCRUDdto>(account);
                return new ResponseDto(200, "Account updated successfully", true, result);
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"Error: {ex.Message}", false, null);
            }
        }



        public async Task<ResponseDto> DeleteAsync(short id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);

                if (entity == null)
                    return new ResponseDto(404, "Account not found", false, null);

                await _repo.DeleteAsync(id);
                await _repo.SaveAsync();

                return new ResponseDto(200, "Account deleted successfully", true, null);
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"Error: {ex.Message}", false, null);
            }
        }



        public async Task<ResponseDto> SearchAsync(string keyword)
        {
            try
            {
                List<SystemAccount> entities;


                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Nếu từ khóa rỗng thì trả về tất cả
                    entities = (await _repo.GetAllAsync()).ToList();
                }
                else
                {
                    // Tìm tài khoản có tên hoặc email chứa keyword (case insensitive)
                    entities = (await _repo.GetAllAsync(q =>
                        q.Where(a => a.AccountName.Contains(keyword) || a.AccountEmail.Contains(keyword)))).ToList();
                }


                var result = _mapper.Map<List<AdminCRUDdto>>(entities);

                if (result == null || result.Count == 0)
                    return new ResponseDto(404, "No accounts found", false, null);

                return new ResponseDto(200, "Search successful", true, result);
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"Error: {ex.Message}", false, null);
            }
        }


    }
}
