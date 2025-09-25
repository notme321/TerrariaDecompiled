using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Social;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.Initializers;

public class UILinksInitializer
{
	public class SomeVarsForUILinkers
	{
		public static Recipe SequencedCraftingCurrent;

		public static int HairMoveCD;
	}

	public static bool NothingMoreImportantThanNPCChat()
	{
		if (!Main.hairWindow && Main.npcShop == 0)
		{
			return Main.player[Main.myPlayer].chest == -1;
		}
		return false;
	}

	public static float HandleSliderHorizontalInput(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
	{
		float x = PlayerInput.GamepadThumbstickLeft.X;
		x = ((!(x < 0f - deadZone) && !(x > deadZone)) ? 0f : (MathHelper.Lerp(0f, sensitivity / 60f, (Math.Abs(x) - deadZone) / (1f - deadZone)) * (float)Math.Sign(x)));
		return MathHelper.Clamp((currentValue - min) / (max - min) + x, 0f, 1f) * (max - min) + min;
	}

	public static float HandleSliderVerticalInput(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
	{
		float num = 0f - PlayerInput.GamepadThumbstickLeft.Y;
		num = ((!(num < 0f - deadZone) && !(num > deadZone)) ? 0f : (MathHelper.Lerp(0f, sensitivity / 60f, (Math.Abs(num) - deadZone) / (1f - deadZone)) * (float)Math.Sign(num)));
		return MathHelper.Clamp((currentValue - min) / (max - min) + num, 0f, 1f) * (max - min) + min;
	}

	public static bool CanExecuteInputCommand()
	{
		return PlayerInput.AllowExecutionOfGamepadInstructions;
	}

	public static void Load()
	{
		Func<string> value = () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
		UILinkPage uILinkPage = new UILinkPage();
		uILinkPage.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int num = 0; num < 20; num++)
		{
			uILinkPage.LinkMap.Add(2000 + num, new UILinkPoint(2000 + num, enabled: true, -3, -4, -1, -2));
		}
		uILinkPage.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]);
		uILinkPage.UpdateEvent += delegate
		{
			bool flag = PlayerInput.Triggers.JustPressed.Inventory;
			if (Main.inputTextEscape)
			{
				Main.inputTextEscape = false;
				flag = true;
			}
			if (CanExecuteInputCommand() && flag)
			{
				FancyExit();
			}
			UILinkPointNavigator.Shortcuts.BackButtonInUse = flag;
			HandleOptionsSpecials();
		};
		uILinkPage.IsValidEvent += () => Main.gameMenu && !Main.MenuUI.IsVisible;
		uILinkPage.CanEnterEvent += () => Main.gameMenu && !Main.MenuUI.IsVisible;
		UILinkPointNavigator.RegisterPage(uILinkPage, 1000);
		UILinkPage cp = new UILinkPage();
		cp.LinkMap.Add(2500, new UILinkPoint(2500, enabled: true, -3, 2501, -1, -2));
		cp.LinkMap.Add(2501, new UILinkPoint(2501, enabled: true, 2500, 2502, -1, -2));
		cp.LinkMap.Add(2502, new UILinkPoint(2502, enabled: true, 2501, 2503, -1, -2));
		cp.LinkMap.Add(2503, new UILinkPoint(2503, enabled: true, 2502, -4, -1, -2));
		cp.UpdateEvent += delegate
		{
			cp.LinkMap[2501].Right = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : (-4));
			if (cp.LinkMap[2501].Right == -4 && UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2)
			{
				cp.LinkMap[2501].Right = 2503;
			}
			cp.LinkMap[2502].Right = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 ? 2503 : (-4));
			cp.LinkMap[2503].Left = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : 2501);
		};
		cp.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[56].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]);
		cp.IsValidEvent += () => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && NothingMoreImportantThanNPCChat();
		cp.CanEnterEvent += () => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && NothingMoreImportantThanNPCChat();
		cp.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		cp.LeaveEvent += delegate
		{
			Main.npcChatRelease = false;
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		UILinkPointNavigator.RegisterPage(cp, 1003);
		UILinkPage cp2 = new UILinkPage();
		cp2.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value2 = delegate
		{
			int currentPoint = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 0, currentPoint);
		};
		Func<string> value3 = () => ItemSlot.GetGamepadInstructions(ref Main.player[Main.myPlayer].trashItem, 6);
		for (int num2 = 0; num2 <= 49; num2++)
		{
			UILinkPoint uILinkPoint = new UILinkPoint(num2, enabled: true, num2 - 1, num2 + 1, num2 - 10, num2 + 10);
			uILinkPoint.OnSpecialInteracts += value2;
			int num3 = num2;
			if (num3 < 10)
			{
				uILinkPoint.Up = -1;
			}
			if (num3 >= 40)
			{
				uILinkPoint.Down = -2;
			}
			if (num3 % 10 == 9)
			{
				uILinkPoint.Right = -4;
			}
			if (num3 % 10 == 0)
			{
				uILinkPoint.Left = -3;
			}
			cp2.LinkMap.Add(num2, uILinkPoint);
		}
		cp2.LinkMap[9].Right = 0;
		cp2.LinkMap[19].Right = 50;
		cp2.LinkMap[29].Right = 51;
		cp2.LinkMap[39].Right = 52;
		cp2.LinkMap[49].Right = 53;
		cp2.LinkMap[0].Left = 9;
		cp2.LinkMap[10].Left = 54;
		cp2.LinkMap[20].Left = 55;
		cp2.LinkMap[30].Left = 56;
		cp2.LinkMap[40].Left = 57;
		cp2.LinkMap.Add(300, new UILinkPoint(300, enabled: true, 309, 310, 49, -2));
		cp2.LinkMap.Add(309, new UILinkPoint(309, enabled: true, 310, 300, 302, 54));
		cp2.LinkMap.Add(310, new UILinkPoint(310, enabled: true, 300, 309, 301, 50));
		cp2.LinkMap.Add(301, new UILinkPoint(301, enabled: true, 300, 302, 53, 50));
		cp2.LinkMap.Add(302, new UILinkPoint(302, enabled: true, 301, 310, 57, 54));
		cp2.LinkMap.Add(311, new UILinkPoint(311, enabled: true, -3, -4, 40, -2));
		cp2.LinkMap[301].OnSpecialInteracts += value;
		cp2.LinkMap[302].OnSpecialInteracts += value;
		cp2.LinkMap[309].OnSpecialInteracts += value;
		cp2.LinkMap[310].OnSpecialInteracts += value;
		cp2.LinkMap[300].OnSpecialInteracts += value3;
		cp2.UpdateEvent += delegate
		{
			bool inReforgeMenu = Main.InReforgeMenu;
			bool flag = Main.player[Main.myPlayer].chest != -1;
			bool flag2 = Main.npcShop != 0;
			TileEntity tileEntity = Main.LocalPlayer.tileEntityAnchor.GetTileEntity();
			bool flag3 = tileEntity is TEHatRack;
			bool flag4 = tileEntity is TEDisplayDoll;
			for (int i = 40; i <= 49; i++)
			{
				if (inReforgeMenu)
				{
					cp2.LinkMap[i].Down = ((i < 45) ? 303 : 304);
				}
				else if (flag)
				{
					cp2.LinkMap[i].Down = 400 + i - 40;
				}
				else if (flag2)
				{
					cp2.LinkMap[i].Down = 2700 + i - 40;
				}
				else if (i == 40)
				{
					cp2.LinkMap[i].Down = 311;
				}
				else
				{
					cp2.LinkMap[i].Down = -2;
				}
			}
			if (flag4)
			{
				for (int j = 40; j <= 47; j++)
				{
					cp2.LinkMap[j].Down = 5100 + j - 40;
				}
			}
			if (flag3)
			{
				for (int k = 44; k <= 45; k++)
				{
					cp2.LinkMap[k].Down = 5000 + k - 44;
				}
			}
			if (flag)
			{
				cp2.LinkMap[300].Up = 439;
				cp2.LinkMap[300].Right = -4;
				cp2.LinkMap[300].Left = 309;
				cp2.LinkMap[309].Up = 438;
				cp2.LinkMap[309].Right = 300;
				cp2.LinkMap[309].Left = 310;
				cp2.LinkMap[310].Up = 437;
				cp2.LinkMap[310].Right = 309;
				cp2.LinkMap[310].Left = -3;
			}
			else if (flag2)
			{
				cp2.LinkMap[300].Up = 2739;
				cp2.LinkMap[300].Right = -4;
				cp2.LinkMap[300].Left = 309;
				cp2.LinkMap[309].Up = 2738;
				cp2.LinkMap[309].Right = 300;
				cp2.LinkMap[309].Left = 310;
				cp2.LinkMap[310].Up = 2737;
				cp2.LinkMap[310].Right = 309;
				cp2.LinkMap[310].Left = -3;
			}
			else
			{
				cp2.LinkMap[49].Down = 300;
				cp2.LinkMap[48].Down = 309;
				cp2.LinkMap[47].Down = 310;
				cp2.LinkMap[300].Up = 49;
				cp2.LinkMap[300].Right = 301;
				cp2.LinkMap[300].Left = 309;
				cp2.LinkMap[309].Up = 48;
				cp2.LinkMap[309].Right = 300;
				cp2.LinkMap[309].Left = 310;
				cp2.LinkMap[310].Up = 47;
				cp2.LinkMap[310].Right = 309;
				cp2.LinkMap[310].Left = 302;
			}
			cp2.LinkMap[0].Left = 9;
			cp2.LinkMap[10].Left = 54;
			cp2.LinkMap[20].Left = 55;
			cp2.LinkMap[30].Left = 56;
			cp2.LinkMap[40].Left = 57;
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0)
			{
				cp2.LinkMap[0].Left = 6000;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 2)
			{
				cp2.LinkMap[10].Left = 6002;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 4)
			{
				cp2.LinkMap[20].Left = 6004;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 6)
			{
				cp2.LinkMap[30].Left = 6006;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 8)
			{
				cp2.LinkMap[40].Left = 6008;
			}
			cp2.PageOnLeft = 9;
			if (Main.CreativeMenu.Enabled)
			{
				cp2.PageOnLeft = 1005;
			}
			if (Main.InReforgeMenu)
			{
				cp2.PageOnLeft = 5;
			}
		};
		cp2.IsValidEvent += () => Main.playerInventory;
		cp2.PageOnLeft = 9;
		cp2.PageOnRight = 2;
		UILinkPointNavigator.RegisterPage(cp2, 0);
		UILinkPage cp3 = new UILinkPage();
		cp3.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value4 = delegate
		{
			int currentPoint = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 1, currentPoint);
		};
		for (int num4 = 50; num4 <= 53; num4++)
		{
			UILinkPoint uILinkPoint2 = new UILinkPoint(num4, enabled: true, -3, -4, num4 - 1, num4 + 1);
			uILinkPoint2.OnSpecialInteracts += value4;
			cp3.LinkMap.Add(num4, uILinkPoint2);
		}
		cp3.LinkMap[50].Left = 19;
		cp3.LinkMap[51].Left = 29;
		cp3.LinkMap[52].Left = 39;
		cp3.LinkMap[53].Left = 49;
		cp3.LinkMap[50].Right = 54;
		cp3.LinkMap[51].Right = 55;
		cp3.LinkMap[52].Right = 56;
		cp3.LinkMap[53].Right = 57;
		cp3.LinkMap[50].Up = -1;
		cp3.LinkMap[53].Down = -2;
		cp3.UpdateEvent += delegate
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				cp3.LinkMap[50].Up = 301;
				cp3.LinkMap[53].Down = 301;
			}
			else
			{
				cp3.LinkMap[50].Up = 504;
				cp3.LinkMap[53].Down = 500;
			}
		};
		cp3.IsValidEvent += () => Main.playerInventory;
		cp3.PageOnLeft = 0;
		cp3.PageOnRight = 2;
		UILinkPointNavigator.RegisterPage(cp3, 1);
		UILinkPage cp4 = new UILinkPage();
		cp4.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value5 = delegate
		{
			int currentPoint = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 2, currentPoint);
		};
		for (int num5 = 54; num5 <= 57; num5++)
		{
			UILinkPoint uILinkPoint3 = new UILinkPoint(num5, enabled: true, -3, -4, num5 - 1, num5 + 1);
			uILinkPoint3.OnSpecialInteracts += value5;
			cp4.LinkMap.Add(num5, uILinkPoint3);
		}
		cp4.LinkMap[54].Left = 50;
		cp4.LinkMap[55].Left = 51;
		cp4.LinkMap[56].Left = 52;
		cp4.LinkMap[57].Left = 53;
		cp4.LinkMap[54].Right = 10;
		cp4.LinkMap[55].Right = 20;
		cp4.LinkMap[56].Right = 30;
		cp4.LinkMap[57].Right = 40;
		cp4.LinkMap[54].Up = -1;
		cp4.LinkMap[57].Down = -2;
		cp4.UpdateEvent += delegate
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				cp4.LinkMap[54].Up = 302;
				cp4.LinkMap[57].Down = 302;
			}
			else
			{
				cp4.LinkMap[54].Up = 504;
				cp4.LinkMap[57].Down = 500;
			}
		};
		cp4.PageOnLeft = 0;
		cp4.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp4, 2);
		UILinkPage cp5 = new UILinkPage();
		cp5.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value6 = delegate
		{
			int num38 = UILinkPointNavigator.CurrentPoint - 100;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].armor, (num38 < 10) ? 8 : 9, num38);
		};
		Func<string> value7 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 120;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].dye, 12, slot);
		};
		for (int num6 = 100; num6 <= 119; num6++)
		{
			UILinkPoint uILinkPoint4 = new UILinkPoint(num6, enabled: true, num6 + 10, num6 - 10, num6 - 1, num6 + 1);
			uILinkPoint4.OnSpecialInteracts += value6;
			int num7 = num6 - 100;
			if (num7 == 0)
			{
				uILinkPoint4.Up = 305;
			}
			if (num7 == 10)
			{
				uILinkPoint4.Up = 306;
			}
			if (num7 == 9 || num7 == 19)
			{
				uILinkPoint4.Down = -2;
			}
			if (num7 >= 10)
			{
				uILinkPoint4.Left = 120 + num7 % 10;
			}
			else if (num7 >= 3)
			{
				uILinkPoint4.Right = -4;
			}
			else
			{
				uILinkPoint4.Right = 312 + num7;
			}
			cp5.LinkMap.Add(num6, uILinkPoint4);
		}
		for (int num8 = 120; num8 <= 129; num8++)
		{
			UILinkPoint uILinkPoint4 = new UILinkPoint(num8, enabled: true, -3, -10 + num8, num8 - 1, num8 + 1);
			uILinkPoint4.OnSpecialInteracts += value7;
			int num9 = num8 - 120;
			if (num9 == 0)
			{
				uILinkPoint4.Up = 307;
			}
			if (num9 == 9)
			{
				uILinkPoint4.Down = 308;
				uILinkPoint4.Left = 1557;
			}
			if (num9 == 8)
			{
				uILinkPoint4.Left = 1570;
			}
			cp5.LinkMap.Add(num8, uILinkPoint4);
		}
		for (int num10 = 312; num10 <= 314; num10++)
		{
			int num11 = num10 - 312;
			UILinkPoint uILinkPoint4 = new UILinkPoint(num10, enabled: true, 100 + num11, -4, num10 - 1, num10 + 1);
			if (num11 == 0)
			{
				uILinkPoint4.Up = -1;
			}
			if (num11 == 2)
			{
				uILinkPoint4.Down = -2;
			}
			uILinkPoint4.OnSpecialInteracts += value;
			cp5.LinkMap.Add(num10, uILinkPoint4);
		}
		cp5.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 0;
		cp5.UpdateEvent += delegate
		{
			int num38 = 107;
			int amountOfExtraAccessorySlotsToShow = Main.player[Main.myPlayer].GetAmountOfExtraAccessorySlotsToShow();
			for (int i = 0; i < amountOfExtraAccessorySlotsToShow; i++)
			{
				cp5.LinkMap[num38 + i].Down = num38 + i + 1;
				cp5.LinkMap[num38 - 100 + 120 + i].Down = num38 - 100 + 120 + i + 1;
				cp5.LinkMap[num38 + 10 + i].Down = num38 + 10 + i + 1;
			}
			cp5.LinkMap[num38 + amountOfExtraAccessorySlotsToShow].Down = 308;
			cp5.LinkMap[num38 + 10 + amountOfExtraAccessorySlotsToShow].Down = 308;
			cp5.LinkMap[num38 - 100 + 120 + amountOfExtraAccessorySlotsToShow].Down = 308;
			bool shouldPVPDraw = Main.ShouldPVPDraw;
			for (int j = 120; j <= 129; j++)
			{
				UILinkPoint uILinkPoint17 = cp5.LinkMap[j];
				int num39 = j - 120;
				uILinkPoint17.Left = -3;
				if (num39 == 0)
				{
					uILinkPoint17.Left = (shouldPVPDraw ? 1550 : (-3));
				}
				if (num39 == 1)
				{
					uILinkPoint17.Left = (shouldPVPDraw ? 1552 : (-3));
				}
				if (num39 == 2)
				{
					uILinkPoint17.Left = (shouldPVPDraw ? 1556 : (-3));
				}
				if (num39 == 3)
				{
					uILinkPoint17.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 1) ? 1558 : (-3));
				}
				if (num39 == 4)
				{
					uILinkPoint17.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 5) ? 1562 : (-3));
				}
				if (num39 == 5)
				{
					uILinkPoint17.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 9) ? 1566 : (-3));
				}
			}
			cp5.LinkMap[num38 - 100 + 120 + amountOfExtraAccessorySlotsToShow].Left = 1557;
			cp5.LinkMap[num38 - 100 + 120 + amountOfExtraAccessorySlotsToShow - 1].Left = 1570;
		};
		cp5.PageOnLeft = 8;
		cp5.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp5, 3);
		UILinkPage uILinkPage2 = new UILinkPage();
		uILinkPage2.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value8 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 400;
			int context = 4;
			Item[] item = Main.player[Main.myPlayer].bank.item;
			switch (Main.player[Main.myPlayer].chest)
			{
			case -1:
				return "";
			case -3:
				item = Main.player[Main.myPlayer].bank2.item;
				break;
			case -4:
				item = Main.player[Main.myPlayer].bank3.item;
				break;
			case -5:
				item = Main.player[Main.myPlayer].bank4.item;
				context = 32;
				break;
			default:
				item = Main.chest[Main.player[Main.myPlayer].chest].item;
				context = 3;
				break;
			case -2:
				break;
			}
			return ItemSlot.GetGamepadInstructions(item, context, slot);
		};
		for (int num12 = 400; num12 <= 439; num12++)
		{
			UILinkPoint uILinkPoint5 = new UILinkPoint(num12, enabled: true, num12 - 1, num12 + 1, num12 - 10, num12 + 10);
			uILinkPoint5.OnSpecialInteracts += value8;
			int num13 = num12 - 400;
			if (num13 < 10)
			{
				uILinkPoint5.Up = 40 + num13;
			}
			if (num13 >= 30)
			{
				uILinkPoint5.Down = -2;
			}
			if (num13 % 10 == 9)
			{
				uILinkPoint5.Right = -4;
			}
			if (num13 % 10 == 0)
			{
				uILinkPoint5.Left = -3;
			}
			uILinkPage2.LinkMap.Add(num12, uILinkPoint5);
		}
		uILinkPage2.LinkMap.Add(500, new UILinkPoint(500, enabled: true, 409, -4, 53, 501));
		uILinkPage2.LinkMap.Add(501, new UILinkPoint(501, enabled: true, 419, -4, 500, 502));
		uILinkPage2.LinkMap.Add(502, new UILinkPoint(502, enabled: true, 429, -4, 501, 503));
		uILinkPage2.LinkMap.Add(503, new UILinkPoint(503, enabled: true, 439, -4, 502, 505));
		uILinkPage2.LinkMap.Add(505, new UILinkPoint(505, enabled: true, 439, -4, 503, 504));
		uILinkPage2.LinkMap.Add(504, new UILinkPoint(504, enabled: true, 439, -4, 505, 50));
		uILinkPage2.LinkMap.Add(506, new UILinkPoint(506, enabled: true, 439, -4, 505, 50));
		uILinkPage2.LinkMap[500].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[501].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[502].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[503].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[504].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[505].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[506].OnSpecialInteracts += value;
		uILinkPage2.LinkMap[409].Right = 500;
		uILinkPage2.LinkMap[419].Right = 501;
		uILinkPage2.LinkMap[429].Right = 502;
		uILinkPage2.LinkMap[439].Right = 503;
		uILinkPage2.LinkMap[439].Down = 300;
		uILinkPage2.LinkMap[438].Down = 309;
		uILinkPage2.LinkMap[437].Down = 310;
		uILinkPage2.PageOnLeft = 0;
		uILinkPage2.PageOnRight = 0;
		uILinkPage2.DefaultPoint = 400;
		UILinkPointNavigator.RegisterPage(uILinkPage2, 4, automatedDefault: false);
		uILinkPage2.IsValidEvent += () => Main.playerInventory && Main.player[Main.myPlayer].chest != -1;
		UILinkPage uILinkPage3 = new UILinkPage();
		uILinkPage3.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value9 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 5100;
			return (!(Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEDisplayDoll tEDisplayDoll)) ? "" : tEDisplayDoll.GetItemGamepadInstructions(slot);
		};
		for (int num14 = 5100; num14 <= 5115; num14++)
		{
			UILinkPoint uILinkPoint6 = new UILinkPoint(num14, enabled: true, num14 - 1, num14 + 1, num14 - 8, num14 + 8);
			uILinkPoint6.OnSpecialInteracts += value9;
			int num15 = num14 - 5100;
			if (num15 < 8)
			{
				uILinkPoint6.Up = 40 + num15;
			}
			if (num15 >= 8)
			{
				uILinkPoint6.Down = -2;
			}
			if (num15 % 8 == 7)
			{
				uILinkPoint6.Right = -4;
			}
			if (num15 % 8 == 0)
			{
				uILinkPoint6.Left = -3;
			}
			uILinkPage3.LinkMap.Add(num14, uILinkPoint6);
		}
		uILinkPage3.PageOnLeft = 0;
		uILinkPage3.PageOnRight = 0;
		uILinkPage3.DefaultPoint = 5100;
		UILinkPointNavigator.RegisterPage(uILinkPage3, 20, automatedDefault: false);
		uILinkPage3.IsValidEvent += () => Main.playerInventory && Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEDisplayDoll;
		UILinkPage uILinkPage4 = new UILinkPage();
		uILinkPage4.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value10 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 5000;
			return (!(Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEHatRack tEHatRack)) ? "" : tEHatRack.GetItemGamepadInstructions(slot);
		};
		for (int num16 = 5000; num16 <= 5003; num16++)
		{
			UILinkPoint uILinkPoint7 = new UILinkPoint(num16, enabled: true, num16 - 1, num16 + 1, num16 - 2, num16 + 2);
			uILinkPoint7.OnSpecialInteracts += value10;
			int num17 = num16 - 5000;
			if (num17 < 2)
			{
				uILinkPoint7.Up = 44 + num17;
			}
			if (num17 >= 2)
			{
				uILinkPoint7.Down = -2;
			}
			if (num17 % 2 == 1)
			{
				uILinkPoint7.Right = -4;
			}
			if (num17 % 2 == 0)
			{
				uILinkPoint7.Left = -3;
			}
			uILinkPage4.LinkMap.Add(num16, uILinkPoint7);
		}
		uILinkPage4.PageOnLeft = 0;
		uILinkPage4.PageOnRight = 0;
		uILinkPage4.DefaultPoint = 5000;
		UILinkPointNavigator.RegisterPage(uILinkPage4, 21, automatedDefault: false);
		uILinkPage4.IsValidEvent += () => Main.playerInventory && Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEHatRack;
		UILinkPage uILinkPage5 = new UILinkPage();
		uILinkPage5.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value11 = delegate
		{
			if (Main.npcShop == 0)
			{
				return "";
			}
			int slot = UILinkPointNavigator.CurrentPoint - 2700;
			return ItemSlot.GetGamepadInstructions(Main.instance.shop[Main.npcShop].item, 15, slot);
		};
		for (int num18 = 2700; num18 <= 2739; num18++)
		{
			UILinkPoint uILinkPoint8 = new UILinkPoint(num18, enabled: true, num18 - 1, num18 + 1, num18 - 10, num18 + 10);
			uILinkPoint8.OnSpecialInteracts += value11;
			int num19 = num18 - 2700;
			if (num19 < 10)
			{
				uILinkPoint8.Up = 40 + num19;
			}
			if (num19 >= 30)
			{
				uILinkPoint8.Down = -2;
			}
			if (num19 % 10 == 9)
			{
				uILinkPoint8.Right = -4;
			}
			if (num19 % 10 == 0)
			{
				uILinkPoint8.Left = -3;
			}
			uILinkPage5.LinkMap.Add(num18, uILinkPoint8);
		}
		uILinkPage5.LinkMap[2739].Down = 300;
		uILinkPage5.LinkMap[2738].Down = 309;
		uILinkPage5.LinkMap[2737].Down = 310;
		uILinkPage5.PageOnLeft = 0;
		uILinkPage5.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(uILinkPage5, 13);
		uILinkPage5.IsValidEvent += () => Main.playerInventory && Main.npcShop != 0;
		UILinkPage cp6 = new UILinkPage();
		cp6.LinkMap.Add(303, new UILinkPoint(303, enabled: true, 304, 304, 40, -2));
		cp6.LinkMap.Add(304, new UILinkPoint(304, enabled: true, 303, 303, 40, -2));
		cp6.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value12 = () => ItemSlot.GetGamepadInstructions(ref Main.reforgeItem, 5);
		cp6.LinkMap[303].OnSpecialInteracts += value12;
		cp6.LinkMap[304].OnSpecialInteracts += () => Lang.misc[53].Value;
		cp6.UpdateEvent += delegate
		{
			if (Main.reforgeItem.type > 0)
			{
				cp6.LinkMap[303].Left = (cp6.LinkMap[303].Right = 304);
			}
			else
			{
				if (UILinkPointNavigator.OverridePoint == -1 && cp6.CurrentPoint == 304)
				{
					UILinkPointNavigator.ChangePoint(303);
				}
				cp6.LinkMap[303].Left = -3;
				cp6.LinkMap[303].Right = -4;
			}
		};
		cp6.IsValidEvent += () => Main.playerInventory && Main.InReforgeMenu;
		cp6.PageOnLeft = 0;
		cp6.PageOnRight = 0;
		cp6.EnterEvent += delegate
		{
			PlayerInput.LockGamepadButtons("MouseLeft");
		};
		UILinkPointNavigator.RegisterPage(cp6, 5);
		UILinkPage cp7 = new UILinkPage();
		cp7.OnSpecialInteracts += delegate
		{
			bool flag = UILinkPointNavigator.CurrentPoint == 600;
			bool flag2 = !flag && WorldGen.IsNPCEvictable(UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
			{
				Point point = Main.player[Main.myPlayer].Center.ToTileCoordinates();
				if (flag)
				{
					if (WorldGen.MoveTownNPC(point.X, point.Y, -1))
					{
						Main.NewText(Lang.inter[39].Value, byte.MaxValue, 240, 20);
					}
					SoundEngine.PlaySound(12);
				}
				else if (WorldGen.MoveTownNPC(point.X, point.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered))
				{
					WorldGen.moveRoom(point.X, point.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
					SoundEngine.PlaySound(12);
				}
				PlayerInput.LockGamepadButtons("Grapple");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.SmartSelect)
			{
				UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay = !UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay;
				PlayerInput.LockGamepadButtons("SmartSelect");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			if (flag2 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseRight)
			{
				WorldGen.kickOut(UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
			}
			return PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]) + PlayerInput.BuildCommand(Lang.misc[70].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]) + PlayerInput.BuildCommand(Lang.misc[69].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["SmartSelect"]) + (flag2 ? PlayerInput.BuildCommand("Evict", false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]) : "");
		};
		for (int num20 = 600; num20 <= 650; num20++)
		{
			UILinkPoint value13 = new UILinkPoint(num20, enabled: true, num20 + 10, num20 - 10, num20 - 1, num20 + 1);
			cp7.LinkMap.Add(num20, value13);
		}
		cp7.UpdateEvent += delegate
		{
			int num38 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
			if (num38 == 0)
			{
				num38 = 100;
			}
			for (int i = 0; i < 50; i++)
			{
				cp7.LinkMap[600 + i].Up = ((i % num38 == 0) ? (-1) : (600 + i - 1));
				if (cp7.LinkMap[600 + i].Up == -1)
				{
					if (i >= num38 * 2)
					{
						cp7.LinkMap[600 + i].Up = 307;
					}
					else if (i >= num38)
					{
						cp7.LinkMap[600 + i].Up = 306;
					}
					else
					{
						cp7.LinkMap[600 + i].Up = 305;
					}
				}
				cp7.LinkMap[600 + i].Down = (((i + 1) % num38 == 0 || i == UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - 1) ? 308 : (600 + i + 1));
				cp7.LinkMap[600 + i].Left = ((i < UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - num38) ? (600 + i + num38) : (-3));
				cp7.LinkMap[600 + i].Right = ((i < num38) ? (-4) : (600 + i - num38));
			}
		};
		cp7.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 1;
		cp7.PageOnLeft = 8;
		cp7.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp7, 6);
		UILinkPage cp8 = new UILinkPage();
		cp8.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value14 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 20, slot);
		};
		Func<string> value15 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 19, slot);
		};
		Func<string> value16 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 18, slot);
		};
		Func<string> value17 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 17, slot);
		};
		Func<string> value18 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 16, slot);
		};
		Func<string> value19 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 185;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscDyes, 33, slot);
		};
		for (int num21 = 180; num21 <= 184; num21++)
		{
			UILinkPoint uILinkPoint9 = new UILinkPoint(num21, enabled: true, 185 + num21 - 180, -4, num21 - 1, num21 + 1);
			int num22 = num21 - 180;
			if (num22 == 0)
			{
				uILinkPoint9.Up = 305;
			}
			if (num22 == 4)
			{
				uILinkPoint9.Down = 308;
			}
			cp8.LinkMap.Add(num21, uILinkPoint9);
			switch (num21)
			{
			case 180:
				uILinkPoint9.OnSpecialInteracts += value15;
				break;
			case 181:
				uILinkPoint9.OnSpecialInteracts += value14;
				break;
			case 182:
				uILinkPoint9.OnSpecialInteracts += value16;
				break;
			case 183:
				uILinkPoint9.OnSpecialInteracts += value17;
				break;
			case 184:
				uILinkPoint9.OnSpecialInteracts += value18;
				break;
			}
		}
		for (int num23 = 185; num23 <= 189; num23++)
		{
			UILinkPoint uILinkPoint9 = new UILinkPoint(num23, enabled: true, -3, -5 + num23, num23 - 1, num23 + 1);
			uILinkPoint9.OnSpecialInteracts += value19;
			int num24 = num23 - 185;
			if (num24 == 0)
			{
				uILinkPoint9.Up = 306;
			}
			if (num24 == 4)
			{
				uILinkPoint9.Down = 308;
			}
			cp8.LinkMap.Add(num23, uILinkPoint9);
		}
		cp8.UpdateEvent += delegate
		{
			cp8.LinkMap[184].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
			cp8.LinkMap[189].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
		};
		cp8.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 2;
		cp8.PageOnLeft = 8;
		cp8.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp8, 7);
		UILinkPage cp9 = new UILinkPage();
		cp9.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp9.LinkMap.Add(305, new UILinkPoint(305, enabled: true, 306, -4, 308, -2));
		cp9.LinkMap.Add(306, new UILinkPoint(306, enabled: true, 307, 305, 308, -2));
		cp9.LinkMap.Add(307, new UILinkPoint(307, enabled: true, -3, 306, 308, -2));
		cp9.LinkMap.Add(308, new UILinkPoint(308, enabled: true, -3, -4, -1, 305));
		cp9.LinkMap[305].OnSpecialInteracts += value;
		cp9.LinkMap[306].OnSpecialInteracts += value;
		cp9.LinkMap[307].OnSpecialInteracts += value;
		cp9.LinkMap[308].OnSpecialInteracts += value;
		cp9.UpdateEvent += delegate
		{
			switch (Main.EquipPage)
			{
			case 0:
				cp9.LinkMap[305].Down = 100;
				cp9.LinkMap[306].Down = 110;
				cp9.LinkMap[307].Down = 120;
				cp9.LinkMap[308].Up = 108 + Main.player[Main.myPlayer].GetAmountOfExtraAccessorySlotsToShow() - 1;
				break;
			case 1:
			{
				cp9.LinkMap[305].Down = 600;
				cp9.LinkMap[306].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 0) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn) : (-2));
				cp9.LinkMap[307].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 1) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn * 2) : (-2));
				int num38 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
				if (num38 == 0)
				{
					num38 = 100;
				}
				if (num38 == 100)
				{
					num38 = UILinkPointNavigator.Shortcuts.NPCS_IconsTotal;
				}
				cp9.LinkMap[308].Up = 600 + num38 - 1;
				break;
			}
			case 2:
				cp9.LinkMap[305].Down = 180;
				cp9.LinkMap[306].Down = 185;
				cp9.LinkMap[307].Down = -2;
				cp9.LinkMap[308].Up = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 184);
				break;
			}
			cp9.PageOnRight = GetCornerWrapPageIdFromRightToLeft();
		};
		cp9.IsValidEvent += () => Main.playerInventory;
		cp9.PageOnLeft = 0;
		cp9.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp9, 8);
		UILinkPage cp10 = new UILinkPage();
		cp10.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value20 = () => ItemSlot.GetGamepadInstructions(ref Main.guideItem, 7);
		Func<string> HandleItem2 = () => (Main.mouseItem.type < 1) ? "" : ItemSlot.GetGamepadInstructions(ref Main.mouseItem, 22);
		for (int num25 = 1500; num25 < 1550; num25++)
		{
			UILinkPoint uILinkPoint10 = new UILinkPoint(num25, enabled: true, num25, num25, -1, -2);
			if (num25 != 1500)
			{
				uILinkPoint10.OnSpecialInteracts += HandleItem2;
			}
			cp10.LinkMap.Add(num25, uILinkPoint10);
		}
		cp10.LinkMap[1500].OnSpecialInteracts += value20;
		cp10.UpdateEvent += delegate
		{
			int num38 = UILinkPointNavigator.Shortcuts.CRAFT_CurrentIngredientsCount;
			int num39 = num38;
			if (Main.numAvailableRecipes > 0)
			{
				num39 += 2;
			}
			if (num38 < num39)
			{
				num38 = num39;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp10.CurrentPoint > 1500 + num38)
			{
				UILinkPointNavigator.ChangePoint(1500);
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp10.CurrentPoint == 1500 && !Main.InGuideCraftMenu)
			{
				UILinkPointNavigator.ChangePoint(1501);
			}
			for (int i = 1; i < num38; i++)
			{
				cp10.LinkMap[1500 + i].Left = 1500 + i - 1;
				cp10.LinkMap[1500 + i].Right = ((i == num38 - 2) ? (-4) : (1500 + i + 1));
			}
			cp10.LinkMap[1501].Left = -3;
			if (num38 > 0)
			{
				cp10.LinkMap[1500 + num38 - 1].Right = -4;
			}
			cp10.LinkMap[1500].Down = ((num38 >= 2) ? 1502 : (-2));
			cp10.LinkMap[1500].Left = ((num38 >= 1) ? 1501 : (-3));
			cp10.LinkMap[1502].Up = (Main.InGuideCraftMenu ? 1500 : (-1));
		};
		cp10.LinkMap[1501].OnSpecialInteracts += delegate
		{
			if (Main.InGuideCraftMenu)
			{
				return "";
			}
			string text = "";
			Player player = Main.player[Main.myPlayer];
			bool flag = false;
			Item createItem = Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem;
			if (Main.mouseItem.type == 0 && createItem.maxStack > 1 && player.ItemSpace(createItem).CanTakeItemToPersonalInventory && !player.HasLockedInventory())
			{
				flag = true;
				if (CanExecuteInputCommand() && PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
				{
					if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
					{
						SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[Main.focusRecipe]];
					}
					ItemSlot.RefreshStackSplitCooldown();
					Main.preventStackSplitReset = true;
					if (SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[Main.focusRecipe]])
					{
						Main.CraftItem(Main.recipe[Main.availableRecipe[Main.focusRecipe]]);
						Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, new GetItemSettings(LongText: false, NoText: true));
					}
				}
			}
			else if (Main.mouseItem.type > 0 && Main.mouseItem.maxStack == 1 && ItemSlot.Equippable(ref Main.mouseItem))
			{
				text += PlayerInput.BuildCommand(Lang.misc[67].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
				{
					ItemSlot.SwapEquip(ref Main.mouseItem);
					if (Main.player[Main.myPlayer].ItemSpace(Main.mouseItem).CanTakeItemToPersonalInventory)
					{
						Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, GetItemSettings.InventoryUIToInventorySettings);
					}
					PlayerInput.LockGamepadButtons("Grapple");
					PlayerInput.SettingsForUI.TryRevertingToMouseMode();
				}
			}
			bool flag2 = Main.mouseItem.stack <= 0;
			if (flag2 || (Main.mouseItem.type == createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack))
			{
				text = ((!flag2) ? (text + PlayerInput.BuildCommand(Lang.misc[72].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"])) : (text + PlayerInput.BuildCommand(Lang.misc[72].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"], PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"])));
			}
			if (!flag2 && Main.mouseItem.type == createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
			{
				text += PlayerInput.BuildCommand(Lang.misc[93].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
			}
			if (flag)
			{
				text += PlayerInput.BuildCommand(Lang.misc[71].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
			}
			return text + HandleItem2();
		};
		cp10.ReachEndEvent += delegate(int current, int next)
		{
			switch (current)
			{
			case 1501:
				switch (next)
				{
				case -1:
					if (Main.focusRecipe > 0)
					{
						Main.focusRecipe--;
					}
					break;
				case -2:
					if (Main.focusRecipe < Main.numAvailableRecipes - 1)
					{
						Main.focusRecipe++;
					}
					break;
				}
				break;
			default:
				switch (next)
				{
				case -1:
					if (Main.focusRecipe > 0)
					{
						UILinkPointNavigator.ChangePoint(1501);
						Main.focusRecipe--;
					}
					break;
				case -2:
					if (Main.focusRecipe < Main.numAvailableRecipes - 1)
					{
						UILinkPointNavigator.ChangePoint(1501);
						Main.focusRecipe++;
					}
					break;
				}
				break;
			case 1500:
				break;
			}
		};
		cp10.EnterEvent += delegate
		{
			Main.recBigList = false;
			PlayerInput.LockGamepadButtons("MouseLeft");
		};
		cp10.CanEnterEvent += () => Main.playerInventory && (Main.numAvailableRecipes > 0 || Main.InGuideCraftMenu);
		cp10.IsValidEvent += () => Main.playerInventory && (Main.numAvailableRecipes > 0 || Main.InGuideCraftMenu);
		cp10.PageOnLeft = 10;
		cp10.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp10, 9);
		UILinkPage cp11 = new UILinkPage();
		cp11.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num26 = 700; num26 < 1500; num26++)
		{
			UILinkPoint uILinkPoint11 = new UILinkPoint(num26, enabled: true, num26, num26, num26, num26);
			int IHateLambda = num26;
			uILinkPoint11.OnSpecialInteracts += delegate
			{
				string text = "";
				bool flag = false;
				Player player = Main.player[Main.myPlayer];
				if (IHateLambda + Main.recStart < Main.numAvailableRecipes)
				{
					int num38 = Main.recStart + IHateLambda - 700;
					if (Main.mouseItem.type == 0 && Main.recipe[Main.availableRecipe[num38]].createItem.maxStack > 1 && player.ItemSpace(Main.recipe[Main.availableRecipe[num38]].createItem).CanTakeItemToPersonalInventory && !player.HasLockedInventory())
					{
						flag = true;
						if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
						{
							SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[num38]];
						}
						if (CanExecuteInputCommand() && PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
						{
							ItemSlot.RefreshStackSplitCooldown();
							if (SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[num38]])
							{
								Main.CraftItem(Main.recipe[Main.availableRecipe[num38]]);
								Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, GetItemSettings.InventoryUIToInventorySettings);
							}
						}
					}
				}
				text += PlayerInput.BuildCommand(Lang.misc[73].Value, !flag, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
				if (flag)
				{
					text += PlayerInput.BuildCommand(Lang.misc[71].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				}
				return text;
			};
			cp11.LinkMap.Add(num26, uILinkPoint11);
		}
		cp11.UpdateEvent += delegate
		{
			int num38 = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
			int cRAFT_IconsPerColumn = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerColumn;
			if (num38 == 0)
			{
				num38 = 100;
			}
			int num39 = num38 * cRAFT_IconsPerColumn;
			if (num39 > 800)
			{
				num39 = 800;
			}
			if (num39 > Main.numAvailableRecipes)
			{
				num39 = Main.numAvailableRecipes;
			}
			for (int i = 0; i < num39; i++)
			{
				cp11.LinkMap[700 + i].Left = ((i % num38 == 0) ? (-3) : (700 + i - 1));
				cp11.LinkMap[700 + i].Right = (((i + 1) % num38 == 0 || i == Main.numAvailableRecipes - 1) ? (-4) : (700 + i + 1));
				cp11.LinkMap[700 + i].Down = ((i < num39 - num38) ? (700 + i + num38) : (-2));
				cp11.LinkMap[700 + i].Up = ((i < num38) ? (-1) : (700 + i - num38));
			}
			cp11.PageOnLeft = GetCornerWrapPageIdFromLeftToRight();
		};
		cp11.ReachEndEvent += delegate(int current, int next)
		{
			int cRAFT_IconsPerRow = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
			switch (next)
			{
			case -1:
				Main.recStart -= cRAFT_IconsPerRow;
				if (Main.recStart < 0)
				{
					Main.recStart = 0;
				}
				break;
			case -2:
				Main.recStart += cRAFT_IconsPerRow;
				SoundEngine.PlaySound(12);
				if (Main.recStart > Main.numAvailableRecipes - cRAFT_IconsPerRow)
				{
					Main.recStart = Main.numAvailableRecipes - cRAFT_IconsPerRow;
				}
				break;
			}
		};
		cp11.EnterEvent += delegate
		{
			Main.recBigList = true;
		};
		cp11.LeaveEvent += delegate
		{
			Main.recBigList = false;
		};
		cp11.CanEnterEvent += () => Main.playerInventory && Main.numAvailableRecipes > 0;
		cp11.IsValidEvent += () => Main.playerInventory && Main.recBigList && Main.numAvailableRecipes > 0;
		cp11.PageOnLeft = 0;
		cp11.PageOnRight = 9;
		UILinkPointNavigator.RegisterPage(cp11, 10);
		UILinkPage cp12 = new UILinkPage();
		cp12.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num27 = 2605; num27 < 2620; num27++)
		{
			UILinkPoint uILinkPoint12 = new UILinkPoint(num27, enabled: true, num27, num27, num27, num27);
			uILinkPoint12.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[73].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
			cp12.LinkMap.Add(num27, uILinkPoint12);
		}
		cp12.UpdateEvent += delegate
		{
			int num38 = 5;
			int num39 = 3;
			int num40 = num38 * num39;
			int count = Main.Hairstyles.AvailableHairstyles.Count;
			for (int i = 0; i < num40; i++)
			{
				cp12.LinkMap[2605 + i].Left = ((i % num38 == 0) ? (-3) : (2605 + i - 1));
				cp12.LinkMap[2605 + i].Right = (((i + 1) % num38 == 0 || i == count - 1) ? (-4) : (2605 + i + 1));
				cp12.LinkMap[2605 + i].Down = ((i < num40 - num38) ? (2605 + i + num38) : (-2));
				cp12.LinkMap[2605 + i].Up = ((i < num38) ? (-1) : (2605 + i - num38));
			}
		};
		cp12.ReachEndEvent += delegate(int current, int next)
		{
			int num38 = 5;
			switch (next)
			{
			case -1:
				Main.hairStart -= num38;
				SoundEngine.PlaySound(12);
				break;
			case -2:
				Main.hairStart += num38;
				SoundEngine.PlaySound(12);
				break;
			}
		};
		cp12.CanEnterEvent += () => Main.hairWindow;
		cp12.IsValidEvent += () => Main.hairWindow;
		cp12.PageOnLeft = 12;
		cp12.PageOnRight = 12;
		UILinkPointNavigator.RegisterPage(cp12, 11);
		UILinkPage uILinkPage6 = new UILinkPage();
		uILinkPage6.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage6.LinkMap.Add(2600, new UILinkPoint(2600, enabled: true, -3, -4, -1, 2601));
		uILinkPage6.LinkMap.Add(2601, new UILinkPoint(2601, enabled: true, -3, -4, 2600, 2602));
		uILinkPage6.LinkMap.Add(2602, new UILinkPoint(2602, enabled: true, -3, -4, 2601, 2603));
		uILinkPage6.LinkMap.Add(2603, new UILinkPoint(2603, enabled: true, -3, 2604, 2602, -2));
		uILinkPage6.LinkMap.Add(2604, new UILinkPoint(2604, enabled: true, 2603, -4, 2602, -2));
		uILinkPage6.UpdateEvent += delegate
		{
			Vector3 value22 = Main.rgbToHsl(Main.selColor);
			float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
			float x = PlayerInput.GamepadThumbstickLeft.X;
			x = ((!(x < 0f - interfaceDeadzoneX) && !(x > interfaceDeadzoneX)) ? 0f : (MathHelper.Lerp(0f, 1f / 120f, (Math.Abs(x) - interfaceDeadzoneX) / (1f - interfaceDeadzoneX)) * (float)Math.Sign(x)));
			int currentPoint = UILinkPointNavigator.CurrentPoint;
			if (currentPoint == 2600)
			{
				Main.hBar = MathHelper.Clamp(Main.hBar + x, 0f, 1f);
			}
			if (currentPoint == 2601)
			{
				Main.sBar = MathHelper.Clamp(Main.sBar + x, 0f, 1f);
			}
			if (currentPoint == 2602)
			{
				Main.lBar = MathHelper.Clamp(Main.lBar + x, 0.15f, 1f);
			}
			Vector3.Clamp(value22, Vector3.Zero, Vector3.One);
			if (x != 0f)
			{
				if (Main.hairWindow)
				{
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
				}
				SoundEngine.PlaySound(12);
			}
		};
		uILinkPage6.CanEnterEvent += () => Main.hairWindow;
		uILinkPage6.IsValidEvent += () => Main.hairWindow;
		uILinkPage6.PageOnLeft = 11;
		uILinkPage6.PageOnRight = 11;
		UILinkPointNavigator.RegisterPage(uILinkPage6, 12);
		UILinkPage cp13 = new UILinkPage();
		for (int num28 = 0; num28 < 30; num28++)
		{
			cp13.LinkMap.Add(2900 + num28, new UILinkPoint(2900 + num28, enabled: true, -3, -4, -1, -2));
			cp13.LinkMap[2900 + num28].OnSpecialInteracts += value;
		}
		cp13.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp13.TravelEvent += delegate
		{
			if (UILinkPointNavigator.CurrentPage == cp13.ID)
			{
				int num38 = cp13.CurrentPoint - 2900;
				if (num38 < 5)
				{
					IngameOptions.category = num38;
				}
			}
		};
		cp13.UpdateEvent += delegate
		{
			int num38 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_LEFT;
			if (num38 == 0)
			{
				num38 = 5;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp13.CurrentPoint < 2930 && cp13.CurrentPoint > 2900 + num38 - 1)
			{
				UILinkPointNavigator.ChangePoint(2900);
			}
			for (int i = 2900; i < 2900 + num38; i++)
			{
				cp13.LinkMap[i].Up = i - 1;
				cp13.LinkMap[i].Down = i + 1;
			}
			cp13.LinkMap[2900].Up = 2900 + num38 - 1;
			cp13.LinkMap[2900 + num38 - 1].Down = 2900;
			int num39 = cp13.CurrentPoint - 2900;
			if (num39 < 4 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseLeft)
			{
				IngameOptions.category = num39;
				UILinkPointNavigator.ChangePage(1002);
			}
			int num40 = ((SocialAPI.Network != null && SocialAPI.Network.CanInvite()) ? 1 : 0);
			if (num39 == 4 + num40 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseLeft)
			{
				UILinkPointNavigator.ChangePage(1004);
			}
		};
		cp13.EnterEvent += delegate
		{
			cp13.CurrentPoint = 2900 + IngameOptions.category;
		};
		cp13.PageOnLeft = (cp13.PageOnRight = 1002);
		cp13.IsValidEvent += () => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible;
		cp13.CanEnterEvent += () => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible;
		UILinkPointNavigator.RegisterPage(cp13, 1001);
		UILinkPage cp14 = new UILinkPage();
		for (int num29 = 0; num29 < 30; num29++)
		{
			cp14.LinkMap.Add(2930 + num29, new UILinkPoint(2930 + num29, enabled: true, -3, -4, -1, -2));
			cp14.LinkMap[2930 + num29].OnSpecialInteracts += value;
		}
		cp14.EnterEvent += delegate
		{
			Main.mouseLeftRelease = false;
		};
		cp14.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp14.UpdateEvent += delegate
		{
			int num38 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_RIGHT;
			if (num38 == 0)
			{
				num38 = 5;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp14.CurrentPoint >= 2930 && cp14.CurrentPoint > 2930 + num38 - 1)
			{
				UILinkPointNavigator.ChangePoint(2930);
			}
			for (int i = 2930; i < 2930 + num38; i++)
			{
				cp14.LinkMap[i].Up = i - 1;
				cp14.LinkMap[i].Down = i + 1;
			}
			cp14.LinkMap[2930].Up = -1;
			cp14.LinkMap[2930 + num38 - 1].Down = -2;
			HandleOptionsSpecials();
		};
		cp14.PageOnLeft = (cp14.PageOnRight = 1001);
		cp14.IsValidEvent += () => Main.ingameOptionsWindow;
		cp14.CanEnterEvent += () => Main.ingameOptionsWindow;
		UILinkPointNavigator.RegisterPage(cp14, 1002);
		UILinkPage cp15 = new UILinkPage();
		cp15.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num30 = 1550; num30 < 1558; num30++)
		{
			UILinkPoint uILinkPoint13 = new UILinkPoint(num30, enabled: true, -3, -4, -1, -2);
			switch (num30)
			{
			case 1551:
			case 1553:
			case 1555:
				uILinkPoint13.Up = uILinkPoint13.ID - 2;
				uILinkPoint13.Down = uILinkPoint13.ID + 2;
				uILinkPoint13.Right = uILinkPoint13.ID + 1;
				break;
			case 1552:
			case 1554:
			case 1556:
				uILinkPoint13.Up = uILinkPoint13.ID - 2;
				uILinkPoint13.Down = uILinkPoint13.ID + 2;
				uILinkPoint13.Left = uILinkPoint13.ID - 1;
				break;
			}
			cp15.LinkMap.Add(num30, uILinkPoint13);
		}
		cp15.LinkMap[1550].Down = 1551;
		cp15.LinkMap[1550].Right = 120;
		cp15.LinkMap[1550].Up = 307;
		cp15.LinkMap[1551].Up = 1550;
		cp15.LinkMap[1552].Up = 1550;
		cp15.LinkMap[1552].Right = 121;
		cp15.LinkMap[1554].Right = 121;
		cp15.LinkMap[1555].Down = 1570;
		cp15.LinkMap[1556].Down = 1570;
		cp15.LinkMap[1556].Right = 122;
		cp15.LinkMap[1557].Up = 1570;
		cp15.LinkMap[1557].Down = 308;
		cp15.LinkMap[1557].Right = 127;
		cp15.LinkMap.Add(1570, new UILinkPoint(1570, enabled: true, -3, -4, -1, -2));
		cp15.LinkMap[1570].Up = 1555;
		cp15.LinkMap[1570].Down = 1557;
		cp15.LinkMap[1570].Right = 126;
		for (int num31 = 0; num31 < 7; num31++)
		{
			cp15.LinkMap[1550 + num31].OnSpecialInteracts += value;
		}
		cp15.UpdateEvent += delegate
		{
			if (!Main.ShouldPVPDraw)
			{
				if (UILinkPointNavigator.OverridePoint == -1 && cp15.CurrentPoint != 1557 && cp15.CurrentPoint != 1570)
				{
					UILinkPointNavigator.ChangePoint(1557);
				}
				cp15.LinkMap[1570].Up = -1;
				cp15.LinkMap[1557].Down = 308;
				cp15.LinkMap[1557].Right = 127;
			}
			else
			{
				cp15.LinkMap[1570].Up = 1555;
				cp15.LinkMap[1557].Down = 308;
				cp15.LinkMap[1557].Right = 127;
			}
			int iNFOACCCOUNT = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
			if (iNFOACCCOUNT > 0)
			{
				cp15.LinkMap[1570].Up = 1558 + (iNFOACCCOUNT - 1) / 2 * 2;
			}
			if (Main.ShouldPVPDraw)
			{
				if (iNFOACCCOUNT >= 1)
				{
					cp15.LinkMap[1555].Down = 1558;
					cp15.LinkMap[1556].Down = 1558;
				}
				else
				{
					cp15.LinkMap[1555].Down = 1570;
					cp15.LinkMap[1556].Down = 1570;
				}
				if (iNFOACCCOUNT >= 2)
				{
					cp15.LinkMap[1556].Down = 1559;
				}
				else
				{
					cp15.LinkMap[1556].Down = 1570;
				}
			}
		};
		cp15.IsValidEvent += () => Main.playerInventory;
		cp15.PageOnLeft = 8;
		cp15.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp15, 16);
		UILinkPage cp16 = new UILinkPage();
		cp16.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num32 = 1558; num32 < 1570; num32++)
		{
			UILinkPoint uILinkPoint14 = new UILinkPoint(num32, enabled: true, -3, -4, -1, -2);
			uILinkPoint14.OnSpecialInteracts += value;
			switch (num32)
			{
			case 1559:
			case 1561:
			case 1563:
				uILinkPoint14.Up = uILinkPoint14.ID - 2;
				uILinkPoint14.Down = uILinkPoint14.ID + 2;
				uILinkPoint14.Right = uILinkPoint14.ID + 1;
				break;
			case 1560:
			case 1562:
			case 1564:
				uILinkPoint14.Up = uILinkPoint14.ID - 2;
				uILinkPoint14.Down = uILinkPoint14.ID + 2;
				uILinkPoint14.Left = uILinkPoint14.ID - 1;
				break;
			}
			cp16.LinkMap.Add(num32, uILinkPoint14);
		}
		cp16.UpdateEvent += delegate
		{
			int iNFOACCCOUNT = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
			if (UILinkPointNavigator.OverridePoint == -1 && cp16.CurrentPoint - 1558 >= iNFOACCCOUNT)
			{
				UILinkPointNavigator.ChangePoint(1558 + iNFOACCCOUNT - 1);
			}
			for (int i = 0; i < iNFOACCCOUNT; i++)
			{
				bool flag = i % 2 == 0;
				int num38 = i + 1558;
				cp16.LinkMap[num38].Down = ((i < iNFOACCCOUNT - 2) ? (num38 + 2) : 1570);
				cp16.LinkMap[num38].Up = ((i > 1) ? (num38 - 2) : (Main.ShouldPVPDraw ? (flag ? 1555 : 1556) : (-1)));
				cp16.LinkMap[num38].Right = ((flag && i + 1 < iNFOACCCOUNT) ? (num38 + 1) : (123 + i / 4));
				cp16.LinkMap[num38].Left = (flag ? (-3) : (num38 - 1));
			}
		};
		cp16.IsValidEvent += () => Main.playerInventory && UILinkPointNavigator.Shortcuts.INFOACCCOUNT > 0;
		cp16.PageOnLeft = 8;
		cp16.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp16, 17);
		UILinkPage cp17 = new UILinkPage();
		cp17.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num33 = 6000; num33 < 6012; num33++)
		{
			UILinkPoint uILinkPoint15 = new UILinkPoint(num33, enabled: true, -3, -4, -1, -2);
			switch (num33)
			{
			case 6000:
				uILinkPoint15.Right = 0;
				break;
			case 6001:
			case 6002:
				uILinkPoint15.Right = 10;
				break;
			case 6003:
			case 6004:
				uILinkPoint15.Right = 20;
				break;
			case 6005:
			case 6006:
				uILinkPoint15.Right = 30;
				break;
			default:
				uILinkPoint15.Right = 40;
				break;
			}
			cp17.LinkMap.Add(num33, uILinkPoint15);
		}
		cp17.UpdateEvent += delegate
		{
			int bUILDERACCCOUNT = UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT;
			if (UILinkPointNavigator.OverridePoint == -1 && cp17.CurrentPoint - 6000 >= bUILDERACCCOUNT)
			{
				UILinkPointNavigator.ChangePoint(6000 + bUILDERACCCOUNT - 1);
			}
			for (int i = 0; i < bUILDERACCCOUNT; i++)
			{
				_ = i % 2;
				int num38 = i + 6000;
				cp17.LinkMap[num38].Down = ((i < bUILDERACCCOUNT - 1) ? (num38 + 1) : (-2));
				cp17.LinkMap[num38].Up = ((i > 0) ? (num38 - 1) : (-1));
			}
		};
		cp17.IsValidEvent += () => Main.playerInventory && UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0;
		cp17.PageOnLeft = 8;
		cp17.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp17, 18);
		UILinkPage uILinkPage7 = new UILinkPage();
		uILinkPage7.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage7.LinkMap.Add(2806, new UILinkPoint(2806, enabled: true, 2805, 2807, -1, 2808));
		uILinkPage7.LinkMap.Add(2807, new UILinkPoint(2807, enabled: true, 2806, 2810, -1, 2809));
		uILinkPage7.LinkMap.Add(2808, new UILinkPoint(2808, enabled: true, 2805, 2809, 2806, -2));
		uILinkPage7.LinkMap.Add(2809, new UILinkPoint(2809, enabled: true, 2808, 2811, 2807, -2));
		uILinkPage7.LinkMap.Add(2810, new UILinkPoint(2810, enabled: true, 2807, -4, -1, 2811));
		uILinkPage7.LinkMap.Add(2811, new UILinkPoint(2811, enabled: true, 2809, -4, 2810, -2));
		uILinkPage7.LinkMap.Add(2805, new UILinkPoint(2805, enabled: true, -3, 2806, -1, -2));
		uILinkPage7.LinkMap[2806].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2807].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2808].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2809].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2805].OnSpecialInteracts += value;
		uILinkPage7.CanEnterEvent += () => Main.clothesWindow;
		uILinkPage7.IsValidEvent += () => Main.clothesWindow;
		uILinkPage7.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		uILinkPage7.LeaveEvent += delegate
		{
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		uILinkPage7.PageOnLeft = 15;
		uILinkPage7.PageOnRight = 15;
		UILinkPointNavigator.RegisterPage(uILinkPage7, 14);
		UILinkPage uILinkPage8 = new UILinkPage();
		uILinkPage8.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, true, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage8.LinkMap.Add(2800, new UILinkPoint(2800, enabled: true, -3, -4, -1, 2801));
		uILinkPage8.LinkMap.Add(2801, new UILinkPoint(2801, enabled: true, -3, -4, 2800, 2802));
		uILinkPage8.LinkMap.Add(2802, new UILinkPoint(2802, enabled: true, -3, -4, 2801, 2803));
		uILinkPage8.LinkMap.Add(2803, new UILinkPoint(2803, enabled: true, -3, 2804, 2802, -2));
		uILinkPage8.LinkMap.Add(2804, new UILinkPoint(2804, enabled: true, 2803, -4, 2802, -2));
		uILinkPage8.LinkMap[2800].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2801].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2802].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2803].OnSpecialInteracts += value;
		uILinkPage8.LinkMap[2804].OnSpecialInteracts += value;
		uILinkPage8.UpdateEvent += delegate
		{
			Vector3 value22 = Main.rgbToHsl(Main.selColor);
			float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
			float x = PlayerInput.GamepadThumbstickLeft.X;
			x = ((!(x < 0f - interfaceDeadzoneX) && !(x > interfaceDeadzoneX)) ? 0f : (MathHelper.Lerp(0f, 1f / 120f, (Math.Abs(x) - interfaceDeadzoneX) / (1f - interfaceDeadzoneX)) * (float)Math.Sign(x)));
			int currentPoint = UILinkPointNavigator.CurrentPoint;
			if (currentPoint == 2800)
			{
				Main.hBar = MathHelper.Clamp(Main.hBar + x, 0f, 1f);
			}
			if (currentPoint == 2801)
			{
				Main.sBar = MathHelper.Clamp(Main.sBar + x, 0f, 1f);
			}
			if (currentPoint == 2802)
			{
				Main.lBar = MathHelper.Clamp(Main.lBar + x, 0.15f, 1f);
			}
			Vector3.Clamp(value22, Vector3.Zero, Vector3.One);
			if (x != 0f)
			{
				if (Main.clothesWindow)
				{
					Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
					switch (Main.selClothes)
					{
					case 0:
						Main.player[Main.myPlayer].shirtColor = Main.selColor;
						break;
					case 1:
						Main.player[Main.myPlayer].underShirtColor = Main.selColor;
						break;
					case 2:
						Main.player[Main.myPlayer].pantsColor = Main.selColor;
						break;
					case 3:
						Main.player[Main.myPlayer].shoeColor = Main.selColor;
						break;
					}
				}
				SoundEngine.PlaySound(12);
			}
		};
		uILinkPage8.CanEnterEvent += () => Main.clothesWindow;
		uILinkPage8.IsValidEvent += () => Main.clothesWindow;
		uILinkPage8.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		uILinkPage8.LeaveEvent += delegate
		{
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		uILinkPage8.PageOnLeft = 14;
		uILinkPage8.PageOnRight = 14;
		UILinkPointNavigator.RegisterPage(uILinkPage8, 15);
		UILinkPage cp18 = new UILinkPage();
		cp18.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int num34 = 3000; num34 <= 4999; num34++)
		{
			cp18.LinkMap.Add(num34, new UILinkPoint(num34, enabled: true, -3, -4, -1, -2));
		}
		cp18.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + FancyUISpecialInstructions();
		cp18.UpdateEvent += delegate
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Inventory)
			{
				FancyExit();
			}
			UILinkPointNavigator.Shortcuts.BackButtonInUse = false;
		};
		cp18.EnterEvent += delegate
		{
			cp18.CurrentPoint = 3002;
		};
		cp18.CanEnterEvent += () => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible;
		cp18.IsValidEvent += () => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible;
		cp18.OnPageMoveAttempt += OnFancyUIPageMoveAttempt;
		UILinkPointNavigator.RegisterPage(cp18, 1004);
		UILinkPage cp19 = new UILinkPage();
		cp19.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int num35 = 10000; num35 <= 11000; num35++)
		{
			cp19.LinkMap.Add(num35, new UILinkPoint(num35, enabled: true, -3, -4, -1, -2));
		}
		for (int num36 = 15000; num36 <= 15000; num36++)
		{
			cp19.LinkMap.Add(num36, new UILinkPoint(num36, enabled: true, -3, -4, -1, -2));
		}
		cp19.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + FancyUISpecialInstructions();
		cp19.UpdateEvent += delegate
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Inventory)
			{
				FancyExit();
			}
			UILinkPointNavigator.Shortcuts.BackButtonInUse = false;
		};
		cp19.EnterEvent += delegate
		{
			cp19.CurrentPoint = 10000;
		};
		cp19.CanEnterEvent += CanEnterCreativeMenu;
		cp19.IsValidEvent += CanEnterCreativeMenu;
		cp19.OnPageMoveAttempt += OnFancyUIPageMoveAttempt;
		cp19.PageOnLeft = 8;
		cp19.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp19, 1005);
		UILinkPage cp20 = new UILinkPage();
		cp20.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value21 = () => PlayerInput.BuildCommand(Lang.misc[94].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
		for (int num37 = 9000; num37 <= 9050; num37++)
		{
			UILinkPoint uILinkPoint16 = new UILinkPoint(num37, enabled: true, num37 + 10, num37 - 10, num37 - 1, num37 + 1);
			cp20.LinkMap.Add(num37, uILinkPoint16);
			uILinkPoint16.OnSpecialInteracts += value21;
		}
		cp20.UpdateEvent += delegate
		{
			int num38 = UILinkPointNavigator.Shortcuts.BUFFS_PER_COLUMN;
			if (num38 == 0)
			{
				num38 = 100;
			}
			for (int i = 0; i < 50; i++)
			{
				cp20.LinkMap[9000 + i].Up = ((i % num38 == 0) ? (-1) : (9000 + i - 1));
				if (cp20.LinkMap[9000 + i].Up == -1)
				{
					if (i >= num38)
					{
						cp20.LinkMap[9000 + i].Up = 184;
					}
					else
					{
						cp20.LinkMap[9000 + i].Up = 189;
					}
				}
				cp20.LinkMap[9000 + i].Down = (((i + 1) % num38 == 0 || i == UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - 1) ? 308 : (9000 + i + 1));
				cp20.LinkMap[9000 + i].Left = ((i < UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - num38) ? (9000 + i + num38) : (-3));
				cp20.LinkMap[9000 + i].Right = ((i < num38) ? (-4) : (9000 + i - num38));
			}
		};
		cp20.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 2 && UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0;
		cp20.PageOnLeft = 8;
		cp20.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp20, 19);
		UILinkPage uILinkPage9 = UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage];
		uILinkPage9.CurrentPoint = uILinkPage9.DefaultPoint;
		uILinkPage9.Enter();
	}

	private static bool CanEnterCreativeMenu()
	{
		if (Main.LocalPlayer.chest != -1)
		{
			return false;
		}
		if (Main.LocalPlayer.talkNPC != -1)
		{
			return false;
		}
		if (Main.playerInventory)
		{
			return Main.CreativeMenu.Enabled;
		}
		return false;
	}

	private static int GetCornerWrapPageIdFromLeftToRight()
	{
		return 8;
	}

	private static int GetCornerWrapPageIdFromRightToLeft()
	{
		if (Main.CreativeMenu.Enabled)
		{
			return 1005;
		}
		return 10;
	}

	private static void OnFancyUIPageMoveAttempt(int direction)
	{
		if (Main.MenuUI.CurrentState is UICharacterCreation uICharacterCreation)
		{
			uICharacterCreation.TryMovingCategory(direction);
		}
		if (UserInterface.ActiveInstance.CurrentState is UIBestiaryTest uIBestiaryTest)
		{
			uIBestiaryTest.TryMovingPages(direction);
		}
	}

	public static void FancyExit()
	{
		switch (UILinkPointNavigator.Shortcuts.BackButtonCommand)
		{
		case 1:
			SoundEngine.PlaySound(11);
			Main.menuMode = 0;
			break;
		case 2:
			SoundEngine.PlaySound(11);
			Main.menuMode = ((!Main.menuMultiplayer) ? 1 : 12);
			break;
		case 3:
			Main.menuMode = 0;
			IngameFancyUI.Close();
			break;
		case 4:
			SoundEngine.PlaySound(11);
			Main.menuMode = 11;
			break;
		case 5:
			SoundEngine.PlaySound(11);
			Main.menuMode = 11;
			break;
		case 6:
			UIVirtualKeyboard.Cancel();
			break;
		case 7:
			if (Main.MenuUI.CurrentState is IHaveBackButtonCommand haveBackButtonCommand)
			{
				haveBackButtonCommand.HandleBackButtonUsage();
			}
			break;
		}
	}

	public static string FancyUISpecialInstructions()
	{
		string text = "";
		int fANCYUI_SPECIAL_INSTRUCTIONS = UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS;
		if (fANCYUI_SPECIAL_INSTRUCTIONS == 1)
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.HotbarMinus)
			{
				UIVirtualKeyboard.CycleSymbols();
				PlayerInput.LockGamepadButtons("HotbarMinus");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[235].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"]);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseRight)
			{
				UIVirtualKeyboard.BackSpace();
				PlayerInput.LockGamepadButtons("MouseRight");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[236].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.SmartCursor)
			{
				UIVirtualKeyboard.Write(" ");
				PlayerInput.LockGamepadButtons("SmartCursor");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[238].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["SmartCursor"]);
			if (UIVirtualKeyboard.CanSubmit)
			{
				if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.HotbarPlus)
				{
					UIVirtualKeyboard.Submit();
					PlayerInput.LockGamepadButtons("HotbarPlus");
					PlayerInput.SettingsForUI.TryRevertingToMouseMode();
				}
				text += PlayerInput.BuildCommand(Lang.menu[237].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
			}
		}
		return text;
	}

	public static void HandleOptionsSpecials()
	{
		switch (UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE)
		{
		case 1:
			Main.bgScroll = (int)HandleSliderHorizontalInput(Main.bgScroll, 0f, 100f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 1f);
			Main.caveParallax = 1f - (float)Main.bgScroll / 500f;
			break;
		case 2:
			Main.musicVolume = HandleSliderHorizontalInput(Main.musicVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 3:
			Main.soundVolume = HandleSliderHorizontalInput(Main.soundVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 4:
			Main.ambientVolume = HandleSliderHorizontalInput(Main.ambientVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 5:
		{
			float hBar = Main.hBar;
			float num3 = (Main.hBar = HandleSliderHorizontalInput(hBar, 0f, 1f));
			if (hBar != num3)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Hue = num3;
					break;
				case 252:
					Main.mouseBorderColorSlider.Hue = num3;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 6:
		{
			float sBar = Main.sBar;
			float num2 = (Main.sBar = HandleSliderHorizontalInput(sBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX));
			if (sBar != num2)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Saturation = num2;
					break;
				case 252:
					Main.mouseBorderColorSlider.Saturation = num2;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 7:
		{
			float lBar = Main.lBar;
			float min = 0.15f;
			if (Main.menuMode == 252)
			{
				min = 0f;
			}
			Main.lBar = HandleSliderHorizontalInput(lBar, min, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX);
			float lBar2 = Main.lBar;
			if (lBar != lBar2)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Luminance = lBar2;
					break;
				case 252:
					Main.mouseBorderColorSlider.Luminance = lBar2;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 8:
		{
			float aBar = Main.aBar;
			float num4 = (Main.aBar = HandleSliderHorizontalInput(aBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX));
			if (aBar != num4)
			{
				int menuMode = Main.menuMode;
				if (menuMode == 252)
				{
					Main.mouseBorderColorSlider.Alpha = num4;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 9:
		{
			bool left = PlayerInput.Triggers.Current.Left;
			bool right = PlayerInput.Triggers.Current.Right;
			if (PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right)
			{
				SomeVarsForUILinkers.HairMoveCD = 0;
			}
			else if (SomeVarsForUILinkers.HairMoveCD > 0)
			{
				SomeVarsForUILinkers.HairMoveCD--;
			}
			if (SomeVarsForUILinkers.HairMoveCD == 0 && (left || right))
			{
				if (left)
				{
					Main.PendingPlayer.hair--;
				}
				if (right)
				{
					Main.PendingPlayer.hair++;
				}
				SomeVarsForUILinkers.HairMoveCD = 12;
			}
			int num = 51;
			if (Main.PendingPlayer.hair >= num)
			{
				Main.PendingPlayer.hair = 0;
			}
			if (Main.PendingPlayer.hair < 0)
			{
				Main.PendingPlayer.hair = num - 1;
			}
			break;
		}
		case 10:
			Main.GameZoomTarget = HandleSliderHorizontalInput(Main.GameZoomTarget, 1f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 11:
			Main.UIScale = HandleSliderHorizontalInput(Main.UIScaleWanted, 0.5f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			Main.temporaryGUIScaleSlider = Main.UIScaleWanted;
			break;
		case 12:
			Main.MapScale = HandleSliderHorizontalInput(Main.MapScale, 0.5f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.7f);
			break;
		}
	}
}
