namespace Contact.Api.Models
{
    /// <summary>
    /// Defines common fields among entities.
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; set; }
    }
}
