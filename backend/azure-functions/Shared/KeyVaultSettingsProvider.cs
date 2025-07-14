using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace azure_functions.Shared;
public class KeyVaultSettingsProvider : IKeyVaultSettingsProvider
{
    private readonly Lazy<Task<Settings>> _settings;
    private readonly string _vaultName;
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public KeyVaultSettingsProvider(IConfiguration config, ILogger<KeyVaultSettingsProvider> logger)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(logger);
        _vaultName = config["KeyVaultName"] ?? throw new ArgumentException("KeyVaultName is not configured.");
        _config = config;
        _logger = logger;
        _settings = new Lazy<Task<Settings>>(LoadSecretsAsync);
    }

    public async Task<string> GetStorageAccountNameAsync() => (await _settings.Value).StorageAccountName;
    public async Task<string> GetStorageKeyAsync() => (await _settings.Value).StorageKey;
    public async Task<string> GetContainerNameAsync() => (await _settings.Value).ContainerName;

    private async Task<Settings> LoadSecretsAsync()
    {
        var tokenCredential = GetCredentials();
        var client = new SecretClient(new Uri($"https://{_vaultName}.vault.azure.net/"), tokenCredential);
        return new Settings
        {
            StorageAccountName = (await client.GetSecretAsync("StorageAccountName")).Value.Value,
            StorageKey = (await client.GetSecretAsync("StorageKey")).Value.Value,
            ContainerName = (await client.GetSecretAsync("BlobContainerName")).Value.Value
        };
    }

    private TokenCredential GetCredentials()
    {
        var clientId = _config["AZURE_CLIENT_ID"];
        var tenantId = _config["AZURE_TENANT_ID"];
        var clientSecret = _config["AZURE_CLIENT_SECRET"];

        if (clientId == null || tenantId == null || clientSecret == null)
        {
            _logger.LogInformation("Using DefaultAzureCredential for Key Vault access."); // az login
            return new DefaultAzureCredential();
        }

        _logger.LogInformation("Using ClientSecretCredential for Key Vault access."); // app registration
        return new ClientSecretCredential(tenantId, clientId, clientSecret); 
    }


    private class Settings
    {
        public string StorageAccountName { get; set; } = "";
        public string StorageKey { get; set; } = "";
        public string ContainerName { get; set; } = "";
    }
}
