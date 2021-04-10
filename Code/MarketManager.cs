using Accord.Statistics.Distributions.Univariate;
using Godot;
using System;
using System.Collections.Generic;

public class MarketManager : Node
{
  public enum CustomerUpdateType { Searching, Found, NotFound }

  [Signal]
  public delegate void EmitCustomerUpdateSprite(string name, CustomerUpdateType type);

  class MarketItem
  {
    string name;
    float basePrice;
    float demand;
    float RRP;

    public MarketItem(string _name, float _basePrice, float _demand)
    {
      name = _name;
      basePrice = _basePrice;
      demand = _demand;
      RRP = demand * basePrice;
    }

    public float GetBasePrice()
    {
      return basePrice;
    }

    public string GetName()
    {
      return name;
    }

    public float GetRRP()
    {
      return RRP;
    }

    public float GetDemand()
    {
      return demand;
    }

    public void UpdateDemand(float change)
    {
      demand *= change;
      RRP = demand * basePrice;
    }
  }

  class Customer
  {
    public float wealthValue;
    public MarketItem desiredItem1;
    public MarketItem desiredItem2;
    public MarketItem backupItem;
    public bool[] GotItem = { false, false };
    public float appearanceTime;
    public float queueTime;

    public Customer(float _wealthValue, MarketItem _desiredItem1,
    MarketItem _desiredItem2,
    MarketItem _backupItem, float _appearanceTime)
    {
      wealthValue = _wealthValue;
      desiredItem1 = _desiredItem1;
      desiredItem2 = _desiredItem2;
      backupItem = _backupItem;
      appearanceTime = _appearanceTime;
      queueTime = appearanceTime + 10f;

      //GD.Print(wealthValue+" "+desiredItem1.GetName()+" "+desiredItem2.GetName()+" "+backupItem.GetName() +" "+appearanceTime);
    }
  }

  class SellerItemSlot
  {
    public MarketItem item;
    public int count;
    public float price;

    public SellerItemSlot(MarketItem _item, float _price)
    {
      price = _price;
      item = _item;
      UniformDiscreteDistribution dist = new UniformDiscreteDistribution(5, 10);
      count = dist.Generate();
    }
  }

  class Seller
  {
    public List<SellerItemSlot> items;

    public Seller(List<SellerItemSlot> _items)
    {
      items = _items;
    }
  }

  /////////////
  //VARIABLES//
  /////////////

  List<Customer> customers = new List<Customer>();
  List<MarketItem> itemsList = new List<MarketItem>();
  int customerCount = 100;
  List<Seller> sellers = new List<Seller>();
  public bool isActive = true;
  int nextCustomerIndex;
  Queue<Customer> customerFirstItemQueue = new Queue<Customer>();
  Queue<Customer> customerSecondItemQueue = new Queue<Customer>();
  Queue<Customer> customerBackupItemQueue = new Queue<Customer>();

  public override void _Ready()
  {
    foreach(KeyValuePair<string,PlantManager.PlantType> plant in ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).plants)
    {
      itemsList.Add(new MarketItem(plant.Value.fruitName, plant.Value.basePrice, 1));
    }
    
