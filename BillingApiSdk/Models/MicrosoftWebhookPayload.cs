using Newtonsoft.Json;
using System;

namespace BillingApiSdk.Models
{
    // EXAMPLES... 

    // end customer changed a quantity of purchased seats for a plan on Microsoft side
    //{
    //    "id": < guid >, // this is the operation ID to call with get operation API
    //    "activityId": "<guid>", // do not use
    //    "subscriptionId": "guid", // The GUID identifier for the SaaS resource which status changes
    //    "publisherId": "contoso", // A unique string identifier for each publisher
    //    "offerId": "offer1", // A unique string identifier for each offer
    //    "planId": "silver", // the most up-to-date plan ID
    //    "quantity": " 25", // the most up-to-date number of seats, can be empty if not relevant
    //    "timeStamp": "2019-04-15T20:17:31.7350641Z", // UTC time when the webhook was called
    //    "action": "ChangeQuantity", // the operation the webhook notifies about
    //    "status": "Success" // Can be either InProgress or Success  
    //}

    // end customer's payment instrument became valid again, after being suspended, and the SaaS subscription is being reinstated
    //{
    //    "id": < guid >, 
    //    "activityId": < guid >, 
    //    "subscriptionId": "guid", 
    //    "publisherId": "contoso",
    //    "offerId": "offer2 ",
    //    "planId": "gold", 
    //    "quantity": " 20", 
    //    "timeStamp": "2019-04-15T20:17:31.7350641Z",
    //    "action": "Reinstate",
    //    "status": "In Progress"
    //}

    public class MicrosoftWebhookPayload
    {
        /// <summary>
        /// this is the operation ID to call with get operation API
        /// </summary>
        [JsonProperty("id")]
        public string OperationId { get; set; }

        /// <summary>
        /// do not use
        /// </summary>
        [JsonProperty("activityId")]
        public string ActivityId { get; set; }

        /// <summary>
        /// The GUID identifier for the SaaS resource which status changes
        /// </summary>
        [JsonProperty("subscriptionId")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// A unique string identifier for each publisher
        /// </summary>
        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        /// <summary>
        /// A unique string identifier for each offer
        /// </summary>
        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        /// <summary>
        /// the most up-to-date plan ID
        /// </summary>
        [JsonProperty("planId")]
        public string PlanId { get; set; }

        /// <summary>
        /// the most up-to-date number of seats, can be empty if not relevant
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// UTC time when the webhook was called
        /// </summary>
        [JsonProperty("timeStamp")]
        public DateTime TimestampUtc { get; set; }

        /// <summary>
        /// When the SaaS subscription is in Subscribed status:
        /// - ChangeQuantity
        /// - Suspend
        /// - Unsubscribe
        /// When SaaS subscription is in Suspended status:
        /// - Reinstate
        /// - Unsubscribe
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// Can be either InProgress or Success
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
