namespace ContactMicroservice.Models
{
    /// <summary>
    /// Defines common fields among entities.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identifier.</typeparam>
    public class EntityBase<TId>
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public TId Id { get; set; }
    }
}
