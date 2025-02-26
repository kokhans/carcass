using Carcass.Core;

namespace Carcass.Http.Settings;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
///     Represents the settings configuration for the HTTP User Identity Provider.
///     This class holds information related to the user identity claim used
///     to identify users based on HTTP context.
/// </summary>
public sealed class HttpUserIdentityProviderSettings
{
    /// <summary>
    ///     Represents the settings required for the HTTP-based User Identity Provider.
    /// </summary>
    public HttpUserIdentityProviderSettings(string userIdClaim)
    {
        ArgumentVerifier.NotNull(userIdClaim, nameof(userIdClaim));

        UserIdClaim = userIdClaim;
    }

    /// <summary>
    ///     Represents the claim type that identifies the user ID in the authentication token or context.
    /// </summary>
    /// <value>
    ///     A string that specifies the claim type used to extract the user identifier.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown during object instantiation when the provided claim type is null or invalid.
    /// </exception>
    public string UserIdClaim { get; }
}