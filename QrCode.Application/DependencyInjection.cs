using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            try
            {

                services.AddMediatR(cfr =>
                {
                    cfr.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(),
                       typeof(BaseEntity).Assembly);
                    //cfr.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
                });
            }
            catch(Exception ex)
            {
                var aa = ex.ToString();
            }
            return services;
        
        }
        
    }
}
