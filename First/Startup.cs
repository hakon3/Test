using First.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(First.Startup))]

namespace First
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ITableService, PublicApiLogService>();
            builder.Services.AddTransient<IBlobService, BlobService>();
            builder.Services.AddTransient<IPayLoadBuilder, PayLoadBuilder>();
            builder.Services.AddSingleton<ITableClientFactory, TableClientFactory>();
        }
    }
}
