using Godot;
using System;
using System.Collections.Generic;
using Accord.Statistics.Distributions.Univariate;

public class MarketManager : Node
{

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
  public bool GotItem1 = false;
	public bool GotItem2 = false;
	public float appearanceTime;

	public Customer(float _wealthValue, MarketItem _desiredItem1,
	MarketItem _desiredItem2,
	MarketItem _backupItem, float _appearanceTime)
	{
	  wealthValue = _wealthValue;
	  desiredItem1 = _desiredItem1;
	  desiredItem2 = _desiredItem2;
	  backupItem = _backupItem;
	  appearanceTime = _appearanceTime;

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

  public override void _Ready()
  {
	itemsList.Add(new MarketItem("Banana", 50, 1));
	itemsList.Add(new MarketItem("Apple", 40, 1));
	itemsList.Add(new MarketItem("Grape", 60, 1));
	itemsList.Add(new MarketItem("Cherry", 20, 1));
	itemsList.Add(new MarketItem("Mango", 200, 1));

	StartDay();
  }

  public void StartDay()
  {
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
	SkewNormalDistribution dist = new SkewNormalDistribution(420, 120, -2);

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
	for (int i = 0; i < 600; i++)
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
		NormalDistribution itemPriceBaseDist = new NormalDistribution(itemsList[index].GetBasePrice()*0.8, 2);

		float price = Math.Max((float)itemPriceRRPDist.Generate(),(float)itemPriceBaseDist.Generate());

		SellerItemSlot item = new SellerItemSlot(itemsList[index], price);

		sellerItems.Add(item);
	  }
	  sellers.Add(new Seller(sellerItems));
	}
	GD.Print("");
  }

  public void ProcessCustomers()
  {
					UniformContinuousDistribution desiredDist = new UniformContinuousDistribution(0.9, 1.1);
			UniformContinuousDistribution backupDist = new UniformContinuousDistribution(0.6, 0.8);
		foreach(Customer customer in customers)
		{
			float desiredItem1Price = (customer.desiredItem1.GetRRP())*(float)desiredDist.Generate();
			float desiredItem2Price =	(customer.desiredItem2.GetRRP())*(float)desiredDist.Generate();
			float backupItemPrice = (customer.desiredItem2.GetRRP())*(float)backupDist.Generate();

			int sellerIndex = -1;
			int sellerItemIndex = -1;
			float sellerPrice = 200000;


			//TODO TURN THIS INTO FUNCTION INSTEAD OF COPYPASTE
			int sellerIndexTrack = 0;
			foreach(Seller seller in sellers)
			{
				int itemIndexTrack = 0;
				foreach(SellerItemSlot itemSlot in seller.items)
				{
					if(itemSlot.item.GetName()==customer.desiredItem1.GetName() && itemSlot.price < sellerPrice && itemSlot.price < desiredItem1Price && itemSlot.count>0)
					{
						sellerIndex = sellerIndexTrack;
						sellerItemIndex = itemIndexTrack;
					}
					itemIndexTrack++;
				}
				sellerIndexTrack++;
			}

			if(sellerIndex>-1)
			{
				customer.GotItem1 = true;
				sellers[sellerIndex].items[sellerItemIndex].count--;
			}

			sellerIndex = -1;
			sellerItemIndex = -1;
			sellerPrice = 200000;

			
			sellerIndexTrack = 0;
			foreach(Seller seller in sellers)
			{
				int itemIndexTrack = 0;
				foreach(SellerItemSlot itemSlot in seller.items)
				{
					if(itemSlot.item.GetName()==customer.desiredItem2.GetName() && itemSlot.price < sellerPrice && itemSlot.price < desiredItem2Price && itemSlot.count>0)
					{
						sellerIndex = sellerIndexTrack;
						sellerItemIndex = itemIndexTrack;
					}
					itemIndexTrack++;
				}
				sellerIndexTrack++;
			}

			if(sellerIndex>-1)
			{
				customer.GotItem2 = true;
				sellers[sellerIndex].items[sellerItemIndex].count--;
			}

			sellerIndex = -1;
			sellerItemIndex = -1;
			sellerPrice = 200000;
			
			sellerIndexTrack = 0;
			foreach(Seller seller in sellers)
			{
				int itemIndexTrack = 0;
				foreach(SellerItemSlot itemSlot in seller.items)
				{
					if(itemSlot.item.GetName()==customer.backupItem.GetName() && itemSlot.price < sellerPrice && itemSlot.price < backupItemPrice && itemSlot.count>0)
					{
						sellerIndex = sellerIndexTrack;
						sellerItemIndex = itemIndexTrack;
					}
					itemIndexTrack++;
				}
				sellerIndexTrack++;
			}

			if(sellerIndex>-1)
			{
				sellers[sellerIndex].items[sellerItemIndex].count--;
			}

		}
		
		int desired1Count=0;
		int desired2Count=0;

		//Update demands
		foreach(Customer customer in customers)
		{
			if(customer.GotItem1)
			{
				customer.desiredItem1.UpdateDemand(0.99f);
				desired1Count++;
			}
			else
			{
				customer.desiredItem1.UpdateDemand(1.01f);
			}

			if(customer.GotItem2)
			{
				desired2Count++;
				customer.desiredItem2.UpdateDemand(0.99f);
			}
			else
			{
				customer.desiredItem2.UpdateDemand(1.01f);
			}
		}
	foreach(MarketItem item in itemsList)
	{
		GD.Print(item.GetName()+" demand = "+item.GetDemand()+ " rrp = "+item.GetRRP());
	}
	GD.Print("desired counts: "+desired1Count+" "+desired2Count);
	GD.Print("");

  }

  List<Customer> customers = new List<Customer>();
  List<MarketItem> itemsList = new List<MarketItem>();
  int customerCount = 100;
  List<Seller> sellers = new List<Seller>();

  public override void _Process(float delta)
  {
		StartDay();
		ProcessCustomers();
  }


}
