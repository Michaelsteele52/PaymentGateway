using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Data;

namespace PaymentGateway.Testing
{
    public class TestServiceProvider
    {
        public static IServiceProvider GetDatabaseContext()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PaymentsContext>(opt => opt.UseInMemoryDatabase(databaseName: "TestPaymentsContext"),
                ServiceLifetime.Scoped,
                ServiceLifetime.Scoped);

            return services.BuildServiceProvider();
        }

        public static IServiceProvider GetTestCache()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MemoryCache>();

            return services.BuildServiceProvider();
        }
    }
}
