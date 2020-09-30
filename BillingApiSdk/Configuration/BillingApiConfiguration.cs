namespace BillingApiSdk.Configuration
{
    public class BillingApiConfiguration
    {
        /// <summary>
        /// True for a test environment, to be able to test your 
        /// implementation without a real marketplace purchase
        /// </summary>
        public bool ActivateMsMockAccount { get; set; }
        /// <summary>
        /// If you use 'mock' as a billingToken, this is the
        /// simulated user that purchased your service.
        /// </summary>
        public string MsMockAccount { get; set; }
        /// <summary>
        /// Could be hard coded. The mock api works with this
        /// subscription id.
        /// </summary>
        public string MsMockSubscriptionId { get; set; }
        /// <summary>
        /// Your TenantId from ActiveDirectory. Needed when your not mocking.
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// Your ClientId from ActiveDirectory. Needed when your not mocking.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Your ClientSecret from ActiveDirectory. Needed when your not mocking.
        /// </summary>
        public string ClientSecret { get; set; }
    }
}
