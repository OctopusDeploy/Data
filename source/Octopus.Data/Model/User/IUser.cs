using System;
using System.Collections.Generic;
using Octopus.Server.MessageContracts.Features.Users;

namespace Octopus.Data.Model.User
{
    public interface IUser : IId<UserId>
    {
        string Username { get; }
        Guid IdentificationToken { get; }

        string DisplayName { get; set; }
        string EmailAddress { get; set; }

        bool IsService { get; set; }
        bool IsActive { get; set; }

        HashSet<Identity> Identities { get; }

        void RevokeSessions(DateTimeOffset validFrom);
        bool ValidateAccessToken(DateTimeOffset tokenIssuedAt);
        bool ValidateRefreshToken(DateTimeOffset tokenIssuedAt);

        bool ValidateActiveSession(Guid sessionId);
 
        void SetPassword(string plainTextPassword);
        bool ValidatePassword(string plainTextPassword);
        SecurityGroups GetSecurityGroups(string identityProviderName);
        IEnumerable<string> GetSecurityGroups();
    }
}