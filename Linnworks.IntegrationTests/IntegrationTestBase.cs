using LinnworksTest.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Linnworks.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected static CategoriesManagementContext GivenLinnworksIntegrationContext(bool beginTransaction = true)
        {
            var context = new CategoriesManagementContext(new DbContextOptionsBuilder<CategoriesManagementContext>()
                .UseSqlServer(LinnworksIntegration.ConnectionString).Options);
            return context;
        }

        private static SqlConnectionStringBuilder LinnworksIntegration =>
            new SqlConnectionStringBuilder
            {
                DataSource = @"EPRUPETW0864\SQLEXPRESS",
                InitialCatalog = "Linnworks.IntegrationTests",
                IntegratedSecurity = true
            };
    }
}