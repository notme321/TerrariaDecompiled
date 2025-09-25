namespace Terraria.DataStructures;

public struct TileReachCheckSettings
{
	public int TileRangeMultiplier;

	public int? TileReachLimit;

	public int? OverrideXReach;

	public int? OverrideYReach;

	public static TileReachCheckSettings Simple => new TileReachCheckSettings
	{
		TileRangeMultiplier = 1,
		TileReachLimit = 20
	};

	public static TileReachCheckSettings Pylons => new TileReachCheckSettings
	{
		OverrideXReach = 60,
		OverrideYReach = 60
	};

	public static TileReachCheckSettings QuickStackToNearbyChests => new TileReachCheckSettings
	{
		OverrideXReach = 39,
		OverrideYReach = 39
	};

	public void GetRanges(Player player, out int x, out int y)
	{
		x = Player.tileRangeX * TileRangeMultiplier;
		y = Player.tileRangeY * TileRangeMultiplier;
		int? tileReachLimit = TileReachLimit;
		if (tileReachLimit.HasValue)
		{
			int value = tileReachLimit.Value;
			if (x > value)
			{
				x = value;
			}
			if (y > value)
			{
				y = value;
			}
		}
		if (OverrideXReach.HasValue)
		{
			x = OverrideXReach.Value;
		}
		if (OverrideYReach.HasValue)
		{
			y = OverrideYReach.Value;
		}
	}
}
