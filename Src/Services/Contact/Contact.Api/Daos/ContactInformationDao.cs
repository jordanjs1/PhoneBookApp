using Contact.Api.Enums;

namespace Contact.Api.Daos
{
    /// <summary>
    /// Represents a contact information for a contact.
    /// </summary>
    public class ContactInformationDao : ContactInformationWithoutContactIdDao
    {
        /// <summary>
        /// The unique identifier of the contact this contact information is associated with.
        /// </summary>
        public Guid ContactId { get; set; }
    }
}
