using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VIR.Objects;
using VIR.Objects.Company;

namespace VIR.Services
{
    public class ResourceHandlingService
    {
        private readonly DataBaseHandlingService _database;

        public ResourceHandlingService(IServiceProvider services, DataBaseHandlingService database)
        {
            _database = database;
        }

        public async void SetResources(JObject resource)
        {
            await _database.SetJObjectAsync(resource, "resources", false);
        }

        public JObject GetResources(string id)
        {
            return _database.getJObjectAsync(id, "resources").Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Type of resource</param>
        /// <param name="idBuyer">Id of the buyer</param>
        /// <param name="amount">Amount of resources to be bought</param>
        /// <param name="isCompany">Whether or not the buyer is a company</param>
        /// <returns></returns>
        public ResourceTransferResult BuyResourceFromMarket(string type, string idBuyer, ulong amount, bool isCompany)
        {
            /*var openListingsJson = _database.getJObjects("resource_market_listings").Result.ToList();
            var openListings = new List<ResourceMarketListing>();

            foreach (var olJson in openListingsJson)
            {
                var ol = new ResourceMarketListing(olJson);
                if (ol.Type == type)
                {
                    openListings.Add(ol);
                }
            }
            
            openListings = openListings.OrderBy(x => x.Price).ToList();

            double totalSpent = 0.0;
            ulong totalResourcesBought = 0L;

            foreach (var listing in openListings)
            {
                if (amount > 0)
                {
                    if (amount < listing.Amount)
                    {
                        var totalPrice = amount * listing.Price;
                        totalResourcesBought += amount;
                        totalSpent += totalPrice;
                        listing.Amount -= amount;
                        _database.SetJObjectAsync(listing.SerializeIntoJObject(), "resource_market_listings");
                        InformSellerOfSale(listing.IdSeller, idBuyer, amount, totalPrice);
                        //add money stuff here
                        //finish return thing
                        return new ResourceTransferResult();
                    }
                    else if (amount == listing.Amount)
                    {
                        var totalPrice = amount * listing.Price;
                        totalResourcesBought += amount;
                        totalSpent += totalPrice;
                        _database.RemoveObjectAsync(listing.Id, "resource_market_listings");
                        InformSellerOfSale(listing.IdSeller, idBuyer, amount, totalPrice);
                        //add money stuff here
                        //finish return thing
                        return new ResourceTransferResult();
                    }
                    else if (amount > listing.Amount)
                    {
                        var totalPrice = amount * listing.Price;
                        totalResourcesBought += listing.Amount;
                        totalSpent += totalPrice;
                        amount -= listing.Amount;
                        _database.RemoveObjectAsync(listing.Id, "resource_market_listings");
                        InformSellerOfSale(listing.IdSeller, idBuyer, amount, totalPrice);
                        //add money stuff here
                        //finish return thing
                        return new ResourceTransferResult();
                    }
                }
                else
                {
                    break;
                }
            }

            AddResource(idBuyer, type, amount);

            //If there weren't enough resources to buy, buys as many as possible
            if (amount > 0)
            {
                //finish return thing
                return new ResourceTransferResult();
            }*/
            return new ResourceTransferResult();
        }

        public ResourceTransferResult BuyResourceFromSpecificOfferOnMarket()
        {
            return new ResourceTransferResult();
        }

        private void InformSellerOfSale(string idSeller, string idBuyer, ulong amount, double totalPrice)
        {

        }

        public ResourceListingResult PutResourceUpForSale(string idSeller, string type, ulong amount, ulong pricePerUnit)
        {
            var resultReduce = ReduceResource(idSeller, type, amount);
            var currentYear = _database.getJObjectAsync("CurrentYear", "system").Result.GetValue("value")
                .ToObject<int>();
            if (!resultReduce.Error)
            {
                var newListing = new ResourceMarketListing(type, idSeller, amount, pricePerUnit, currentYear);
                _database.SetJObjectAsync(newListing.SerializeIntoJObject(), "resource_market_listings", true);

                return new ResourceListingResult()
                {
                    Message = $"Successfully listed {amount} {type} for {pricePerUnit} per unit.",
                    Amount = amount,
                    Error = false,
                    IdSeller = idSeller,
                    Type = type
                };
            }
            return new ResourceListingResult()
            {
                Message = resultReduce.Message,
                Amount = amount,
                Error = true,
                IdSeller = idSeller,
                Type = type
            };
        }

        public ResourceTransferResult TransferResources(string idSender, string idRecipient, string type, ulong amount)
        {
            var resultReduce = ReduceResource(idSender, type, amount);
            if (!resultReduce.Error)
            {
                var resultAdd = AddResource(idRecipient, type, amount);
                if (!resultAdd.Error)
                {
                    resultReduce.Message = $"Successfully transferred {amount} of {type} from {idSender} to {idRecipient}.";
                    var currentYear = _database.getJObjectAsync("CurrentYear", "system").Result.GetValue("value")
                        .ToObject<int>();
                    var jsonTransactionHistory = new ResourceTransaction(idSender,
                        idRecipient,
                        type,
                        amount,
                        currentYear).SerializeIntoJObject();
                    _database.SetJObjectAsync(jsonTransactionHistory, "resource_transaction_history", true);
                    return resultReduce;
                }

                return resultAdd;
            }

            return resultReduce;
        }

        //automagically saves to DB
        public ResourceTransferResult ReduceResource(string id, string type, ulong amount)
        {
            try
            {
                var resource = new Resource(GetResources(id));
                var result = new ResourceTransferResult
                {
                    IdSender = id,
                    Type = type,
                    Amount = amount,
                    Message = $"Successfully reduced {type} in the account of User or Company with ID {id} by {amount}.",
                    Error = false
                };

                switch (type)
                {
                    case "MNRL":
                        if (resource.Minerals >= amount)
                        {
                            resource.Minerals -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return result;
                        }
                        else
                        {
                            result.Message = $"Couldn't reduce {type} by {amount}, insufficient resources.";
                            result.Error = true;
                            return result;
                        }
                    case "FOOD":
                        if (resource.Food >= amount)
                        {
                            resource.Food -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return result;
                        }
                        else
                        {
                            result.Message = $"Couldn't reduce {type} by {amount}, insufficient resources.";
                            result.Error = true;
                            return result;
                        }
                    case "ALLY":
                        if (resource.Alloys >= amount)
                        {
                            resource.Alloys -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return result;
                        }
                        else
                        {
                            result.Message = $"Couldn't reduce {type} by {amount}, insufficient resources.";
                            result.Error = true;
                            return result;
                        }
                    case "CSGD":
                        if (resource.ConsumerGoods >= amount)
                        {
                            resource.ConsumerGoods -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return result;
                        }
                        else
                        {
                            result.Message = $"Couldn't reduce {type} by {amount}, insufficient resources.";
                            result.Error = true;
                            return result;
                        }
                    case "RFML":
                        if (resource.RefinedMinerals >= amount)
                        {
                            resource.RefinedMinerals -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return result;
                        }
                        else
                        {
                            result.Message = $"Couldn't reduce {type} by {amount}, insufficient resources.";
                            result.Error = true;
                            return result;
                        }
                    case "RFFD":
                        if (resource.RefinedFood >= amount)
                        {
                            resource.RefinedFood -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return result;
                        }
                        else
                        {
                            result.Message = $"Couldn't reduce {type} by {amount}, insufficient resources.";
                            result.Error = true;
                            return result;
                        }
                }
                return new ResourceTransferResult
                {
                    IdSender = id,
                    Type = type,
                    Amount = amount,
                    Message = "Resource type wasn't valid. Please choose from: \nMNRL - Minerals \nFOOD - Food \nALLY - Alloys \nRFML - Refined Minerals \nRFFD - Refined Food",
                    Error = true
                };
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                //in case it doesn't work
                return new ResourceTransferResult
                {
                    IdSender = id,
                    Type = type,
                    Amount = amount,
                    Message = "Error executing code, please contact a dev.",
                    Error = true
                };
            }
        }

        public ResourceTransferResult AddResource(string id, string type, ulong amount)
        {
            try
            {
                var resource = new Resource(GetResources(id));

                switch (type)
                {
                    case "MNRL": resource.Minerals += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "FOOD": resource.Food += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "ALLY": resource.Alloys += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "CSGD": resource.ConsumerGoods += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "RFML": resource.RefinedMinerals += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "RFFD": resource.RefinedFood += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    default:
                        return new ResourceTransferResult
                        {
                            IdSender = id,
                            Type = type,
                            Amount = amount,
                            Message = "Resource type wasn't valid. Please choose from: \nMNRL - Minerals \nFOOD - Food \nALLY - Alloys \nRFML - Refined Minerals \nRFFD - Refined Food",
                            Error = true
                        };
                }
                return new ResourceTransferResult
                {
                    IdSender = id,
                    Type = type,
                    Amount = amount,
                    Message = $"Successfully increased {type} in the account of User or Company with ID {id} by {amount}.",
                    Error = false
                };
            }
            catch(Exception e)
            {
                Console.Write(e.StackTrace);
                //in case it doesn't work
                return new ResourceTransferResult
                {
                    IdSender = id,
                    Type = type,
                    Amount = amount,
                    Message = "Error executing code, please contact a dev.",
                    Error = true
                };
            }
        }
    }
}
