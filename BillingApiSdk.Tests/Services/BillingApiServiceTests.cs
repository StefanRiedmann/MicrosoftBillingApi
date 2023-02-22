using Xunit;
using System.Threading.Tasks;
using BillingApiSdk.Services;
using BillingApiSdk.Models;
using BillingApiSdk.Configuration;

namespace BillingApiSdk.Tests.Services
{

    public class BillingApiServiceTests
    {
        private const string MockSubscriptionId = "37f9dea2-4345-438f-b0bd-03d40d28c7e0";
        private const string MockOperationId = "74dfb4db-c193-4891-827d-eb05fbdc64b0";

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
                var result = await _fixture.GetSubscription(MockSubscriptionId);
                Assert.NotNull(result);
            }).GetAwaiter().GetResult();
        }

        [Fact]
        public void DeleteSubscription_MockApiTest()
        {
            Task.Run(async () =>
            {
                await _fixture.DeleteSubscription(MockSubscriptionId);
            }).GetAwaiter().GetResult();
        }

        [Fact]
        public void GetPendingOperations_MockApiTest()
        {
            Task.Run(async () =>
            {
                var result = await _fixture.GetPendingOperations(MockSubscriptionId);
                Assert.NotEmpty(result);
            }).GetAwaiter().GetResult();
        }

        [Fact]
        public void GetOperationStatus_MockApiTest()
        {
            Task.Run(async () =>
            {
                var result = await _fixture.GetOperationStatus(MockSubscriptionId, MockOperationId);
                Assert.NotNull(result);
            }).GetAwaiter().GetResult();
        }
    }
}
