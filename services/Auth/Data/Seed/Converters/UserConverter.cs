using Auth.Models;

namespace Auth.Data.Seed.Converters;
public static class UserConverter
{
    public static User FromSeedUser(SeedUser seedUser)
    {
        var user = new User
        {
            FirstName = seedUser.FirstName,
            LastName = seedUser.LastName,
            Email = seedUser.Email,
            UserName = seedUser.Email,
            EmailConfirmed = seedUser.EmailConfirmed
        };

        return user;
    }
}