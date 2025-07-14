namespace azure_functions.Shared
{
    public interface IKeyVaultSettingsProvider
    {
        Task<string> GetStorageAccountNameAsync();
        Task<string> GetStorageKeyAsync();
        Task<string> GetContainerNameAsync();
    }
}