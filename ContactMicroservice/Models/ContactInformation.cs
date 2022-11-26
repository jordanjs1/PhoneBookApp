using ContactMicroservice.Enums;

namespace ContactMicroservice.Models
{
    /// <summary>
    /// Represents a contact information for a person.
    /// </summary>
    public class ContactInformation<TId> : EntityBase<TId>
    {
        /// <summary>
        /// The unique identifier of the person this contact information is associated with.
        /// </summary>
        public TId PersonId { get; set; }

        /// <summary>
        /// The <see cref="Person"/> associated with this contact information.
        /// </summary>
        public virtual Person Person { get; set; }

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
