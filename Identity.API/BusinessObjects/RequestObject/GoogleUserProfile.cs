namespace Identity.API.BusinessObjects.RequestObject;

public class GoogleUserProfile
{
    public List<Name> Names { get; set; }
    public List<EmailAddress> EmailAddresses { get; set; }
    public List<PhoneNumber> PhoneNumbers { get; set; }
    public List<Address> Addresses { get; set; }
}

public class Name
{
    public string DisplayName { get; set; }
}

public class EmailAddress
{
    public string Value { get; set; }
}

public class PhoneNumber
{
    public string Value { get; set; }
}

public class Address
{
    public string FormattedValue { get; set; }
}
