namespace BO;
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
             : base(message, innerException) { }
}

public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
             : base(message, innerException) { }
}


public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
    public BlUnauthorizedAccessException(string message, Exception innerException)
             : base(message, innerException) { }
}

public class BlArgumentNullException : Exception
{
    public BlArgumentNullException(string? message) : base(message) { }
    public BlArgumentNullException(string message, Exception innerException)
             : base(message, innerException) { }
}
public class BlDataAccessException : Exception
{
    public BlDataAccessException(string? message) : base(message) { }
    public BlDataAccessException(string message, Exception innerException)
             : base(message, innerException) { }
}
public class BlInvalidPasswordException : Exception
{ 
     public BlInvalidPasswordException(string? message) : base(message) { }

}

public class BlInvalidStatusException : Exception//חריגה של אם ישנומצב שאינו תקין לדוגמא סטטוס הזמנה שלא קיים 
{
    public BlInvalidStatusException(string? message) : base(message) { }
}
public class BlInvalidOperationException : Exception//חריגה של אם ישנומצב שאינו תקין לדוגמא סטטוס הזמנה שלא קיים 
{
    public BlInvalidOperationException(string? message) : base(message) { }
}
