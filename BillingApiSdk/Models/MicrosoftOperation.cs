using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace BillingApiSdk.Models
{
    public class MicrosoftOperation
    {
        //"id": 
        //"activityId": 
        //"subscriptionId": 
        //"offerId": "offer1",  // purchased offer ID
        //"publisherId": "contoso",  
        //"planId": "silver",  // purchased plan ID
        //"quantity": "20", // purchased amount of seats, will be empty is not relevant
        //"action": "Reinstate", 
        //"timeStamp": "2018-12-01T00:00:00",  // UTC
        //"status": "InProgress" // the only status that can be returned in this case

        /// <summary>
        /// "<guid>",  //Operation ID, should be provided in the operations patch API call
        /// </summary>
        [JsonProperty("id")]
        public string OperationId { get; set; }

        /// <summary>
        /// "<guid>", //not relevant
        /// </summary>
        [JsonProperty("activityId")]
        public string ActivityId { get; set; }

        /// <summary>
        /// "<guid>", // subscriptionId of the SaaS subscription that is being reinstated
        /// </summary>
        [JsonProperty("subscriptionId")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// purchased offer ID
        /// </summary>
        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        /// <summary>
        /// That's us
        /// </summary>
        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        /// <summary>
        /// purchased plan ID
        /// </summary>
        [JsonProperty("planId")]
        public string PlanId { get; set; }

        /// <summary>
        /// "20", // purchased amount of seats, will be empty if not relevant
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// When the SaaS subscription is in Subscribed status:
        /// - ChangePlan
        /// - ChangeQuantity
        /// - Suspend
        /// - Unsubscribe
        /// When SaaS subscription is in Suspended status:
        /// - Reinstate
        /// - Unsubscribe
        /// </summary>
        [JsonProperty("action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MicrosoftOperationAction Action { get; set; }

        [JsonProperty("timeStamp")]
        public DateTime TimeStampUtc { get; set; }

        /// <summary>
        /// Only available when calling GetOperationStatus
        /// Possible values: NotStarted, InProgress, Failed, Succeed, Conflict (new quantity / plan is the same as existing)
        /// </summary>
        public string Status { get; set; }

    }
}
