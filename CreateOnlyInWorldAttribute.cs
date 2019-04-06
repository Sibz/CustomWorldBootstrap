using System;
public class CreateOnlyInWorldAttribute : Attribute
{
    public string Name;
    public CreateOnlyInWorldAttribute(string name)
    {
        Name = name;
    }
}