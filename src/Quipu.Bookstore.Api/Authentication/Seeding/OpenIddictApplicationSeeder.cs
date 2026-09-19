using Quipu.Bookstore.Api.Authorization;

using OpenIddict.Abstractions;

namespace Quipu.Bookstore.Api.Authentication.Seeding
{
    public static class OpenIddictApplicationSeeder
    {
        public static async Task EnsureScopeAsync(
            this IOpenIddictScopeManager scopeManager,
            string scopeName,
            CancellationToken cancellationToken)
        {
            if (await scopeManager.FindByNameAsync(scopeName, cancellationToken) is not null)
                return;

            await scopeManager.CreateAsync(
                new OpenIddictScopeDescriptor
                {
                    Name = scopeName,
                    Resources = { BookstoreAudience.Api }
                },
                cancellationToken
            );
        }

        public static async Task UpsertApplicationAsync(
            this IOpenIddictApplicationManager applicationManager,
            OpenIddictApplicationDescriptor descriptor,
            CancellationToken cancellationToken)
        {
            object? application = await applicationManager.FindByClientIdAsync(descriptor.ClientId!, cancellationToken);

            if (application is null)
            {
                await applicationManager.CreateAsync(descriptor, cancellationToken);
                return;
            }

            await applicationManager.UpdateAsync(application, descriptor, cancellationToken);
        }
    }
}
