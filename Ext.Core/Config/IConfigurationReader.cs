namespace Ext.Core.Config
{
    public interface IConfigurationReader
    {
        string ReadConfigurationValue(string key);
        string ReadConnectionStringValue(string connectionStringName);
    }
}