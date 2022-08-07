using Web.Models;

namespace Web.Data;

public static class DbInitializer
{
    public static void Initialize(SettingContext context)
    {
        // Look for any students.
        if (context.PlexAccounts.Any())
        {
            return; // DB has been seeded
        }

        var plexAccounts = new PlexAccount[]
        {
            new PlexAccount()
            {
                Username = "MaxMustermann",
                Password = "******",
                AuthKey = "jgfieowjcsylr<j2453983"
            }
        };

        context.PlexAccounts.AddRange(plexAccounts);
        context.SaveChanges();
    }
}