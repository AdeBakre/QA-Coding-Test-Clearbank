# QA Tech Test

## What you will need

To complete this test you will need:

 - Visual Studio - _Community edition available to download [here](https://visualstudio.microsoft.com/downloads/)_
 - Internet connection - _To download any tools/packages you require_

## Description

In `PaymentService.cs` you will find a method for making a payment. At a high level the steps for making a payment are:

- Lookup the account the payment is being made from
- Check the account is in a valid state to make the payment
- Deduct the payment amount from the account's balance and update the account in the database

We would like you to add some automated tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code, keeping the following in mind:

- The solution should build
- All tests should pass
- You are free to use any frameworks/NuGet packages that you see fit

In order to make the code more testable, you may wish to refactor the existing code. If you do so, please bear in mind:

- You must not change the signature of the MakePayment method
- Testability
- Adherence to SOLID principals
- Readability

You should plan to spend around one hour on this exercise.

During the interview you will be asked to share your screen and walk us through your code.
