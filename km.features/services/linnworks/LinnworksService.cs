using Microsoft.EntityFrameworkCore;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Service;
using System.Data.SqlClient;

namespace LinnworksTest.Features.services.linnworks
{
    public class LinnworksService : ServiceBase
    {
        public string AuthToken = "bccf905c-6592-40f2-8db1-c976791fa40a";

        public LinnworksSeleniumContext Context;

        public LinnworksNavigate Navigate { get; }
        public LinnworksScenarios Scenarios { get; }
        public Browser Browser => Context.Browser;

        public LinnworksService(LinnworksSeleniumContext context, BaseUrlInfo defaultBaseUrlInfo,
            BaseUrlPattern baseUrlPattern, Router router)
            : base(defaultBaseUrlInfo, baseUrlPattern, router)
        {
            Context = context;
            Scenarios = new LinnworksScenarios(this);
            Navigate = new LinnworksNavigate(this);
        }

        public LinnworksContext GetLinnworksContext()
        {
            var context = new LinnworksContext(new DbContextOptionsBuilder<LinnworksContext>()
                .UseSqlServer(LinnworksIntegration.ConnectionString).Options);
            return context;
        }

        private static SqlConnectionStringBuilder LinnworksIntegration =>
            new SqlConnectionStringBuilder
            {
                DataSource = @"EPRUPETW0864\SQLEXPRESS",
                InitialCatalog = "Linnworks.TestDb",
                IntegratedSecurity = true
            };
    }
}