using MediatR;

using Microsoft.AspNetCore.Identity;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.SeedWork;

namespace QrCode.Application.Features.Users.CreateUser
{
    public  class CreateUserCommandHandler:IRequestHandler<CreateUserCommand,IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            //business kontrolleri yapabilirsin
            //await _userService.CreateAsync(request.UserName,
            //    request.Email,
            //    request.Password,                
            //    cancellationToken);
            //await _UnitOfWork.SaveChangesAsync(cancellationToken);
            var user = new ApplicationUser
            {
                Name = request.Name,
                Email = request.Email,
                UserName=request.UserName,
                
                
               
               
                // Diğer kullanıcı özellikleri buraya eklenir
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            return result;
        }
    }
}
  
