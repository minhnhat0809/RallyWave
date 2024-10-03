namespace MatchManagement.Ultility;

public class Validate
{
    public bool IsEmptyOrWhiteSpace(string? input)
    {
        return string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input);
    }
}