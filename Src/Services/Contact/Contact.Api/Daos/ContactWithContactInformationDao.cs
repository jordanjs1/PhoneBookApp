using Contact.Api.Models;

namespace Contact.Api.Daos
{
    /// <summary>
    /// Represents a contact with any contact information associated to the contact.
    /// </summary>
    public class ContactWithContactInformationDao : Models.Contact
    {
        /// <summary>
        /// The contact information associated with the contact.
        /// </summary>
        public IEnumerable<ContactInformationWithoutContactIdDao> ContactInformation { get; set; }
    }
}
