using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BillingApiSdk.Configuration;
using BillingApiSdk.Models;

namespace BillingApiSdk.Services
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2?WT.mc_id=pc_24
    /// 
    /// https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/saas-fulfillment-apis-faq
    /// </summary>
    public interface IBillingApiService
    {
        /// <summary>
        /// The resolve endpoint enables the publisher to exchange the marketplace purchase identification token
        /// to a persistent purchased SaaS subscription ID and its details.
        /// </summary>
        Task<MicrosoftPurchase> ResolvePurchase(string marketplaceToken);

        /// <summary>
        /// Retrieves a list of all purchased SaaS subscriptions for all offers published by the publisher in marketplace. 
        /// SaaS subscriptions in all possible statuses will be returned. 
        /// Unsubscribed SaaS subscriptions are also returned, as this information is not deleted on Microsoft side.
        /// </summary>
        /// <param name="mock">To test it using the mocking api</param>
        Task<List<MicrosoftSubscription>> GetAllSubscriptions(bool mock = false);

        /// <summary>
        /// Retrieves a specified purchased SaaS subscription for a SaaS offer published in the marketplace by the publisher. 
        /// Use this call to get all available information for a specific SaaS subscription by its ID 
        /// rather than calling the API for getting list of all subscriptions.
        /// </summary>
        Task<MicrosoftSubscription> GetSubscription(string subscriptionId);

        /// <summary>
        /// Once the SaaS account is configured for an end customer, the publisher must call the Activate Subscription API on Microsoft side. 
        /// The customer will not be billed unless this API call is successful.
        /// </summary>
        Task ActivateSubscription(string subscriptionId);

        /// <summary>
        /// Get list of the pending operations for the specified SaaS subscription. Returned operations should be acknowledged 
        /// by the publisher by calling the Operation patch API.
        /// Currently only Reinstate operations are returned as response for this API call.
        /// </summary>
        Task<List<MicrosoftOperation>> GetPendingOperations(string subscriptionId);

        /// <summary>
        /// Enables the publisher to track the status of the specified async operation: Unsubscribe, ChangePlan, or ChangeQuantity.
        /// The operationId for this API call can be retrieved from the value returned by Operation-Location, get pending operations API call, 
        /// or the<id> parameter value received in a webhook call.
        /// </summary>
        Task<MicrosoftOperation> GetOperationStatus(string subscriptionId, string operationId);

        /// <summary>
        /// Update the status of a pending operation to indicate the operation's success or failure 
        /// on the publisher side.
        /// </summary>
        /// <param name="status">
        /// Allowed Values: Success/Failure. Indicates the status of the operation on ISV side.
        /// </param>
        Task UpdateOperationStatus(string subscriptionId, string operationId, string status);
    }

    public class BillingApiService : IBillingApiService
    {
        private const string mockApiVersion = "2018-09-15";
        private const string productionApiVersion = "2018-08-31";
        private BillingApiConfiguration config;

        public BillingApiService(BillingApiConfiguration config)
        {
            this.config = config;
        }

        public async Task TestAzureDirectoryToken()
        {
            await GetAllSubscriptions();
        }

        public async Task<MicrosoftPurchase> ResolvePurchase(string marketplaceToken)
        {
            var api = GetApiVersion(marketplaceToken, null);
            using var httpClient = await GetMsSaasSubscriptionClient(null, null, !api.isMock, marketplaceToken);
            //Post https://marketplaceapi.microsoft.com/api/saas/subscriptions/resolve?api-version=<ApiVersion>
            var uri = new Uri($"https://marketplaceapi.microsoft.com/api/saas/subscriptions/resolve?api-version={api.apiVersion}");
            var msg = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            var response = await httpClient.SendAsync(msg);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"ResolveSubscription {response.StatusCode} - {responseString}");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<MicrosoftPurchase>(responseString);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deserializaing MicrosoftSubscription", ex);
            }
        }

        public async Task<List<MicrosoftSubscription>> GetAllSubscriptions(bool mock = false)
        {
            var apiVersion = mock ? mockApiVersion : productionApiVersion;
            using var httpClient = await GetMsSaasSubscriptionClient(null, null, !mock);
            var link = $"https://marketplaceapi.microsoft.com/api/saas/subscriptions/?api-version={apiVersion}";

            var result = new List<MicrosoftSubscription>();
            while (!string.IsNullOrEmpty(link))
            {
                var uri = new Uri(link);
                var msg = new HttpRequestMessage()
                {
                    RequestUri = uri,
                    Method = HttpMethod.Get
                };
                var response = await httpClient.SendAsync(msg);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetAllSubscriptions {response.StatusCode} - {responseString}");
                }

                try
                {
                    var list = JsonConvert.DeserializeObject<MicrosoftSubscriptions>(responseString);
                    result.AddRange(list.Subscriptions);
                    link = list.NextLink;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Error deserializaing list of MicrosoftSubscription, link: {link}", ex);
                }
            }
            if (mock)
            {
                result.ForEach(r =>
                {
                    r.Beneficiary.EmailId = config.MsMockAccount;
                    r.PlanId = "moondeskteam";
                    r.IsFreeTrial = true;
                });
            }
            return result;
        }

        public async Task<MicrosoftSubscription> GetSubscription(string subscriptionId)
        {
            var api = GetApiVersion(null, subscriptionId);
            using var httpClient = await GetMsSaasSubscriptionClient(null, null, !api.isMock);
            var uri = new Uri($"https://marketplaceapi.microsoft.com/api/saas/subscriptions/{subscriptionId}?api-version={api.apiVersion}");

            var msg = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            var response = await httpClient.SendAsync(msg);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"GetSubscription {response.StatusCode} - {responseString}");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<MicrosoftSubscription>(responseString);
                if (api.isMock)
                {
                    result.Beneficiary.EmailId = config.MsMockAccount;
                    result.PlanId = "moondeskteam";
                    result.IsFreeTrial = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deserializaing MicrosoftSubscription", ex);
            }
        }

        public async Task ActivateSubscription(string subscriptionId)
        {
            // Post https://marketplaceapi.microsoft.com/api/saas/subscriptions/<subscriptionId>/activate?api-version=<ApiVersion>
            // There is no mock api, and no response body
            var uri = new Uri($"https://marketplaceapi.microsoft.com/api/saas/subscriptions/{subscriptionId}/activate?api-version={productionApiVersion}");
            using var httpClient = await GetMsSaasSubscriptionClient(null, null);
            var msg = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            var response = await httpClient.SendAsync(msg);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"ActivateSubscription {response.StatusCode} - {responseString}");
            }
        }

        public async Task<List<MicrosoftOperation>> GetPendingOperations(string subscriptionId)
        {
            var api = GetApiVersion(null, subscriptionId);
            using var httpClient = await GetMsSaasSubscriptionClient(null, null, !api.isMock);
            var uri = new Uri($"https://marketplaceapi.microsoft.com/api/saas/subscriptions/{subscriptionId}/operations?api-version={api.apiVersion}");

            var msg = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            var response = await httpClient.SendAsync(msg);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"GetPendingOperations {response.StatusCode} - {responseString}");
            }
            try
            {
                var result = JsonConvert.DeserializeObject<List<MicrosoftOperation>>(responseString);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deserializaing list of MicrosoftOperations", ex);
            }
        }

        public async Task<MicrosoftOperation> GetOperationStatus(string subscriptionId, string operationId)
        {
            var api = GetApiVersion(null, subscriptionId);
            using var httpClient = await GetMsSaasSubscriptionClient(null, null, !api.isMock);
            var uri = new Uri($"https://marketplaceapi.microsoft.com/api/saas/subscriptions/{subscriptionId}/operations/{operationId}?api-version={api.apiVersion}");

            var msg = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            var response = await httpClient.SendAsync(msg);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"GetPendingOperations {response.StatusCode} - {responseString}");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<MicrosoftOperation>(responseString);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deserializaing MicrosoftOperation", ex);
            }
        }

        public async Task UpdateOperationStatus(string subscriptionId, string operationId, string status)
        {
            // There is no mock api, and no response body
            var uri = new Uri($"https://marketplaceapi.microsoft.com/api/saas/subscriptions/{subscriptionId}/operations/{operationId}?api-version={productionApiVersion}");
            using var httpClient = await GetMsSaasSubscriptionClient(null, null);
            var msg = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            var response = await httpClient.SendAsync(msg);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"UpdateOperationStatus {response.StatusCode} - {responseString}");
            }
        }

        /// <summary>
        /// Does not support mocking api
        /// </summary>
        /// <param name="status">Success or Failure</param>
        /// <returns></returns>
        public async Task UpdateOperation(string subscriptionId, string operationId, string status)
        {
            using var httpClient = await GetMsSaasSubscriptionClient(null, null);
            var uri = new Uri($"https://marketplaceapi.microsoft.com/api/saas/subscriptions/{subscriptionId}/operations/{operationId}?api-version={productionApiVersion}");

            var bodyStr = $"{{\"status\":\"{status}\"}}";
            var content = new StringContent(bodyStr);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await httpClient.PostAsync(uri, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"UpdateOperation {response.StatusCode} - {responseString}");
            }
        }

        private (string apiVersion, bool isMock) GetApiVersion(string token, string subscriptionId)
        {
            var mock = config.ActivateMsMockAccount;
            if (!string.IsNullOrEmpty(token))
            {
                mock = mock && token == "mock";
            }
            else if (!string.IsNullOrEmpty(subscriptionId))
            {
                mock = mock && subscriptionId == "37f9dea2-4345-438f-b0bd-03d40d28c7e0"; // The mock api works with this id
            }
            else
            {
                mock = mock && config.ClientId == null;
            }
            var apiVersion = mock ? mockApiVersion : productionApiVersion;
            return (apiVersion, mock);
        }

        /// <summary>
        /// Get a client with the corresponding headers
        /// </summary>
        /// <param name="msRequestId">A unique string value for tracking the request from the client, preferably a GUID. 
        /// If this value isn't provided, one will be generated and provided in the response headers.</param>
        /// <param name="msCorrelationId">A unique string value for operation on the client. This parameter correlates all events from client operation 
        /// with events on the server side. If this value isn't provided, one will be generated and provided in the response headers.</param>
        /// <param name="getToken">If true, performs a GET against Azure ActiveDirectory to get a token which is then used for authorization header 
        /// "Bearer <access_token>" when the token value is retrieved by the publisher as explained in Get a token based on the Azure AD app.</param>
        /// <returns></returns>
        private async Task<HttpClient> GetMsSaasSubscriptionClient(string msRequestId, string msCorrelationId, bool getToken = true, string marketplaceToken = null)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(msRequestId))
            {
                httpClient.DefaultRequestHeaders.Add("x-ms-requestid", msRequestId);
            }

            if (!string.IsNullOrWhiteSpace(msCorrelationId))
            {
                httpClient.DefaultRequestHeaders.Add("x-ms-correlationid", msCorrelationId);
            }

            if (getToken)
            {
                var token = await GetAzureActiveDirectoryToken();
                httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");
            }

            if (!string.IsNullOrWhiteSpace(marketplaceToken))
            {
                httpClient.DefaultRequestHeaders.Add("x-ms-marketplace-token", marketplaceToken);
            }
            return httpClient;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-registration#get-the-token-with-an-http-post
        /// </summary>
        private async Task<string> GetAzureActiveDirectoryToken()
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var url = $"https://login.microsoftonline.com/{config.TenantId}/oauth2/token";
            var bodyStr = $"grant_type=client_credentials" +
                            $"&client_id={config.ClientId}" +
                            $"&client_secret={config.ClientSecret}" +
                            $"&resource=20e940b3-4c77-4b0b-9a53-9e16a1b010a7"; //Marketplace SaaS API is always the target resource in this case
            var content = new StringContent(bodyStr);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"GetAzureActiveDirectoryToken {response.StatusCode} - {responseString}");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<MicrosoftTokenResponse>(responseString);
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deserializaing MicrosoftTokenResponse", ex);
            }
        }
    }
}
