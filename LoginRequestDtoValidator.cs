using FluentValidation;

// DTO sınıfınızın adı neyse <LoginRequestDto> yerine onu yazabilirsiniz
public record LoginRequestDto(string Email, string Password);
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta alanı boş bırakılamaz.")
            .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre alanı boş bırakılamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.");
    }
}