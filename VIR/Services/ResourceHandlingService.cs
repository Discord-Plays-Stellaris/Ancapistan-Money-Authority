using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VIR.Modules.Objects.Company;
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
            JObject resources;
            try
            {
                resources = _database.getJObjectAsync(id, "resources").Result;
            }
            catch (Exception e)
            {
                resources = new Resource(id).SerializeIntoJObject();
                SetResources(resources);
            }

            return resources;
        }

        public class TransactionToBeExecutedOnceChecked
        {
            public string IdBuyer;
            public string IdSeller;
            public ulong Amount;
            public double PricePerUnit;
        }

        public class MarketListingChangeToBeExecutedOnceChecked
        {
            public string ListingId;
            public ulong NewAmount;
        }

        public async Task<string> BuyResourceFromMarketAsCompanyAsync(string type, string idBuyer, ulong amount)
        {
            var buyer = new User(_database.getJObjectAsync(idBuyer, "companies").Result);
            var buyerResourceWallet = new Resource(_database.getJObjectAsync(idBuyer, "resources").Result);

            if (Math.Abs(buyer.Money) <= 0.0)
            {
                return "You have no credits.";
            }
            if (amount <= 0)
            {
                return "You ordered zero resources";
            }

            var openListingsJson = _database.getJObjects("resource_market_listings").Result.ToList();
            var openListings = new List<ResourceMarketListing>();

            // buffer for making the monetary transactions
            var transactionBuffer = new List<TransactionToBeExecutedOnceChecked>();
            // buffer for transferring the resources
            var listingChangesBuffer = new List<MarketListingChangeToBeExecutedOnceChecked>();

            //finds all relevant listings
            foreach (var olJson in openListingsJson)
            {
                var ol = new ResourceMarketListing(olJson);
                if (ol.Type == type)
                {
                    openListings.Add(ol);
                }
            }

            //sorts by price per unit
            openListings = openListings.OrderBy(x => x.Price).ToList();

            //total amount of credits spent and resources bought, usefull for when not enough resources were on the market
            double totalSpent = 0.0;
            ulong totalResourcesBought = 0L;

            var workingAmount = amount;
            foreach (var listing in openListings)
            {
                if (workingAmount > 0)
                {
                    // if the amount left to be bought is less than the current listing being processed
                    if (workingAmount < listing.Amount)
                    {
                        var totalPrice = workingAmount * listing.Price;
                        totalResourcesBought += workingAmount;
                        totalSpent += totalPrice;
                        listingChangesBuffer.Add(new MarketListingChangeToBeExecutedOnceChecked()
                        {
                            ListingId = listing.Id,
                            NewAmount = listing.Amount - workingAmount
                        });
                        transactionBuffer.Add(new TransactionToBeExecutedOnceChecked
                        {
                            Amount = workingAmount,
                            IdBuyer = idBuyer,
                            IdSeller = listing.IdSeller,
                            PricePerUnit = listing.Price
                        });
                    }
                    // if the amount left to be bought is more than or equal to the current listing being processed
                    else if (workingAmount >= listing.Amount)
                    {
                        var totalPrice = listing.Amount * listing.Price;
                        totalResourcesBought += listing.Amount;
                        totalSpent += totalPrice;
                        workingAmount -= listing.Amount;
                        listingChangesBuffer.Add(new MarketListingChangeToBeExecutedOnceChecked()
                        {
                            ListingId = listing.Id,
                            // 0  means remove
                            NewAmount = 0
                        });
                        transactionBuffer.Add(new TransactionToBeExecutedOnceChecked
                        {
                            Amount = workingAmount,
                            IdBuyer = idBuyer,
                            IdSeller = listing.IdSeller,
                            PricePerUnit = listing.Price
                        });
                    }
                }
                else
                {
                    // in case something else went wrong
                    return "Something went wrong in buy process. ResourceHandlingService.cs.";
                }
            }

            // Checks if the buyer has enough money
            if (buyer.Money < totalSpent)
            {
                return "You don't have enough money to buy this many resources.";
            }

            // Gives the buyer the resources they bought
            AddResource(idBuyer, type, totalResourcesBought);

            buyer.Money -= totalSpent;
            await _database.SetJObjectAsync(buyer.SerializeIntoJObject(), "companies");

            // does the monetary transactions
            foreach (var x in transactionBuffer)
            {
                if (x.IdSeller.Length <=3)
                {
                    var seller = new Company(_database.getJObjectAsync(x.IdSeller, "companies").Result);
                    seller.Money += (x.Amount * x.PricePerUnit);
                    await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "companies");
                }
                else
                {
                    var seller = new User(_database.getJObjectAsync(x.IdSeller, "users").Result);
                    seller.Money += (x.Amount * x.PricePerUnit);
                    await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "users");
                }
            }

            foreach (var t in listingChangesBuffer)
            {
                // changes the listing
                if (t.NewAmount > 0)
                {
                    var listing = new ResourceMarketListing(_database.getJObjectAsync(t.ListingId, "mresource_market_listings").Result);
                    listing.Amount = t.NewAmount;
                    await _database.SetJObjectAsync(listing.SerializeIntoJObject(), "resource_market_listings");
                }
                else
                {
                    await _database.RemoveObjectAsync(t.ListingId, "resource_market_listings");
                }
            }

            if (totalResourcesBought < amount)
            {
                return
                    $"Was only able to buy {totalResourcesBought} of type {type} for {totalSpent} because of a shortage on the market.";
            }

            return $"Successfully bought {totalResourcesBought} of type {type} for {totalSpent}.";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Type of resource</param>
        /// <param name="idBuyer">Id of the buyer</param>
        /// <param name="amount">Amount of resources to be bought</param>
        /// <returns></returns>
        public async Task<string> BuyResourceFromMarketAsUserAsync(string type, string idBuyer, ulong amount)
        {
            var buyer = new User(_database.getJObjectAsync(idBuyer, "users").Result);
            var buyerResourceWallet = new Resource(_database.getJObjectAsync(idBuyer, "resources").Result);

            if (Math.Abs(buyer.Money) <= 0.0)
            {
                return "You have no credits.";
            }
            if (amount <= 0)
            {
                return "You ordered zero resources";
            }

            var openListingsJson = _database.getJObjects("resource_market_listings").Result.ToList();
            var openListings = new List<ResourceMarketListing>();

            // buffer for making the monetary transactions
            var transactionBuffer = new List<TransactionToBeExecutedOnceChecked>();
            // buffer for transferring the resources
            var listingChangesBuffer = new List<MarketListingChangeToBeExecutedOnceChecked>();

            //finds all relevant listings
            foreach (var olJson in openListingsJson)
            {
                var ol = new ResourceMarketListing(olJson);
                if (ol.Type == type)
                {
                    openListings.Add(ol);
                }
            }
            
            //sorts by price per unit
            openListings = openListings.OrderBy(x => x.Price).ToList();

            //total amount of credits spent and resources bought, usefull for when not enough resources were on the market
            double totalSpent = 0.0;
            ulong totalResourcesBought = 0L;

            var workingAmount = amount;
            foreach (var listing in openListings)
            {
                if (workingAmount > 0)
                {
                    // if the amount left to be bought is less than the current listing being processed
                    if (workingAmount < listing.Amount)
                    {
                        var totalPrice = workingAmount * listing.Price;
                        totalResourcesBought += workingAmount;
                        totalSpent += totalPrice;
                        listingChangesBuffer.Add(new MarketListingChangeToBeExecutedOnceChecked()
                        {
                            ListingId = listing.Id,
                            NewAmount = listing.Amount - workingAmount
                        });
                        transactionBuffer.Add(new TransactionToBeExecutedOnceChecked
                        {
                            Amount = workingAmount,
                            IdBuyer = idBuyer,
                            IdSeller = listing.IdSeller,
                            PricePerUnit = listing.Price
                        });
                    }
                    // if the amount left to be bought is more than or equal to the current listing being processed
                    else if (workingAmount >= listing.Amount)
                    {
                        var totalPrice = listing.Amount * listing.Price;
                        totalResourcesBought += listing.Amount;
                        totalSpent += totalPrice;
                        workingAmount -= listing.Amount;
                        listingChangesBuffer.Add(new MarketListingChangeToBeExecutedOnceChecked()
                        {
                            ListingId = listing.Id,
                            // 0  means remove
                            NewAmount = 0
                        });
                        transactionBuffer.Add(new TransactionToBeExecutedOnceChecked
                        {
                            Amount = workingAmount,
                            IdBuyer = idBuyer,
                            IdSeller = listing.IdSeller,
                            PricePerUnit = listing.Price
                        });
                    }
                }
                else
                {
                    // in case something else went wrong
                    return "Something went wrong in buy process. ResourceHandlingService.cs.";
                }
            }

            // Checks if the buyer has enough money
            if (buyer.Money < totalSpent)
            {
                return "You don't have enough money to buy this many resources.";
            }

            // Gives the buyer the resources they bought
            AddResource(idBuyer, type, totalResourcesBought);

            buyer.Money -= totalSpent;
            await _database.SetJObjectAsync(buyer.SerializeIntoJObject(), "users");

            // does the monetary transactions
            foreach (var x in transactionBuffer)
            {
                if (x.IdSeller.Length <= 3)
                {
                    var seller = new Company(_database.getJObjectAsync(x.IdSeller, "companies").Result);
                    seller.Money += (x.Amount * x.PricePerUnit);
                    await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "companies");
                }
                else
                {
                    var seller = new User(_database.getJObjectAsync(x.IdSeller, "users").Result);
                    seller.Money += (x.Amount * x.PricePerUnit);
                    await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "users");
                }
            }

            foreach (var t in listingChangesBuffer)
            {
                // changes the listing
                if (t.NewAmount > 0)
                {
                    var listing = new ResourceMarketListing(_database.getJObjectAsync(t.ListingId, "resource_market_listings").Result);
                    listing.Amount = t.NewAmount;
                    await _database.SetJObjectAsync(listing.SerializeIntoJObject(), "resource_market_listings");
                }
                else
                {
                    await _database.RemoveObjectAsync(t.ListingId, "resource_market_listings");
                }
            }

            if (totalResourcesBought < amount)
            {
                return
                    $"Was only able to buy {totalResourcesBought} of type {type} for {totalSpent} because of a shortage on the market.";
            }

            return $"Successfully bought {totalResourcesBought} of type {type} for {totalSpent}.";
        }

        /// <summary>
        /// Buy resources from a specific listing as a company
        /// </summary>
        /// <param name="idListing"></param>
        /// <param name="idBuyer"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<string> BuyResourcesFromSpecificOfferOnMarketAsCompany(string idListing, string idBuyer, ulong amount)
        {
            // listing
            ResourceMarketListing listing;
            try
            {
                listing = new ResourceMarketListing(_database.getJObjectAsync(idListing, "resource_market_listings").Result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return $"Could not find listing with ID {idListing}";
            }
            var buyer = new Company(_database.getJObjectAsync(idBuyer, "company").Result);

            double totalPrice;

            //processing of transaction
            if (amount >= listing.Amount)
            {
                totalPrice = listing.Price * listing.Amount;
                if (totalPrice > buyer.Money)
                {
                    return "Insufficient funds.";
                }

                amount = listing.Amount;
                await _database.RemoveObjectAsync(idListing, "resource_market_listings");
                AddResource(idBuyer, listing.Type, amount);
            }
            else
            {
                totalPrice = listing.Price * listing.Amount;
                if (totalPrice > buyer.Money)
                {
                    return "Insufficient funds.";
                }
                listing.Amount -= amount;
                await _database.SetJObjectAsync(listing.SerializeIntoJObject(), "resource_market_listings");
                AddResource(idBuyer, listing.Type, amount);
            }

            // doing the money stuff
            if (listing.IdSeller.Length <=3)
            {
                var seller = new Company(_database.getJObjectAsync(listing.IdSeller, "companies").Result);
                seller.Money += (listing.Amount * listing.Price);
                await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "companies");
            }
            else
            {
                var seller = new User(_database.getJObjectAsync(listing.IdSeller, "users").Result);
                seller.Money += (listing.Amount * listing.Price);
                await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "users");
            }

            buyer.Money -= amount;
            await _database.SetJObjectAsync(buyer.SerializeIntoJObject(), "users");

            return $"Successfully bought {amount} of {listing.Type} for {totalPrice} from listing {listing.Id}.";
        }

        // buy resources from a specific listing as user
        public async Task<string> BuyResourcesFromSpecificOfferOnMarketAsUser(string idListing, string idBuyer, ulong amount)
        {
            // listing
            ResourceMarketListing listing;
            try
            {
                listing = new ResourceMarketListing(_database.getJObjectAsync(idListing, "resource_market_listings").Result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return $"Could not find listing with ID {idListing}";
            }
            var buyer = new User(_database.getJObjectAsync(idBuyer, "users").Result);

            double totalPrice;

            //processing of transaction
            if (amount >= listing.Amount)
            {
                totalPrice = listing.Price * listing.Amount;
                if (totalPrice > buyer.Money)
                {
                    return "Insufficient funds.";
                }

                amount = listing.Amount;
                await _database.RemoveObjectAsync(idListing, "resource_market_listings");
                AddResource(idBuyer, listing.Type, amount);
            }
            else
            {
                totalPrice = listing.Price * listing.Amount;
                if (totalPrice > buyer.Money)
                {
                    return "Insufficient funds.";
                }
                listing.Amount -= amount;
                await _database.SetJObjectAsync(listing.SerializeIntoJObject(), "resource_market_listings");
                AddResource(idBuyer, listing.Type, amount);
            }

            // doing the money stuff
            if (listing.IdSeller.Length <=3)
            {
                var seller = new Company(_database.getJObjectAsync(listing.IdSeller, "companies").Result);
                seller.Money += (listing.Amount * listing.Price);
                await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "companies");
            }
            else
            {
                var seller = new User(_database.getJObjectAsync(listing.IdSeller, "users").Result);
                seller.Money += (listing.Amount * listing.Price);
                await _database.SetJObjectAsync(seller.SerializeIntoJObject(), "users");
            }

            buyer.Money -= amount;
            await _database.SetJObjectAsync(buyer.SerializeIntoJObject(), "users");

            return $"Successfully bought {amount} of {listing.Type} for {totalPrice} from listing {listing.Id}.";
        }

        public async Task<string> PutResourceUpForSale(string idSeller, string type, ulong amount, double pricePerUnit)
        {
            var resultReduce = ReduceResource(idSeller, type, amount);
            var currentYear = _database.getJObjectAsync("CurrentYear", "system").Result.GetValue("value")
                .ToObject<int>();
            if (resultReduce)
            {
                var newListing = new ResourceMarketListing(type, idSeller, amount, pricePerUnit, currentYear);
                await _database.SetJObjectAsync(newListing.SerializeIntoJObject(), "resource_market_listings", true);
                return
                    $"Successfully put up a listing of {newListing.Amount} of {newListing.Type} for {newListing.Price} per unit.";
            }

            return "Something went wrong and the listing wasn't put up.";
        }

        public async Task<string> TransferResources(string idSender, string idRecipient, string type, ulong amount)
        {
            CheckIfResourceWalletExistsAndMakeNewOneIfItDoesNot(idRecipient);
            CheckIfResourceWalletExistsAndMakeNewOneIfItDoesNot(idSender);

            var resultReduce = ReduceResource(idSender, type, amount);
            if (resultReduce)
            {
                var resultAdd = AddResource(idRecipient, type, amount);
                if (resultAdd)
                {
                    var currentYear = _database.getJObjectAsync("CurrentYear", "system").Result.GetValue("value")
                        .ToObject<int>();
                    var jsonTransactionHistory = new ResourceTransaction(idSender,
                        idRecipient,
                        type,
                        amount,
                        currentYear).SerializeIntoJObject();
                    await _database.SetJObjectAsync(jsonTransactionHistory, "resource_transaction_history", true);
                    return $"Successfully transferred {amount} of {type}.";
                }

                return "Managed to subtract resources but not add them.";
            }

            return "Unable to subtract resources from account.";
        }

        public void CheckIfResourceWalletExistsAndMakeNewOneIfItDoesNot(string id)
        {
            try
            {
                var resource = new Resource(GetResources(id));

            }
            catch (Exception e)
            {
                var resource = new Resource(id);
                SetResources(resource.SerializeIntoJObject());
                throw;
            }
        }

        // hopefully done
        public bool ReduceResource(string id, string type, ulong amount)
        {
            try
            {
                var resource = new Resource(GetResources(id));

                switch (type)
                {
                    case "MNRL":
                        if (resource.Minerals >= amount)
                        {
                            resource.Minerals -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "FOOD":
                        if (resource.Food >= amount)
                        {
                            resource.Food -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "ALLY":
                        if (resource.Alloys >= amount)
                        {
                            resource.Alloys -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "CSGD":
                        if (resource.ConsumerGoods >= amount)
                        {
                            resource.ConsumerGoods -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "RFML":
                        if (resource.RefinedMinerals >= amount)
                        {
                            resource.RefinedMinerals -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case "RFFD":
                        if (resource.RefinedFood >= amount)
                        {
                            resource.RefinedFood -= amount;
                            SetResources(resource.SerializeIntoJObject());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    default: return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AddResource(string id, string type, ulong amount)
        {
            try
            {
                var resource = new Resource(GetResources(id));

                switch (type)
                {
                    case "MNRL":
                        resource.Minerals += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "FOOD":
                        resource.Food += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "ALLY":
                        resource.Alloys += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "CSGD":
                        resource.ConsumerGoods += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "RFML":
                        resource.RefinedMinerals += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    case "RFFD":
                        resource.RefinedFood += amount;
                        SetResources(resource.SerializeIntoJObject());
                        break;
                    default: return false;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                //in case it doesn't work
                return false;
            }

            return false;
        }
    }
}
