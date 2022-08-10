using System.Configuration;
using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Factories
{
    public class AccountDataStoreFactory : IAccountDataStoreFactory
    {
        public IAccountDataStore Create()
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            IAccountDataStore accountDataStore;
            if(dataStoreType == "Backup")
            {
                accountDataStore = new BackupAccountDataStore();
            }
            else
            {
                accountDataStore = new AccountDataStore();
            }

            return accountDataStore;
        }
    }
}

