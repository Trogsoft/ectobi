using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Trogsoft.Ectobi.Common
{
    public class PersonModel
    {
        [DisplayName("First Name"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.SuggestedDefault)]
        public string? FirstName { get; set; }

        [DisplayName("Last Name"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.SuggestedDefault)]
        public string? LastName { get; set; }

        [DisplayName("Gender"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation)]
        public string? Gender { get; set; }

        [DisplayName("Honorific"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation)]
        public string? Honorific { get; set; }

        [DisplayName("Date of Birth"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation)]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Phone Number"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [DisplayName("Email Address"), 
            ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.EmailAddress | EctoModelPropertyFlags.SuggestedDefault)]
        public string? EmailAddress { get; set; }

        [DisplayName("Street Address"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.GeographicIdentifier)]
        public string? StreetAddress { get; set; }

        [DisplayName("Address"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.GeographicIdentifier)]
        public string? Address { get; set; }

        [DisplayName("City"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.GeographicIdentifier)]
        public string? City { get; set; }

        [DisplayName("Postal Code"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.GeographicIdentifier)]
        public string? PostalCode { get; set; }

        [DisplayName("Country"), ModelFlags(EctoModelPropertyFlags.PersonallyIdentifiableInformation | EctoModelPropertyFlags.GeographicIdentifier)]
        public string? Country { get; set; }

    }
}
