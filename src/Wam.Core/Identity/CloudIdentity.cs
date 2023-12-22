using Azure.Core;
using Azure.Identity;

namespace Wam.Core.Identity;

public class CloudIdentity
{
    public static TokenCredential GetCloudIdentity()
    {
        return new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new VisualStudioCredential(),
            new AzureCliCredential());
    }
}