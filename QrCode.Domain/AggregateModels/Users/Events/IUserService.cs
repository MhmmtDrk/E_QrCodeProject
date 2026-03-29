using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.Users.Events
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllUser();
    }
}
