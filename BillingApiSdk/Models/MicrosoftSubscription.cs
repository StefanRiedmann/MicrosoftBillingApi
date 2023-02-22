using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BillingApiSdk.Models
{
    public class MicrosoftPurchase
    {
        /// <summary>
        /// purchased SaaS subscription ID 
        /// </summary>
        [JsonProperty("id")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// SaaS subscription name 
        /// </summary>
        [JsonProperty("subscriptionName")]
        public string SubscriptionName { get; set; }

        /// <summary>
        /// purchased offer ID
        /// </summary>
        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        /// <summary>
        /// purchased offer's plan ID
        /// </summary>
        [JsonProperty("planId")]
        public string PlanId { get; set; }

        /// <summary>
        /// number of purchased seats, might be empty if the plan is not per seat
        /// </summary>
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
    }

    public class MicrosoftSubscriptions
    {
        [JsonProperty("subscriptions")]
        public List<MicrosoftSubscription> Subscriptions { get; set; }
        [JsonProperty("@nextLink")]
        public string NextLink { get; set; }
    }

    public class MicrosoftSubscription
    {
        /// <summary>
        /// purchased SaaS subscription ID
        /// </summary>
        [JsonProperty("id")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// publisher ID
        /// </summary>
        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        /// <summary>
        /// purchased offer ID
        /// </summary>
        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        /// <summary>
        /// SaaS subscription name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Indicates the status of the operation: PendingFulfillmentStart, Subscribed, Suspended or Unsubscribed.
        /// </summary>
        [JsonProperty("saasSubscriptionStatus")]
        public string SaasSubscriptionStatus { get; set; }

        /// <summary>
        /// email address, user ID and tenant ID for which SaaS subscription is purchased.
        /// </summary>
        [JsonProperty("beneficiary")]
        public MicrosoftUser Beneficiary { get; set; }

        /// <summary>
        /// email address ,user ID and tenant ID that purchased the SaaS subscription.  
        /// These could be different from beneficiary information for reseller (CSP) scenario
        /// </summary>
        [JsonProperty("purchaser")]
        public MicrosoftUser Purchaser { get; set; }

        /// <summary>
        /// purchased plan ID
        /// </summary>
        [JsonProperty("planId")]
        public string PlanId { get; set; }

        /// <summary>
        /// purchased amount of seats, will be empty if plan is not per seat
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// The period for which the subscription was purchased. 
        /// </summary>
        [JsonProperty("term")]
        public MicrosoftTerm Term { get; set; }

        /// <summary>
        /// Indicating whether the subscription will renew automatically.
        /// </summary>
        [JsonProperty("autoRenew")]
        public bool AutoRenew { get; set; }

        /// <summary>
        /// not relevant
        /// </summary>
        [JsonProperty("isTest")]
        public bool IsTest { get; set; }

        /// <summary>
        /// true - the customer subscription is currently in free trial, 
        /// false - the customer subscription is not currently in free trial. 
        /// (Optional field -– if not returned, the value is false.)
        /// </summary>
        [JsonProperty("isFreeTrial")]
        public bool IsFreeTrial { get; set; }

        /// <summary>
        /// "Read", "Update", "Delete" 
        /// </summary>
        [JsonProperty("allowedCustomerOperations")]
        public List<string> AllowedCustomerOperations { get; set; }

        /// <summary>
        /// The ID of the session that created the subscription.
        /// </summary>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// The ID of the fulfillment operation that created the subscription.
        /// </summary>
        [JsonProperty("fulfillmentId")]
        public string FulfillmentId { get; set; }

        /// <summary>
        /// not relevant
        /// </summary>
        [JsonProperty("sandboxType")]
        public string SandboxType { get; set; }

        /// <summary>
        /// The date and time when the subscription was created.
        /// </summary>
        [JsonProperty("created")]
        public DateTime CreatedDateUtc { get; set; }

        /// <summary>
        /// not relevant
        /// </summary>
        [JsonProperty("sessionMode")]
        public string SessionMode { get; set; }
    }

    public class MicrosoftTerm
    {
        /// <summary>
        /// where P1M is monthly and P1Y is yearly. Also reflected in the startDate and endDate values
        /// E.g. P1M
        /// </summary>
        [JsonProperty("termUnit")]
        public string TermUnit { get; set; }
        /// <summary>
        /// format: YYYY-MM-DD. This is the date when the subscription was activated by the ISV and the billing started. 
        /// This field is relevant only for Active and Suspended subscriptions.
        /// E.g. 2019-05-31
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime StartDateUtc { get; set; }
        /// <summary>
        /// This is the last day the subscription is valid. Unless stated otherwise, the automatic renew will happen the next day. 
        /// This field is relevant only for Active and Suspended subscriptions.
        /// E.g. 2019-06-29
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime EndDateUtc { get; set; }
    }

    public class MicrosoftUser
    {
        [JsonProperty("emailId")]
        public string EmailId { get; set; }
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }
        /// <summary>
        /// Id of the user
        /// </summary>
        [JsonProperty("puid")]
        public string Puid { get; set; }
    }
}
