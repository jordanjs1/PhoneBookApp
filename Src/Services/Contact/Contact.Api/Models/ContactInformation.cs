using Contact.Api.Enums;

namespace Contact.Api.Models
{
    /// <summary>
    /// Represents a contact information for a contact.
    /// </summary>
    public class ContactInformation : EntityBase
    {
        /// <summary>
        /// The unique identifier of the contact this contact information is associated with.
        /// </summary>
        public Guid ContactId { get; set; }

        public virtual Contact Contact { get; set; }

        /// <summary>
        /// The type of the contact information.
        /// </summary>
        public ContactInformationType Type { get; set; }

        /// <summary>
        /// The content of the contact information.
        /// </summary>
        public string Content { get; set; }
    }
}
