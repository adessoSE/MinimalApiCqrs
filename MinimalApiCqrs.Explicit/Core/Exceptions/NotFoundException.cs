namespace MinimalApiCqrs.Explicit.Core.Exceptions;

public class NotFoundException : ApplicationException
{
    /// <summary>
    /// Throws a ApplicationNotFoundException with the given resource name.
    /// <para>
    /// The message is: "<paramref name="resourceName"/> wurde nicht gefunden."
    /// </para>
    /// </summary>
    /// <param name="resourceName">The name of the resource that was not found.
    /// <para>
    /// The message is: "<paramref name="resourceName"/> wurde nicht gefunden."
    /// </para>
    /// </param>
    public NotFoundException(string resourceName)
        : base($"{resourceName} wurde nicht gefunden.") { }
}
