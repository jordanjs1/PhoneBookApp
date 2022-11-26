namespace ContactMicroservice.Models
{
    /// <summary>
    /// Represents a person in the phone book.
    /// </summary>
    public class Person : EntityBase<Guid>
    {
        /// <summary>
        /// The name of the person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The surname of the person.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// The company the person is working in.
        /// </summary>
        public string Company { get; set; }
    }
}