    StartDay();
  }

  public void StartDay()
  {
    nextCustomerIndex = 0;
    List<float> itemSampleChart = new List<float>();
    customers = new List<Customer>();

    float runningSum = 0;
    foreach (MarketItem item in itemsList)
    {
      runningSum += item.GetDemand();
      itemSampleChart.Add(runningSum);
    }

    UniformContinuousDistribution uniformDistWealth = new UniformContinuousDistribution(0.5, 1.5);
    UniformContinuousDistribution uniformDistItems = new UniformContinuousDistribution(0, runningSum);
    UniformDiscreteDistribution randomItem = new UniformDiscreteDistribution(0, itemsList.Count);
    SkewNormalDistribution dist = new SkewNormalDistribution(DayNightCycle.dayLength*0.7, DayNightCycle.dayLength * 0.2, -2);

    for (int i = 0; i < customerCount; i++)
    {
      float wealth = (float)(uniformDistWealth.Generate());

      float itemChooser = (float)(uniformDistItems.Generate());
      int index;
      for (index = 0; index < itemsList.Count; index++)
      {
        if (itemChooser < itemSampleChart[index])
        {
          break;
        }
      }
      MarketItem desiredItem1 = itemsList[index];

      itemChooser = (float)(uniformDistItems.Generate());
      for (index = 0; index < itemsList.Count; index++)
      {
        if (itemChooser < itemSampleChart[index])
        {
          break;
        }
      }
      MarketItem desiredItem2 = itemsList[index];
      MarketItem backupItem = itemsList[randomItem.Generate()];

      customers.Add(new Customer(wealth, desiredItem1, desiredItem2, backupItem, (float)dist.Generate()));
    }

    List<int> buckets = new List<int>();
    List<int> count = new List<int>(100);
    for (int i = 0; i < DayNightCycle.dayLength; i++)
    {

      count.Add(0);
      buckets.Add(i + 1);
    }

    foreach (Customer customer in customers)
    {
      foreach (int bucket in buckets)
      {
        if (customer.appearanceTime < bucket)
        {
          count[bucket - 1]++;
          break;
        }
      }
    }

    customers.Sort((x, y) => x.appearanceTime.CompareTo(y.appearanceTime));

    sellers = new List<Seller>();

    //Create sellers
    for (int i = 0; i < 5; i++)
    {
      List<SellerItemSlot> sellerItems = new List<SellerItemSlot>();

      for (int j = 0; j < 5; j++)
      {
        float itemChooser = (float)(uniformDistItems.Generate());
        int index;
        for (index = 0; index < itemsList.Count; index++)
        {
          if (itemChooser < itemSampleChart[index])
          {
            break;
          }
        }

        NormalDistribution itemPriceRRPDist = new NormalDistribution(itemsList[index].GetRRP(), 2);
        NormalDistribution itemPriceBaseDist = new NormalDistribution(itemsList[index].GetBasePrice() * 0.8, 2);

        float price = Math.Max((float)itemPriceRRPDist.Generate(), (float)itemPriceBaseDist.Generate());

        SellerItemSlot item = new SellerItemSlot(itemsList[index], price);

        sellerItems.Add(item);
      }
      sellers.Add(new Seller(sellerItems));
    }
    GD.Print("");
  }

  void ProcessCustomer(Customer customer, MarketItem currentWantedItem, int itemIndex)
  {
    UniformContinuousDistribution desiredDist = new UniformContinuousDistribution(0.9, 1.1);
    UniformContinuousDistribution backupDist = new UniformContinuousDistribution(0.6, 0.8);

    float currentWantedItemMaxPrice;

    if (itemIndex!=2)
    {
      currentWantedItemMaxPrice = (currentWantedItem.GetRRP()) * (float)desiredDist.Generate();
    }
    else
    {
      currentWantedItemMaxPrice = (currentWantedItem.GetRRP()) * (float)backupDist.Generate();
    }

    //TODO TURN THIS INTO FUNCTION INSTEAD OF COPYPASTE
    //First desired item
    int sellerIndex = -1;
    int sellerItemIndex = -1;
    float sellerPrice = 200000;

    int sellerIndexTrack = 0;
    foreach (Seller seller in sellers)
    {
      int itemIndexTrack = 0;
      foreach (SellerItemSlot itemSlot in seller.items)
      {
        if (itemSlot.item.GetName() == currentWantedItem.GetName() && itemSlot.price < sellerPrice && itemSlot.price < currentWantedItemMaxPrice && itemSlot.count > 0)
        {
          sellerIndex = sellerIndexTrack;
          sellerItemIndex = itemIndexTrack;
        }
        itemIndexTrack++;
      }
      sellerIndexTrack++;
    }

    if (sellerIndex > -1)
    {
      if (itemIndex != 3)
      {
        customer.GotItem[itemIndex] = true;
      }
      sellers[sellerIndex].items[sellerItemIndex].count--;
    }
  }

  public void EndOfDay()
  {
    int desired1Count = 0;
    int desired2Count = 0;

    //Update demands
    foreach (Customer customer in customers)
    {
      if (customer.GotItem[0])
      {
        customer.desiredItem1.UpdateDemand(0.99f);
        desired1Count++;
      }
      else
      {
        customer.desiredItem1.UpdateDemand(1.01f);
      }

      if (customer.GotItem[1])
      {
        desired2Count++;
        customer.desiredItem2.UpdateDemand(0.99f);
      }
      else
      {
        customer.desiredItem2.UpdateDemand(1.01f);
      }
    }

    //Print results
    foreach (MarketItem item in itemsList)
    {
      GD.Print(item.GetName() + " demand = " + item.GetDemand() + " rrp = " + item.GetRRP());
    }
    GD.Print("desired counts: " + desired1Count + " " + desired2Count);
    GD.Print("");
  }

  public override void _Process(float delta)
  {
    if(isActive )
    {
      float timeOfDay = ((DayNightCycle)GetTree().CurrentScene.GetNode("TimeOfDayManager")).GetTimePassedToday();

      if(nextCustomerIndex<customerCount)
      {
      Customer nextCustomer = customers[nextCustomerIndex];

      if (nextCustomer.appearanceTime < timeOfDay)
      {
        //Process for first desired item and add to queue for second desired item
        nextCustomer.queueTime = nextCustomer.appearanceTime + 5;
        customerFirstItemQueue.Enqueue(nextCustomer);
        EmitSignal(nameof(EmitCustomerUpdateSprite), nextCustomer.desiredItem1.GetName(), CustomerUpdateType.Searching);
        nextCustomerIndex++;
      }
      }

      //Process first item customer
      if(customerFirstItemQueue.Count>0 && customerFirstItemQueue.  Peek().queueTime < timeOfDay)
      {
        ProcessCustomer(customerFirstItemQueue.Peek(), customerFirstItemQueue.Peek().desiredItem1, 0);
        customerFirstItemQueue.Peek().queueTime = timeOfDay + 10;
        if(customerFirstItemQueue.Peek().GotItem[0])
        {
          EmitSignal(nameof(EmitCustomerUpdateSprite), customerFirstItemQueue.Peek().desiredItem1.GetName(), CustomerUpdateType.Found);
        }
        else
        {
          EmitSignal(nameof(EmitCustomerUpdateSprite), customerFirstItemQueue.Peek().desiredItem1.GetName(), CustomerUpdateType.NotFound);
        }
        EmitSignal(nameof(EmitCustomerUpdateSprite), customerFirstItemQueue.Peek().desiredItem2.GetName(), CustomerUpdateType.Searching);
        customerSecondItemQueue.Enqueue(customerFirstItemQueue.Dequeue());
      }

      //Process second item customer
      if (customerSecondItemQueue.Count > 0 && customerSecondItemQueue.Peek().queueTime < timeOfDay)
      {
        ProcessCustomer(customerSecondItemQueue.Peek(), customerSecondItemQueue.Peek().desiredItem2, 1);

        if (customerFirstItemQueue.Peek().GotItem[1])
        {
          EmitSignal(nameof(EmitCustomerUpdateSprite), customerSecondItemQueue.Peek().desiredItem2.GetName(), CustomerUpdateType.Found);
        }
        else
        {
          EmitSignal(nameof(EmitCustomerUpdateSprite), customerSecondItemQueue.Peek().desiredItem2.GetName(), CustomerUpdateType.NotFound);
        }

        //If we didn't get both items
        if (customerSecondItemQueue.Peek().GotItem[0]== false || customerSecondItemQueue.Peek().GotItem[1] == false)
        {
          EmitSignal(nameof(EmitCustomerUpdateSprite), customerSecondItemQueue.Peek().backupItem.GetName(), CustomerUpdateType.Searching);
          //Add to backup item queue
          customerSecondItemQueue.Peek().queueTime += 5;
          customerBackupItemQueue.Enqueue(customerSecondItemQueue.Dequeue());
        }
        else
        {
          customerSecondItemQueue.Dequeue();
        }
      }
      //Process backup item customer
      if(customerBackupItemQueue.Count>0 && customerBackupItemQueue.Peek().queueTime < timeOfDay)
      {
        //TODO EMIT FOUND/NOT FOUND FOR BACKUP ITEM 
        ProcessCustomer(customerBackupItemQueue.Peek(), customerBackupItemQueue.Dequeue().backupItem, 2);
      }
    }
  }


}
