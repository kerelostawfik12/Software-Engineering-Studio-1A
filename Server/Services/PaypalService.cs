using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Studio1BTask.Services
{
    public class PaypalService
    {
        private static readonly string _clientId =
            "AbY5qhTQK2zo_163bAA1xkA19YkBMg8uiLqqh3MIAULsH2yF7T2ZGQ38cx740JmcUq24_bwNLS6-e3LF";

        private static readonly string _secret =
            "EMicuENMd4PD-W86tF2wEvUFHTCf_bHhNqJYwc7zsRvEnKKj1tmSDlv30H220hxyeq-f0rbevVAJgGrk";

        public PayPalHttpClient Client()
        {
            // Creating a sandbox environment
            var environment = new SandboxEnvironment(_clientId, _secret);

            // Creating a client for the environment
            var client = new PayPalHttpClient(environment);
            return client;
        }

        public async Task<Order> CreateOrder(List<Item> items, decimal totalPrice)
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
                            CurrencyCode = "AUD",
                            Value = totalPrice.ToString(CultureInfo.InvariantCulture),
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = "AUD",
                                    Value = totalPrice.ToString(CultureInfo.InvariantCulture)
                                }
                            }
                        },
                        Items = items
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "http://studio1btask.azurewebsites.net/",
                    CancelUrl = "http://studio1btask.azurewebsites.net/"
                }
            };
            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);
            var response = await Client().Execute(request);
            var result = response.Result<Order>();
            return result;
        }

        public async Task<Order> CaptureOrder(string approvedOrderId)
        {
            // Construct a request object and set desired parameters
            // Replace ORDER-ID with the approved order id from create order
            var request = new OrdersCaptureRequest(approvedOrderId);
            request.RequestBody(new OrderActionRequest());
            var response = await Client().Execute(request);
            var getRequest = new OrdersGetRequest(approvedOrderId);
            response = await Client().Execute(getRequest);
            var result = response.Result<Order>();
            return result;
        }
    }
}