using System;
using System.Collections.Generic;
using System.Linq;

using CMS.ContactManagement;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;

using Newtonsoft.Json.Linq;

namespace Kentico.Xperience.Intercom
{
    internal static class ContactFieldMapper
    {
        private static readonly Lazy<Dictionary<string, int?>> modifiableStringFields = new Lazy<Dictionary<string, int?>>()
        {
            Value =
            {
                // "ContactEmail" is handled separately due to additional logic
                { "ContactFirstName", 100 },
                { "ContactMiddleName", 100 },
                { "ContactLastName", 100 },
                { "ContactJobTitle", 50 },
                { "ContactAddress1", 100 },
                { "ContactCity", 100 },
                { "ContactZIP", 100 },
                { "ContactMobilePhone", 26 },
                { "ContactBusinessPhone", 26 },
                { "ContactNotes", null },
                { "ContactCompanyName", 100 }
            }
        };


        /// <summary>
        /// Maps supported default contact fields from <paramref name="contactData"/> to the <paramref name="contact"/>.
        /// </summary>
        public static void MapDefaultContactFields(JObject contactData, ContactInfo contact)
        {
            var contactEmail = contactData.Value<string>("ContactEmail");
            if (!String.IsNullOrEmpty(contactEmail) && ValidationHelper.IsEmail(contactEmail, true))
            {
                if (!IsEmailTaken(contactEmail, contact))
                {
                    contact.ContactEmail = contactEmail;
                }
            }

            foreach (var fieldLengthPair in modifiableStringFields.Value)
            {
                MapStringField(fieldLengthPair.Key, fieldLengthPair.Value, contactData, contact);
            }
        }


        /// <summary>
        /// Checks if provided <paramref name="contactEmail"/> belongs to any contact/user/customer other than <paramref name="contact"/>.
        /// </summary>
        private static bool IsEmailTaken(string contactEmail, ContactInfo contact)
        {
            var emailTaken = false;

            var existingContactID = ContactInfoProvider.GetContactIDByEmail(contactEmail);
            if (existingContactID != 0 && contact.ContactID != existingContactID)
            {
                emailTaken = true;
            }

            if (!emailTaken)
            {
                var existingUser = UserInfo.Provider.Get().WhereEquals("Email", contactEmail).TopN(1).FirstOrDefault();

                if (existingUser != null)
                {
                    var userRelatedContactID = ContactMembershipInfoProvider.GetContactIDByMembership(existingUser.UserID, MemberTypeEnum.CmsUser);

                    emailTaken = contact.ContactID != userRelatedContactID;
                }
            }

            if (!emailTaken)
            {
                var existingCustomer = CustomerInfo.Provider.Get().WhereEquals("CustomerEmail", contactEmail).TopN(1).FirstOrDefault();

                if (existingCustomer != null)
                {
                    var customerRelatedContactID = ContactMembershipInfoProvider.GetContactIDByMembership(existingCustomer.CustomerID, MemberTypeEnum.EcommerceCustomer);

                    emailTaken = contact.ContactID != customerRelatedContactID;
                }
            }

            return emailTaken;
        }


        private static void MapStringField(string fieldName, int? maxLength, JObject contactData, ContactInfo contact)
        {
            var fieldValue = contactData.Value<string>(fieldName);
            if (!String.IsNullOrEmpty(fieldValue))
            {
                var truncatedValue = (maxLength.Value > 0) ? fieldValue.Truncate(maxLength.Value) : fieldValue;
                contact.SetValue(fieldName, truncatedValue);
            }
        }
    }
}
