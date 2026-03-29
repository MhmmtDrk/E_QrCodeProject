using Microsoft.EntityFrameworkCore;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.AggregateModels.Users.Events;
using QrCode.Infrastructure.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Infrastructure.Service
{
    internal class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetAllUser()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
    }
}
