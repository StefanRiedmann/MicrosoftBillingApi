using Xunit;
using System.Threading.Tasks;
using BillingApiSdk.Services;
using BillingApiSdk.Models;
using BillingApiSdk.Configuration;

namespace BillingApiSdk.Tests.Services
{

    public class BillingApiServiceTests
    {
        /// <summary>
        /// I know I know, configuration has to be solved in a better way.
        /// You have two options
        /// - Write an Issue
        /// - Send me a pull request with the solution ;-) 
        /// </summary>
        private static BillingApiConfiguration configuration = new BillingApiConfiguration
        {
            ActivateMsMockAccount = true,
            MsMockAccount = "someone@abc.com"
        };

        private IBillingApiService _fixture;

        public BillingApiServiceTests()
        {
            _fixture = new BillingApiService(configuration);
        }

        [Fact]
        public void ResolveSubscription_MockApiTest()
        {
            Task.Run(async () =>
            {
                var result = await _fixture.ResolvePurchase("mock");
                Assert.NotNull(result);
            }).GetAwaiter().GetResult();
        }

        [Fact]
        public void GetAllSubscriptions_MockApiTest()
        {
            Task.Run(async () =>
            {
                var result = await _fixture.GetAllSubscriptions(true);
                Assert.NotNull(result);
            }).GetAwaiter().GetResult();
        }

        [Fact]
        public void GetSubscription_MockApiTest()
        {
            Task.Run(async () =>
            {
                // mock subscription id: 37f9dea2-4345-438f-b0bd-03d40d28c7e0
                var result = await _fixture.GetSubscription("37f9dea2-4345-438f-b0bd-03d40d28c7e0");
                Assert.NotNull(result);
            }).GetAwaiter().GetResult();
        }

        [Fact]
        public void GetPendingOperations_MockApiTest()
        {
            Task.Run(async () =>
            {
                // mock subscription id: 37f9dea2-4345-438f-b0bd-03d40d28c7e0
                var result = await _fixture.GetPendingOperations("37f9dea2-4345-438f-b0bd-03d40d28c7e0");
                Assert.NotEmpty(result);
            }).GetAwaiter().GetResult();
        }

        [Fact]
        public void GetOperationStatus_MockApiTest()
        {
            Task.Run(async () =>
            {
                // mock subscription id: 37f9dea2-4345-438f-b0bd-03d40d28c7e0
                var result = await _fixture.GetOperationStatus("37f9dea2-4345-438f-b0bd-03d40d28c7e0", "74dfb4db-c193-4891-827d-eb05fbdc64b0");
                Assert.NotNull(result);
            }).GetAwaiter().GetResult();
        }
    }
}
