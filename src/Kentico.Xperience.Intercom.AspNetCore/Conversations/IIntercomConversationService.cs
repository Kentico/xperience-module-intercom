using System.Threading.Tasks;

using CMS;
using CMS.ContactManagement;
using CMS.DataEngine;

using Kentico.Xperience.Intercom;

[assembly: RegisterImplementation(typeof(IIntercomConversationService), typeof(IntercomConversationService), Lifestyle = CMS.Core.Lifestyle.Singleton, Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace Kentico.Xperience.Intercom
{
    public interface IIntercomConversationService
    {
        Task<string> GetConversationHistory(ContactInfo contact, SiteInfoIdentifier siteIdentifier);
    }
}
