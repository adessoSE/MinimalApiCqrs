namespace MinimalApiCqrs.Explicit.Core.Exceptions;

internal class TooManySuccessHandlerException : Exception
{
    public TooManySuccessHandlerException(string routeName)
        : base($"Only one success handler is allowed for route {routeName}.") { }
}
