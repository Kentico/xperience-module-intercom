using System;

using CMS.Base;
using CMS.ContactManagement;

using Newtonsoft.Json.Linq;

namespace Kentico.Xperience.Intercom
{
    /// <summary>
    /// Event arguments for handler <see cref="IntercomUpdateContactHandler"/>.
    /// </summary>
    public class IntercomUpdateContactEventArgs : CMSEventArgs
    {
        /// <summary>
        /// JSON object containing all field values provided in the webhook request. Custom fields should be set to the <see cref="Contact"/>.
        /// </summary>
        public JObject ContactData { get; private set; }


        /// <summary>
        /// Contact that should be updated.
        /// </summary>
        public ContactInfo Contact { get; private set; }


        internal IntercomUpdateContactEventArgs(JObject contactData, ContactInfo contact)
        {
            ContactData = contactData ?? throw new ArgumentNullException(nameof(contactData));
            Contact = contact ?? throw new ArgumentNullException(nameof(contact));
        }
    }
}
