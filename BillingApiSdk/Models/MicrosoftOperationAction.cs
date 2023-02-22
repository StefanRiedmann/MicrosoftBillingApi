namespace BillingApiSdk.Models
{
    public enum MicrosoftOperationAction
    {
        /// (When the resource has been deleted)
        /// <summary>
        /// The unsubscribe
        /// </summary>
        Unsubscribe,

        /// (When the change plan operation has completed)
        /// <summary>
        /// The change plan
        /// </summary>
        ChangePlan,

        /// (When the change quantity operation has completed),
        /// <summary>
        /// The change quantity
        /// </summary>
        ChangeQuantity,

        /// (When resource has been suspended)
        /// <summary>
        /// The suspend
        /// </summary>
        Suspend,

        /// (When resource has been reinstated after suspension)
        /// <summary>
        /// The reinstate
        /// </summary>
        Reinstate,

        /// (When resource has been reinstated after suspension)
        /// <summary>
        /// The reinstate
        /// </summary>
        Renew,

        /// <summary>
        /// The transfer
        /// </summary>
        Transfer,
    }
}
