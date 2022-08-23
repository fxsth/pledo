using Web.Models;

namespace Web.Data;

public static class DbInitializer
{
    public static void Initialize(DbContext context)
    {
        // Look for any students.
        if (context.Accounts.Any())
        {
            return; // DB has been seeded
        }

        // var plexAccounts = new Account[]
        // {
        //     new Account()
        //     {
        //         Username = "MaxMustermann",
        //         Password = "******",
        //         UserToken = "jgfieowjcsylr<j2453983"
        //     }
        // };

        // context.PlexAccounts.AddRange(plexAccounts);
        // context.SaveChanges();
    }
}