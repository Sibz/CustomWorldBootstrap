using System;

/// <summary>
/// Apply to a system, system group or buffer system 
/// to only create in specified world
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CreateInWorldAttribute : Attribute
{
    public string Name;
    public CreateInWorldAttribute(string name)
    {
    test broken build
        Name = name;
    }
}
