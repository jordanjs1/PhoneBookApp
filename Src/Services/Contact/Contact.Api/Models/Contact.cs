namespace Contact.Api.Models
{
    /// <summary>
    /// Represents a contact in the phone book.
    /// </summary>
    public class Contact : EntityBase
    {
        /// <summary>
        /// The name of the contact.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The surname of the contact.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// The company the contact is working in.
        /// </summary>
        public string Company { get; set; }
    }
}
