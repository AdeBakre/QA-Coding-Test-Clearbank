using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Factories;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStoreFactory _accountDataStoreFactory;

        public PaymentService(IAccountDataStoreFactory accountDataStoreFactory)
        {
            _accountDataStoreFactory = accountDataStoreFactory;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var accountDataStore = _accountDataStoreFactory.Create();
            //Is this really needed if not being retrieved from database?
            var account = accountDataStore.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();

            //Extracting common logic
            if (account == null) return result;

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                    }
                    else
                    {
                        result.Success = true;
                    }

                    break;

                case PaymentScheme.FasterPayments:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        result.Success = false;
                    }
                    else if (account.Balance < request.Amount)
                    {
                        result.Success = false;
                    }
                    else
                    {
                        result.Success = true;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                    {
                        result.Success = false;
                    }
                    else if (account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
                    }
                    else
                    {
                        result.Success = true;
                    }
                    break;
            }

            if (result.Success)
            {

                account.Balance -= request.Amount;
                accountDataStore.UpdateAccount(account);
                result.Success = true;
            }

            return result;
        }
    }
}
