using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BraintreeHttp;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Studio1BTask.Services
{
    public class PaypalService
    {
        private static readonly string _clientId = "PAYPAL-CLIENT-ID";
        private static readonly string _secret = "PAYPAL-CLIENT-SECRET";

        public static PayPalHttpClient Client()
        {
            // Creating a sandbox environment
            var environment = new SandboxEnvironment(_clientId, _secret);

            // Creating a client for the environment
            var client = new PayPalHttpClient(environment);
            return client;
        }

        public static async Task<HttpResponse> createOrder()
        {
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            var order = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = "100.00"
                        }
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "https://www.example.com",
                    CancelUrl = "https://www.example.com"
                }
            };


            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);
            var response = await Client().Execute(request);
            var statusCode = response.StatusCode;
            var result = response.Result<Order>();
            Console.WriteLine("Status: {0}", result.Status);
            Console.WriteLine("Order Id: {0}", result.Id);
            Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
            Console.WriteLine("Links:");
            foreach (var link in result.Links)
                Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
            return response;
        }
    }
}