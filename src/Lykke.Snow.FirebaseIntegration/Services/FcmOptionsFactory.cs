using FirebaseAdmin;
using Lykke.Snow.FirebaseIntegration.Interfaces;

namespace Lykke.Snow.FirebaseIntegration.Services
{

    public sealed class FcmOptionsFactory(
        IGoogleCredentialsProvider credentialsProvider) : IFcmOptionsFactory
    {
        private readonly IGoogleCredentialsProvider _credentialsProvider = credentialsProvider;
        public AppOptions Create() => new() { Credential = _credentialsProvider.Get() };
    }
}
