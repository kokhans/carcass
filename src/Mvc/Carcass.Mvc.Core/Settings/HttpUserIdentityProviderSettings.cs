using Carcass.Core;

namespace Carcass.Mvc.Core.Settings;

public sealed class HttpUserIdentityProviderSettings
{
    public HttpUserIdentityProviderSettings(string userIdClaim)
    {
        ArgumentVerifier.NotNull(userIdClaim, nameof(userIdClaim));

        UserIdClaim = userIdClaim;
    }

    public string UserIdClaim { get; }
}