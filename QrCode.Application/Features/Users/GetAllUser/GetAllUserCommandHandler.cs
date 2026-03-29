using MediatR;
using QrCode.Application.Features.Tags.GetUserTag;
using QrCode.Application.Features.Users.GetUser;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags.Events;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.AggregateModels.Users.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Users.GetAllUser
{
    public class GetAllUserCommandHandler : IRequestHandler<GetAllUserCommand, List<ApplicationUser>>
    {
        private readonly IUserService _userService;
        public GetAllUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<List<ApplicationUser>> Handle(GetAllUserCommand request, CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUser();
            return users;

        }
    }
}
