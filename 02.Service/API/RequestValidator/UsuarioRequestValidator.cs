using FluentValidation;
using Infrastructure.Dto;

namespace CasaAhorro_Back.RequestValidator;

public class UsuarioRequestValidator : AbstractValidator<UserDto>
{
    public UsuarioRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("El campo Nombre no puede ser vació");
        ;
        RuleFor(x => x.Email).NotEmpty()
            .WithMessage("El campo Correo no puede ser vació");
    }
}
