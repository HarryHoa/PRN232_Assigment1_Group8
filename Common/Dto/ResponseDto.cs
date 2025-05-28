using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Dto
{
   public class ResponseDto
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } 
        public object?  Result  { get; set; }
        public override string ToString() => JsonSerializer.Serialize(this);

        public ResponseDto(int statusCode, string? message, bool isSuccess, object? result)
        {
            StatusCode = statusCode;
            Message = message;
            IsSuccess = isSuccess;
            Result = result;
        }
    }
}
