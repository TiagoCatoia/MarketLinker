namespace MarketLinker.Domain.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string? message = null) : base(message ?? "Bad request.") { }
}