using System.Configuration;
using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Factories
{
    public interface IAccountDataStoreFactory
    {
        IAccountDataStore Create();
    }
}

