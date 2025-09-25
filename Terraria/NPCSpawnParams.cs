using Terraria.DataStructures;

namespace Terraria;

public struct NPCSpawnParams
{
	public float? sizeScaleOverride;

	public int? playerCountForMultiplayerDifficultyOverride;

	public GameModeData gameModeData;

	public float? strengthMultiplierOverride;

	public NPCSpawnParams WithScale(float scaleOverride)
	{
		return new NPCSpawnParams
		{
			sizeScaleOverride = scaleOverride,
			playerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride,
			gameModeData = gameModeData,
			strengthMultiplierOverride = strengthMultiplierOverride
		};
	}
}
