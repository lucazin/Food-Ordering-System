using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using System.Diagnostics;
using Food_Order.Models;

namespace net.authorize.sample
{
    public class ChargeCreditCard
    {
        public static ResponseHandler Run(decimal amount)
        {
            Console.WriteLine("Charge Credit Card Sample");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "9ZtJvb59pK",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "73A9F4v7NEnj76BJ",
            };

            var creditCard = new creditCardType
            {
                cardNumber = "4111111111111111",
                expirationDate = "1028",
                cardCode = "123"
            };

            var billingAddress = new customerAddressType
            {
                firstName = "John",
                lastName = "Doe",
                address = "123 My St",
                city = "OurTown",
                zip = "98004"
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            // Add line Items
            var lineItems = new lineItemType[2];
            lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(15.00) };
            lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(450.00) };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card

                amount = amount,
                payment = paymentType,
                billTo = billingAddress,
                lineItems = lineItems,
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        ResponseHandler handler = new ResponseHandler();
                        handler.check = true;
                        handler.response = response.transactionResponse.transId;
                        return handler;
                    }
                    else
                    {
                        if (response.transactionResponse.errors != null)
                        {
                            ResponseHandler handler = new ResponseHandler();
                            handler.check = false;
                            handler.response = response.transactionResponse.errors[0].errorText;
                            return handler;
                        }
                        return null;
                    }
                }
                else
                {

                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        ResponseHandler handler = new ResponseHandler();
                        handler.check = false;
                        handler.response = response.transactionResponse.errors[0].errorText;
                        return handler;
                    }
                    else
                    {
                        ResponseHandler handler = new ResponseHandler();
                        handler.check = false;
                        handler.response = response.messages.message[0].text;
                        return handler;
                    }
                }
            }
            else
            {
                return null;
            }


        }
    }
}