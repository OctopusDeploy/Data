using NUnit.Framework;
using Octopus.Data.Model.User;

namespace Tests
{
    public class IdentityTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenExistingIdentityHasIdentifyingTrue()
        {
            var identity1 = CreateIdentity("foo@test.com", true, "foo@test.com", "foo");
            var identity2 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            Assert.IsTrue(identity1.Equals(identity2));
        }

        [Test]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenExistingIdentityHasIdentifyingTrueAndNullValue()
        {
            var identity1 = CreateIdentity(null, true, "foo@test.com", "foo");
            var identity2 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            Assert.IsFalse(identity1.Equals(identity2));
        }

        [Test]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenOtherIdentityHasIdentifyingTrue()
        {
            var identity1 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            var identity2 = CreateIdentity("foo@test.com", true, "foo@test.com", "foo");
            Assert.IsTrue(identity1.Equals(identity2));
        }

        [Test]
        public void EqualityCheckWhereIdentifyingAttributeHasChangedWorksWhenOtherIdentityHasIdentifyingTrueAndNullValue()
        {
            var identity1 = CreateIdentity("foo@test.com", false, "foo@test.com", "foo");
            var identity2 = CreateIdentity(null, true, "foo@test.com", "foo");
            Assert.IsTrue(identity1.Equals(identity2));
        }

        Identity CreateIdentity(string email, bool emailIsIdentifying, string upn, string displayName)
        {
            var identity = new Identity("Test Provider");
            identity.Claims.Add("email", new IdentityClaim(email, emailIsIdentifying));
            identity.Claims.Add("upn", new IdentityClaim(upn, true));
            return identity;
        }
    }
}