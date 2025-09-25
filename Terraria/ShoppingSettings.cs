namespace Terraria;

public struct ShoppingSettings
{
	public double PriceAdjustment;

	public string HappinessReport;

	public static ShoppingSettings NotInShop => new ShoppingSettings
	{
		PriceAdjustment = 1.0,
		HappinessReport = ""
	};
}
