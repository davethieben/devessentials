using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Essentials
{
    public static class SecurityExtensions
    {
        public static void AddClaim(this ClaimsPrincipal principal, string claimType, string claimValue)
        {
            principal.IsRequired();
            Contract.Requires(claimType, nameof(claimType));

            if (principal.Identity is ClaimsIdentity identity)
                identity.AddClaim(new Claim(claimType, claimValue));
            else
                throw new ArgumentException("Identity is not a ClaimsIdentity");
        }

        public static void AddClaims(this ClaimsPrincipal principal, IEnumerable<Claim> claims)
        {
            principal.IsRequired();

            if (!claims.IsNullOrEmpty())
            {
                if (principal.Identity is ClaimsIdentity identity)
                    identity.AddClaims(claims);
                else
                    throw new ArgumentException("Identity is not a ClaimsIdentity");
            }
        }

        public static bool HasClaim(this ClaimsPrincipal principal, string claimType)
        {
            return principal != null
                && claimType.HasValue()
                && principal.HasClaim(c => c.Type.IsEquivalent(claimType));
        }

        public static bool HasClaim(this ClaimsIdentity identity, string claimType)
        {
            return identity != null
                && claimType.HasValue()
                && identity.HasClaim(c => c.Type.IsEquivalent(claimType));
        }

        public static string? FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            identity.IsRequired();
            return identity.FindFirst(claimType)?.Value;
        }

        public static string? FindClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            principal.IsRequired();

            foreach(ClaimsIdentity? identity in principal.Identities)
            {
                string? value = identity?.FindFirstValue(claimType);
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return null;
        }

    }
}