using System.Threading.Tasks;
using BillingApiSdk.Models;
using BillingApiSdk.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillingApiExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingApiService service;

        public BillingController(IBillingApiService service)
        {
            this.service = service;
        }

        [HttpGet("[action]")]
        public async Task<MicrosoftSubscription> ResolvePurchase([FromQuery] string billingToken)
        {
            var result = await service.ResolvePurchase(billingToken);
            var subsription = await service.GetSubscription(result.SubscriptionId);
            return subsription;
        }

        [HttpPost("[action]")]
        public async Task ActivateSubscription([FromQuery] string billingToken)
        {
            var purchase = await service.ResolvePurchase(billingToken);
            var subscription = await service.GetSubscription(purchase.SubscriptionId);
            // ToDo: Before activing the subscription on Microsoft side, you
            // have to configure your users account accordingly
            // ...

            await service.ActivateSubscription(subscription.SubscriptionId);
        }

        [HttpPost("[action]")]
        public async Task SubscriptionWebhook([FromBody] MicrosoftWebhookPayload payload)
        {
            // ToDo: Perform the changes described in the payload on your user's account
            // Alternative: Get the full operation for more details
            var operation = await service.GetOperationStatus(payload.SubscriptionId, payload.OperationId);

            await service.UpdateOperationStatus(payload.SubscriptionId, payload.OperationId, "Success");
        }
    }
}
