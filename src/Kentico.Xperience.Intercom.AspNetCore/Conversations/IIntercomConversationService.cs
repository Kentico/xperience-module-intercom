using System.Threading.Tasks;

using CMS;
using CMS.ContactManagement;
using CMS.DataEngine;

using Kentico.Xperience.Intercom;

[assembly: RegisterImplementation(typeof(IIntercomConversationService), typeof(IntercomConversationService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.Intercom
{
    /// <summary>
    /// Defines service that retrieves contact's conversation history from the Intercom.
    /// </summary>
    public interface IIntercomConversationService
    {
        /// <summary>
        /// Returns the conversation history of the <paramref name="contact"/> on the provided site (<paramref name="siteIdentifier"/>).
        /// </summary>
        /// <param name="contact">Contact to download conversation history for.</param>
        /// <param name="siteIdentifier">Site where the conversation occurred.</param>
        Task<string> GetConversationHistory(ContactInfo contact, SiteInfoIdentifier siteIdentifier);
    }
}
