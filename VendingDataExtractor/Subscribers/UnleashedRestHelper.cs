using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using VendingDataExtractor.Annotations;
using VendingDataExtractor.Consumer;
using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Misc;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Subscribers
{
    public class UnleashedRestHelper : RestHelper
    {

        private ApiEndpoint ApiEndpoint { get; set;}
        private String ApiEndpointParams { get; set; }
        private String EndPointArgument { get; set; }
        private Auth UnleashedAuth { get; set; }
        private ILogger<string> Logger { [UsedImplicitly] get; set; }

        public UnleashedRestHelper(ILogger<string> logger)
        {
            Init("", Method.Get, ApiEndpoint.Customers, new Auth("",""), logger);
        }

        public UnleashedRestHelper(String endPointAddress, ILogger<string> logger)
        {
            
            this.Init(endPointAddress, Method.Get, ApiEndpoint.Customers, new Auth("", ""), logger);
        }

        public UnleashedRestHelper(String endPointAddress, Method method, ILogger<string> logger)
        {
            this.Init(endPointAddress, method, ApiEndpoint.Customers, new Auth("", ""), logger);
        }

        public UnleashedRestHelper(string endPointAddress, Method method, ApiEndpoint endpointFunction, ILogger<string> logger)
        {
            this.Init(endPointAddress, method, endpointFunction, new Auth("", ""), logger);
        }

        public UnleashedRestHelper(string endPointAddress, Method method, ApiEndpoint endpointFunction, Auth auth, ILogger<string> logger)
        {
            this.Init(endPointAddress, method, endpointFunction, auth, logger);
        }

        private void Init(String endPointAddress, Method method, ApiEndpoint endPoint, Auth auth, ILogger<string> logger)
        {
            this.ApiEndpoint = endPoint;
            this.ApiAddress = endPointAddress;
            this.ApiMethod = method;
            this.UnleashedAuth = auth;
            this.Logger = logger;
        }

        public override string GetContentTypeString()
        {
            switch (this.ContentType)
            {
                case ContentType.ApplicationJson:
                    return "application/json";

                case ContentType.ApplicationXml:
                    return "application/xml";

                default:
                    return "application/json";
            }
        }

        public override string GetEndPointFuntionString()
        {
            return this.ApiEndpoint.ToString();
        }

       

        public override string GetEndPointFunctionParametersString()
        {
            if (this.ApiEndpointParams.IndexOf("?", StringComparison.Ordinal) > -1)
            {
                return this.ApiEndpointParams.Substring(this.ApiEndpointParams.IndexOf("?", StringComparison.Ordinal));
            }
            return this.ApiEndpointParams;
        }

        public override string GetEndPointFunctionArgumentString()
        {
            return this.EndPointArgument;
        }

        public override object Get(Type serializeType)
        {
            var result = new object();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this.ApiAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(this.GetContentTypeString()));
                    client.DefaultRequestHeaders.Add("api-auth-id", UnleashedAuth.Id);
                    client.DefaultRequestHeaders.Add("api-auth-signature", GetSignature(this.GetEndPointFunctionParametersString().Replace("?", ""), UnleashedAuth.Password));
                    HttpResponseMessage response = client.GetAsync(this.GetEndPointFuntionString() + this.GetEndPointFunctionArgumentString() + this.GetEndPointFunctionParametersString()).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //result = response.Content.ReadAsAsync(serializeType).Result;
                        result = Utilities.DeserializeObjectByString(serializeType, response.Content.ReadAsStringAsync().Result, Logger);
                    }
                }

            }
            catch (Exception e)
            {
                this.Logger.LogEntry("Error in Get(): " + e);
            }
                
            return result;
        }


        public override object Post(object data)
        {
            var result = new object();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this.ApiAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(this.GetContentTypeString()));
                    client.DefaultRequestHeaders.Add("api-auth-id", UnleashedAuth.Id);
                    client.DefaultRequestHeaders.Add("api-auth-signature", GetSignature(this.GetEndPointFunctionParametersString().Replace("?", ""), UnleashedAuth.Password));
                    var endString = (this.GetEndPointFuntionString() + "/" + this.GetEndPointFunctionArgumentString() +
                                    "?" + this.GetEndPointFunctionParametersString()).Trim();
                    StringContent serializedData;
                    if (this.ContentType.Equals(ContentType.ApplicationJson))
                    {
                        serializedData = new StringContent(Utilities.JSONSerialize(data), Encoding.UTF8, "application/xml");
                    }
                    else
                    {
                        serializedData = new StringContent(Utilities.XMLSerialize(data),
                            Encoding.UTF8, "application/xml");
                    }

                    HttpResponseMessage response = client.PostAsync(endString, serializedData).Result;

                    result = response.IsSuccessStatusCode;
                    if (!response.IsSuccessStatusCode)
                    {
                        var responseBodyAsText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine + response.RequestMessage + Environment.NewLine + response.Content.ReadAsStringAsync().Result;
                        responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                        Console.WriteLine(responseBodyAsText);
                        throw new Exception("response.IsSuccessStatusCode == False : " + responseBodyAsText);
                    }
                }

            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var s = reader.ReadToEnd();
                            this.Logger.LogEntry("Error in Post(): " + s);
                        }
                    }
            }
            catch (Exception e)
            {
                this.Logger.LogEntry("Error in Post(): " + e);
            }

            return result;
        }

        public override object Put(object data)
        {
            var result = new object();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this.ApiAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(this.GetContentTypeString()));
                    client.DefaultRequestHeaders.Add("api-auth-id", UnleashedAuth.Id);
                    client.DefaultRequestHeaders.Add("api-auth-signature", GetSignature("", UnleashedAuth.Password));

                    HttpResponseMessage response;
                    switch (this.ContentType)
                    {
                        case ContentType.ApplicationJson:
                            response = client.PutAsJsonAsync(this.GetEndPointFuntionString() + this.GetEndPointFunctionParametersString(), data).Result;
                            break;

                        case ContentType.ApplicationXml:
                            response = client.PutAsJsonAsync(this.GetEndPointFuntionString() + this.GetEndPointFunctionParametersString(), data).Result;
                            break;

                        default:
                            response = client.PutAsJsonAsync(this.GetEndPointFuntionString() + this.GetEndPointFunctionParametersString(), data).Result;
                            break;
                    }

                    result = response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                this.Logger.LogEntry("Error in Post(): " + e);
            }
            
            return result;
        }

        private static string GetSignature(string args, string privatekey)
        {
            var encoding = new ASCIIEncoding();
            byte[] key = encoding.GetBytes(privatekey);
            var myhmacsha256 = new HMACSHA256(key);
            byte[] hashValue = myhmacsha256.ComputeHash(encoding.GetBytes(args));
            string hmac64 = Convert.ToBase64String(hashValue);
            myhmacsha256.Clear();
            return hmac64;
        }

        //Unleashed REST call wrappers
        public bool PostSalesOrder(List<SoldGenericProduct> products, List<SalesOrderCustomer> customerObj, string orderNumber, Decimal taxRate)
        {
            var result = false;
            try
            {
                if (products.Count <= 0) return false;
                var salesOrderLines = GetSalesOrderLines(products, Convert.ToDecimal(taxRate));
                if (salesOrderLines == null) return false;
                if (salesOrderLines.Count <= 0) return false;
                var orderGuid = Guid.NewGuid().ToString();
                var salesOrder = getSalesOrderHeader(salesOrderLines, customerObj, orderNumber, orderGuid);
                if (salesOrder == null) return false;
                salesOrder.SalesOrderLines = salesOrderLines.ToArray();
                this.SetupApiCommunication(ApiEndpoint.SalesOrders, orderGuid, "", Method.Post, ContentType.ApplicationXml);
                result = (bool)this.Post(salesOrder);
            }
            catch (Exception e)
            {
                this.Logger.LogEntry("Error in PostSalesOrder(): " + e);
            }
                
            return result;
        }


        private Products GetProductsByGuid(String productCode)
        {
            this.SetupApiCommunication(ApiEndpoint.Products, "", "?productCode=" + productCode,  Method.Get, ContentType.ApplicationXml);
            return (Products)this.Get(typeof(Products));
        }

        private void SetupApiCommunication(ApiEndpoint endPoint, string endPointArgument, string endPointParams, Method apiMethod, ContentType contentType)
        {
            this.ApiEndpoint = endPoint;
            this.ApiEndpointParams = endPointParams;
            this.ApiMethod = apiMethod;
            this.ContentType = contentType;
            this.EndPointArgument = endPointArgument;
        }

        private List<SalesOrderSalesOrderLinesSalesOrderLine> GetSalesOrderLines(List<SoldGenericProduct> productList, Decimal taxRate)
        {
            const Int16 ELEMENT_PRODUCTS = 1;

            var theseLines = new List<SalesOrderSalesOrderLinesSalesOrderLine>();

            // Get a list of distinct products from the coil list
            var coilList = productList.GroupBy(i => i.ProductCode).Select(group => group.First()).ToList();

            if (coilList.Count > 0)
            {
                var lineCounter = 1;
                foreach (var distinctObject in coilList)
                {
                    // Get all coils from list of the same type.
                    var theseDistinctCoils = productList.Where(i => i.ProductCode.Trim() == distinctObject.ProductCode.Trim()).ToList();
                    if (theseDistinctCoils.Count <= 0) continue;

                    var thisLine = new SalesOrderSalesOrderLinesSalesOrderLine();

                    //Get coil product code from mapping file.
                    //String productCode = this.GetGuidByMapping(distinctObject);

                    //If we cant find a productcode to map to then exit as we cant match it to anything
                    if (string.IsNullOrEmpty(distinctObject.ProductCode))
                    {
                        // Maybe get productcode by description here??????
                        return null;
                    }

                    // Now that we have product code we can query unleashed with it.
                    var thisProduct = (ProductsProduct)this.GetProductsByGuid(distinctObject.ProductCode).Items[ELEMENT_PRODUCTS];
                    if (thisProduct == null) continue;

                    var customerTaxRate = Decimal.Round(taxRate, 2);
                    var defaultCost = Decimal.Round(Convert.ToDecimal(String.IsNullOrEmpty(thisProduct.DefaultPurchasePrice) ? "0.00" : thisProduct.DefaultPurchasePrice), 2);
                    var defaultSellPrice = Decimal.Round(Convert.ToDecimal(String.IsNullOrEmpty(thisProduct.DefaultSellPrice) ? "0.00" : thisProduct.DefaultSellPrice), 2);
                    var quantitySold = Convert.ToInt32(theseDistinctCoils.Sum(thisProd => String.IsNullOrEmpty(thisProd.QuantitySold.Trim()) ? 0.00 : Convert.ToDouble(thisProd.QuantitySold.Trim())));
                    var productWeight = Decimal.Round(Convert.ToDecimal(String.IsNullOrEmpty(thisProduct.Weight) ? 0.00 : Convert.ToDouble(thisProduct.Weight.Trim())), 2);
                    var productVolume = Convert.ToDecimal(String.IsNullOrEmpty(thisProduct.Height) ? "0.00" : thisProduct.Height) * Convert.ToDecimal(String.IsNullOrEmpty(thisProduct.Width) ? "0.00" : thisProduct.Width) * Convert.ToDecimal(String.IsNullOrEmpty(thisProduct.Depth) ? "0.00" : thisProduct.Depth);
                    var lineVolume = Decimal.Round((quantitySold*productVolume), 2);

                    thisLine.BCLineTax = Decimal.Round((defaultSellPrice * customerTaxRate) * quantitySold, 2).ToString(CultureInfo.InvariantCulture);
                    thisLine.BCLineTotal = Decimal.Round(quantitySold * defaultSellPrice, 2).ToString(CultureInfo.InvariantCulture);
                    thisLine.BCUnitPrice = Decimal.Round(defaultSellPrice, 2).ToString(CultureInfo.InvariantCulture);

                    thisLine.LineTax = Decimal.Round((defaultSellPrice * customerTaxRate) * quantitySold, 2).ToString(CultureInfo.InvariantCulture);
                    thisLine.LineTotal = Decimal.Round(quantitySold * defaultSellPrice, 2).ToString(CultureInfo.InvariantCulture);
                    thisLine.UnitPrice = Decimal.Round(defaultSellPrice, 2).ToString(CultureInfo.InvariantCulture);

                    thisLine.DiscountRate = "0.00";
                    thisLine.DueDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddT00:00:00");
                    thisLine.Guid = Guid.NewGuid().ToString();
                    thisLine.LastModifiedOn = DateTime.UtcNow.ToString("yyyy-MM-ddT00:00:00");
                    thisLine.LineNumber = lineCounter.ToString(CultureInfo.InvariantCulture);

                    thisLine.Weight = Decimal.Round((productWeight * quantitySold), 2).ToString(CultureInfo.InvariantCulture);
                    thisLine.OrderQuantity = quantitySold.ToString(CultureInfo.InvariantCulture);
                    thisLine.UnitCost = defaultCost.ToString(CultureInfo.InvariantCulture);

                    thisLine.Volume = lineVolume.ToString(CultureInfo.InvariantCulture);
                    var tempProductList = new List<SalesOrderSalesOrderLinesSalesOrderLineProduct>();
                    var tempProduct = new SalesOrderSalesOrderLinesSalesOrderLineProduct
                    {
                        Guid = thisProduct.Guid,
                        ProductCode = thisProduct.ProductCode
                    };
                    tempProductList.Add(tempProduct);
                    thisLine.Product = tempProductList.ToArray();
                    
                    lineCounter++;
                    theseLines.Add(thisLine);
                }
            }

            return theseLines;
        }

        private SalesOrder getSalesOrderHeader(List<SalesOrderSalesOrderLinesSalesOrderLine> salesOrderLines, List<SalesOrderCustomer> customerObj, string orderNumber, string orderGUID)
        {
            var thisOrder = new SalesOrder
            {
                Total = "0.00",               
                TaxTotal = "0.00",
                SubTotal = "0.00",
                DiscountRate = "0.00",
                SendAccountingJournalOnly = "false",//
                RequiredDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddT00:00:00"),
                ReceivedDate = DateTime.UtcNow.ToString("yyyy-MM-ddT00:00:00"),//
                PaymentDueDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddT00:00:00"),//
                Customer = customerObj.ToArray(),
                Guid = orderGUID,
                OrderDate = DateTime.UtcNow.ToString("yyyy-MM-ddT00:00:00"),
                OrderNumber = "SO-" + orderNumber.PadLeft(10,'0'),
                OrderStatus = "Pending",
                SalesOrderLines = salesOrderLines.ToArray()
            };
            
            var thisTax = new SalesOrderTax
            {
                Guid = "00000000-0000-0000-0000-000000000000",
                CanApplyToExpenses = "false",
                CanApplyToRevenue = "false",
                TaxCode = "S",
                LastModifiedOn = DateTime.UtcNow.ToString("yyyy-MM-ddT00:00:00")
            };

            var thisTaxList = new List<SalesOrderTax> {thisTax};
            thisOrder.Tax = thisTaxList.ToArray();

            thisOrder.TotalWeight = salesOrderLines.Sum(thisLine => String.IsNullOrEmpty(thisLine.Weight) ? 0.00 : Convert.ToDouble(thisLine.Weight)).ToString(CultureInfo.InvariantCulture);
            thisOrder.TotalVolume = salesOrderLines.Sum(thisLine => String.IsNullOrEmpty(thisLine.Volume) ? 0.00 : Convert.ToDouble(thisLine.Volume)).ToString(CultureInfo.InvariantCulture);
            thisOrder.TaxTotal = salesOrderLines.Sum(thisLine => String.IsNullOrEmpty(thisLine.LineTax) ? 0.00 : Convert.ToDouble(thisLine.LineTax)).ToString(CultureInfo.InvariantCulture);
            thisOrder.SubTotal = salesOrderLines.Sum(thisLine => String.IsNullOrEmpty(thisLine.LineTotal) ? 0.00 : Convert.ToDouble(thisLine.LineTotal)).ToString(CultureInfo.InvariantCulture);
            thisOrder.Total = (Convert.ToDecimal(thisOrder.SubTotal) + Convert.ToDecimal(thisOrder.TaxTotal)).ToString(CultureInfo.InvariantCulture);
            
            thisOrder.BCTaxTotal = salesOrderLines.Sum(thisLine => String.IsNullOrEmpty(thisLine.BCLineTax) ? 0.00 : Convert.ToDouble(thisLine.BCLineTax)).ToString(CultureInfo.InvariantCulture);
            thisOrder.BCSubTotal = salesOrderLines.Sum(thisLine => String.IsNullOrEmpty(thisLine.BCLineTotal) ? 0.00 : Convert.ToDouble(thisLine.BCLineTotal)).ToString(CultureInfo.InvariantCulture);
            thisOrder.BCTotal = (Convert.ToDecimal(thisOrder.BCSubTotal) + Convert.ToDecimal(thisOrder.BCTaxTotal)).ToString(CultureInfo.InvariantCulture);
            

            return thisOrder;
        }

    }


}