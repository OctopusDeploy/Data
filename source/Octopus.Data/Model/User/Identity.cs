using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Octopus.Data.Resources.Users;

namespace Octopus.Data.Model.User
{
    public sealed class Identity : IEquatable<Identity>, IEquatable<IdentityResource>
    {
        public Identity()
        {
            Claims = new Dictionary<string, IdentityClaim>();
        }

        public Identity(string identityProviderName) : this()
        {
            IdentityProviderName = identityProviderName;
        }

        public string IdentityProviderName { get; set; }

        public Dictionary<string, IdentityClaim> Claims { get; }

        [JsonIgnore]
        public string[] SearchableIdentifiers => Claims.Where(kvp => kvp.Value.IsIdentifyingClaim && !string.IsNullOrWhiteSpace(kvp.Value.Value)).Select(kvp => IdentityProviderName + ":" + kvp.Value.Value).ToArray();

        public IdentityResource ToResource()
        {
            return new IdentityResource(IdentityProviderName)
                .WithClaims(Claims);
        }

        internal Identity WithClaims(Dictionary<string, IdentityClaimResource> claims)
        {
            foreach (var kvp in claims)
            {
                if (Claims.ContainsKey(kvp.Key))
                {
                    Claims[kvp.Key].Value = kvp.Value.Value;
                }
                else
                {
                    Claims.Add(kvp.Key, new IdentityClaim(kvp.Value.Value, kvp.Value.IsIdentifyingClaim));
                }
            }
            return this;
        }

        public Identity WithClaim(string type, string value, bool isIdentifyingClaim, bool isServerSideOnly = false)
        {
            if (Claims.ContainsKey(type))
            {
                var claim = Claims[type];
                claim.Value = value;
                claim.IsIdentifyingClaim = isIdentifyingClaim;
                claim.IsServerSideOnly = isServerSideOnly;
            }
            else
            {
                var claim = new IdentityClaim(value, isIdentifyingClaim, isServerSideOnly);
                Claims.Add(type, claim);
            }
            return this;
        }

        public bool Equals(Identity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (IdentityProviderName != other.IdentityProviderName) return false;
            return Claims.All(kvp =>
            {
                if (!kvp.Value.IsIdentifyingClaim)
                    return true;
                if (!other.Claims.ContainsKey(kvp.Key))
                    return false;
                var existingClaimValue = kvp.Value.Value;
                var bothClaimsAreNullOrWhitespace = string.IsNullOrWhiteSpace(existingClaimValue) && string.IsNullOrWhiteSpace(other.Claims[kvp.Key].Value);
                var existingClaimHasValueAndMatches = !string.IsNullOrWhiteSpace(existingClaimValue) && existingClaimValue.Equals(other.Claims[kvp.Key].Value, StringComparison.OrdinalIgnoreCase);
                return bothClaimsAreNullOrWhitespace || existingClaimHasValueAndMatches;
            });
        }

        public bool Equals(IdentityResource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (IdentityProviderName != other.IdentityProviderName) return false;
            return Claims.All(kvp =>
            {
                if (!kvp.Value.IsIdentifyingClaim || kvp.Value.IsServerSideOnly)
                    return true;
                if (!other.Claims.ContainsKey(kvp.Key))
                    return false;
                var existingClaimValue = kvp.Value.Value;
                var bothClaimsAreNullOrWhitespace = string.IsNullOrWhiteSpace(existingClaimValue) && string.IsNullOrWhiteSpace(other.Claims[kvp.Key].Value);
                var existingClaimHasValueAndMatches = !string.IsNullOrWhiteSpace(existingClaimValue) && existingClaimValue.Equals(other.Claims[kvp.Key].Value, StringComparison.OrdinalIgnoreCase);
                return bothClaimsAreNullOrWhitespace || existingClaimHasValueAndMatches;
            });
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Identity) obj);
        }

        public override int GetHashCode()
        {
            return string.Concat(SearchableIdentifiers).GetHashCode();
        }
    }
}