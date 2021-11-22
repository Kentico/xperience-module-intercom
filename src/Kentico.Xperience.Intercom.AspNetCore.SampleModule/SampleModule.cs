using CMS;
using CMS.DataEngine;

using Kentico.Xperience.Intercom.SampleModule;

[assembly: AssemblyDiscoverable]

[assembly: RegisterModule(typeof(SampleModule))]

namespace Kentico.Xperience.Intercom.SampleModule
{
    /// <summary>
    /// Represents sample module that handles <see cref="IntercomEvents.UpdateContact"/> event to configure custom fields.
    /// </summary>
    public class SampleModule : Module
    {
        public const string NAME = "Kentico.Xperience.Intercom.Sample";


        /// <summary>
        /// Initializes a new instance of the <see cref="SampleModule"/> class.
        /// </summary>
        public SampleModule() : base(NAME)
        {
        }


        protected override void OnInit()
        {
            base.OnInit();

            IntercomEvents.UpdateContact.Execute += UpdateContactCustomFields;
        }


        private static void UpdateContactCustomFields(object sender, IntercomUpdateContactEventArgs e)
        {
            var contact = e.Contact;

            if (e.ContactData.TryGetValue("ContactCafeOwner", out var ownerValue))
            {
                contact.SetValue("ContactCafeOwner", (string)ownerValue);
            }

            if (e.ContactData.TryGetValue("ContactCoffeePreference", out var coffeePreference))
            {
                contact.SetValue("ContactCoffeePreference", (string)coffeePreference);
            }
        }
    }
}