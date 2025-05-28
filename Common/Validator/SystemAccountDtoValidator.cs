using Common.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Validator

{
   public class SystemAccountDtoValidator : AbstractValidator<SystemAccountDto>
    {
        public SystemAccountDtoValidator()
        {
            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("Account name is required")
                .MaximumLength(100).WithMessage("Account name must not exceed 100 characters");

            RuleFor(x => x.AccountEmail)
                .NotEmpty().WithMessage("Email is required")
                .Must(BeValidFunewsEmail)
                .WithMessage("Email must be a valid address under FUNewsManagement.org domain");

            RuleFor(x => x.AccountPassword)
                .NotEmpty().WithMessage("Password is required");
       

            RuleFor(x => x.AccountRole)
                .Must(role => role == null || new[] { 1, 2 }.Contains(role.Value))
                .WithMessage("AccountRole must be either 1 (Admin) or 2 (Staff)");
        }

        private bool BeValidFunewsEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@FUNewsManagement\.org$";
            return Regex.IsMatch(email, pattern);
        }
    }

}
