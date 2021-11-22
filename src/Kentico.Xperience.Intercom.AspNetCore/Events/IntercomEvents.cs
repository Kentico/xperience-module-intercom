namespace Kentico.Xperience.Intercom
{
    public static class IntercomEvents 
    {
        /// <summary>
        /// Fires when the contact is updated through the Intercom webhook endpoint. This event should be handled to fill contact's custom fields.
        /// </summary>
        public static IntercomUpdateContactHandler UpdateContact = new IntercomUpdateContactHandler { Name = "IntercomEvents.UpdateContact" };
    }
}
