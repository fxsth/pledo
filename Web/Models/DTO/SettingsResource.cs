namespace Web.Models.DTO;

public class SettingsResource
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public Option[]? Options { get; set; }
}

public class Option
{
    public Option(string value, string uiName)
    {
        UiName = uiName;
        Value = value;
    }
    public string UiName { get; }
    public string Value { get; }
}