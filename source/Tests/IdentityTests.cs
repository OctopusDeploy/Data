using System;
using FluentAssertions;
using Octopus.Data.Model.User;
using Xunit;

namespace Tests
{
    public class IdentityTests
    {
        [Fact]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenExistingIdentityHasIdentifyingTrue()
        {
            var identity1 = CreateIdentity("foo@test.com", true, "foo@test.com", "foo");
            var identity2 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            identity1.Should().Be(identity2);
        }

        [Fact]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenExistingIdentityHasIdentifyingTrueAndNullValue()
        {
            var identity1 = CreateIdentity(null, true, "foo@test.com", "foo");
            var identity2 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            identity1.Should().NotBe(identity2);
        }

        [Fact]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenOtherIdentityHasIdentifyingTrue()
        {
            var identity1 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            var identity2 = CreateIdentity("foo@test.com", true, "foo@test.com", "foo");
            identity1.Should().Be(identity2);
        }

        [Fact]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenOtherIdentityHasIdentifyingTrueAndNullValue()
        {
            var identity1 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            var identity2 = CreateIdentity(null, true, "foo@test.com", "foo");
            identity1.Should().Be(identity2);
        }

        Identity CreateIdentity(string? email, bool emailIsIdentifying, string upn, string displayName)
        {
            var identity = new Identity("Test Provider");
            identity.Claims.Add("email", new IdentityClaim(email, emailIsIdentifying));
            identity.Claims.Add("upn", new IdentityClaim(upn, true));
            return identity;
        }
    }
}