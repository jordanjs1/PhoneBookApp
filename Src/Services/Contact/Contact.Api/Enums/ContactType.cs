namespace Contact.Api.Enums;

/// <summary>
/// Defines possible contact information types.
/// </summary>
public enum ContactInformationType
{
    /// <summary>
    /// Indicates that contact information is a mobile phone number.
    /// </summary>
    MobileNumber = 1,

    /// <summary>
    /// Indicates that contact information is an e-mail address.
    /// </summary>
    EmailAddress = 2,

    /// <summary>
    /// Indicates that contact information is a location.
    /// </summary>
    Location = 3
}