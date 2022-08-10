using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests.Tests
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private Mock<IAccountDataStore> accountDataStoreMock;
        private Mock<IAccountDataStoreFactory> accountDataStoreFactoryMock;


        [SetUp]
        public void SetUpMockService()
        {
            accountDataStoreFactoryMock = new Mock<IAccountDataStoreFactory>();
            accountDataStoreMock = new Mock<IAccountDataStore>();
        }

        [Test]
        public void BacsPaymentVerification()
        {

            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 1250
            };

            accountDataStoreMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
            accountDataStoreFactoryMock.Setup(m => m.Create()).Returns(accountDataStoreMock.Object);

            var paymentService = new PaymentService(accountDataStoreFactoryMock.Object);
            var request = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 250, AllowedPaymentSchemes = account.AllowedPaymentSchemes
            };

            //Store the expected value
            var expectedBalance = account.Balance - request.Amount;
            //Action
            var actual = paymentService.MakePayment(request);
            //Assert
            Assert.IsTrue(actual.Success);
            Assert.That(account.Balance, Is.EqualTo(expectedBalance));
            //Extra check to ensure we never get to the code for updating account
            accountDataStoreMock.Verify(m => m.UpdateAccount(It.IsAny<Account>()), Times.Once);


        }

        [Test]
        public void ChapsPaymentVerification()
        {

            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 1500,
                Status = AccountStatus.Live
            };

            accountDataStoreMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
            accountDataStoreFactoryMock.Setup(m => m.Create()).Returns(accountDataStoreMock.Object);

            var paymentService = new PaymentService(accountDataStoreFactoryMock.Object);
            var request = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 350,
                AllowedPaymentSchemes = account.AllowedPaymentSchemes
            };

            //Store the expected value
            var expectedBalance = account.Balance - request.Amount;
            //Action
            var actual = paymentService.MakePayment(request);
            //Assert
            Assert.IsTrue(actual.Success);
            Assert.That(account.Balance, Is.EqualTo(expectedBalance));
            //Extra check to ensure we never get to the code for updating account
            accountDataStoreMock.Verify(m => m.UpdateAccount(It.IsAny<Account>()), Times.Once);

        }

        [Test]
        public void FasterPaymentsVerification()
        {

            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 2500

            };

            accountDataStoreMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
            accountDataStoreFactoryMock.Setup(m => m.Create()).Returns(accountDataStoreMock.Object);

            var paymentService = new PaymentService(accountDataStoreFactoryMock.Object);
            var request = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 650,
                AllowedPaymentSchemes = account.AllowedPaymentSchemes
            };

            //Store the expected value
            var expectedBalance = account.Balance - request.Amount;

            //Action
            var actual = paymentService.MakePayment(request);
            //Assert
            Assert.IsTrue(actual.Success);
            Assert.That(account.Balance, Is.EqualTo(expectedBalance));
            //Extra check to ensure we never get to the code for updating account
            accountDataStoreMock.Verify(m => m.UpdateAccount(It.IsAny<Account>()), Times.Once);


        }

        [Test]
        public void NonAccountHolderVerification()
        {
            //Set account to null to trigger condition of account being null
            accountDataStoreMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns((Account)null);
            accountDataStoreFactoryMock.Setup(m => m.Create()).Returns(accountDataStoreMock.Object);

            var paymentService = new PaymentService(accountDataStoreFactoryMock.Object);
            var request = new MakePaymentRequest();


            //Action
            var actual = paymentService.MakePayment(request);
            //Assert
            Assert.IsFalse(actual.Success);
            //Extra check to ensure we never get to the code for updating account
            accountDataStoreMock.Verify(m => m.UpdateAccount(It.IsAny<Account>()), Times.Never);


        }

        [Test]
        public void InactiveAccountVerification()
        {

            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 1500,Status = AccountStatus.Disabled
            };

            accountDataStoreMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
            accountDataStoreFactoryMock.Setup(m => m.Create()).Returns(accountDataStoreMock.Object);

            var paymentService = new PaymentService(accountDataStoreFactoryMock.Object);
            var request = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Chaps,
                Amount = 350,
                AllowedPaymentSchemes = account.AllowedPaymentSchemes
            };

            //Action
            var actual = paymentService.MakePayment(request);
            //Assert
            Assert.IsFalse(actual.Success);
            //Extra check to ensure we never get to the code for updating account
            accountDataStoreMock.Verify(m => m.UpdateAccount(It.IsAny<Account>()), Times.Never);


        }


        [Test]
        public void InsufficientFundsForFasterPayment()
        {

            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 500
             
            };

            accountDataStoreMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
            accountDataStoreFactoryMock.Setup(m => m.Create()).Returns(accountDataStoreMock.Object);

            var paymentService = new PaymentService(accountDataStoreFactoryMock.Object);
            var request = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.FasterPayments,
                Amount = 650,
                AllowedPaymentSchemes = account.AllowedPaymentSchemes
            };

            //Action
            var actual = paymentService.MakePayment(request);
            //Assert
            Assert.IsFalse(actual.Success);
            //Extra check to ensure we never get to the code for updating account
            accountDataStoreMock.Verify(m => m.UpdateAccount(It.IsAny<Account>()), Times.Never);


        }

        [Test]
        public void InsufficientFundsForNonFasterPayment()
        {

            var account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 500

            };

            accountDataStoreMock.Setup(m => m.GetAccount(It.IsAny<string>())).Returns(account);
            accountDataStoreFactoryMock.Setup(m => m.Create()).Returns(accountDataStoreMock.Object);

            var paymentService = new PaymentService(accountDataStoreFactoryMock.Object);
            var request = new MakePaymentRequest
            {
                PaymentScheme = PaymentScheme.Bacs,
                Amount = 650,
                AllowedPaymentSchemes = account.AllowedPaymentSchemes
            };

            //Store the expected value
            var expectedBalance = account.Balance - request.Amount;

            //Action
            var actual = paymentService.MakePayment(request);
            //Assert
            Assert.IsTrue(actual.Success);
            Assert.That(account.Balance, Is.EqualTo(expectedBalance));
            //Extra check to ensure we never get to the code for updating account
            accountDataStoreMock.Verify(m => m.UpdateAccount(It.IsAny<Account>()), Times.Once);


        }

    }
}
