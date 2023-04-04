using Google.Apis.Auth.OAuth2;

namespace Lykke.Snow.FirebaseIntegration.Interfaces
{
    public interface IGoogleCredentialsProvider
    {
        GoogleCredential Get();
    }
}
