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
    public class PaginatedListViewModel<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
    }

    public class BasePaginatedListDto<T>
    {
        public IReadOnlyCollection<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class ResponseDto<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Result { get; set; }
    }


}
