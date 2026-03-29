using MediatR;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application.Features.Users.GetUser
{
    public sealed record GetAllUserCommand() : IRequest<List<ApplicationUser>>;
}
