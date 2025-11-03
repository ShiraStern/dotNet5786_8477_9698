using System;

namespace DO;

/// <summary>
/// Courier Entity represents a courier with all its properties.
/// </summary>
/// <param name="Id">Courier’s unique ID number (cannot be null or modified)</param>
/// <param name="FullName">Courier’s full name (first and last name)</param>
/// <param name="Phone">Courier’s phone number (10 digits, validated in logic layer)</param>
/// <param name="Email">Courier’s email address (validated in logic layer)</param>
/// <param name="Password">Courier’s password, used for login</param>
/// <param name="Active">Indicates whether the courier is currently active</param>
/// <param name="MaxDistance">Maximum delivery distance in kilometers for this courier , can be null (validated in logic layer)</param>
/// <param name="DeliveryType">Type of delivery vehicle or method used by the courier</param>
public record Courier
(
    int Id,
    string FullName,
    string Phone,
    string Email,
    string Password,
    bool Active,
    double? MaxDistance,
    DeliveryType DeliveryType ,//לשאול את אפרת מחרר בדחיפוות
    DateTime EmploymentStartDate 
)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Courier() : this(0, "", "", "", "", false, 0, DeliveryType.None, DateTime.Now) { }
}

/// <summary>
/// Enum representing different types of delivery methods.
/// </summary>
public enum DeliveryType
{
    None,
    Drone,
    Motorcycle,
    Car
}