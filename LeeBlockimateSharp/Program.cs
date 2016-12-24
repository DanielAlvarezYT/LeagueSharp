// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeeBlockimateSharp">
//      Copyright (c) LeeBlockimateSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

namespace LeeBlockimateSharp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            #region Spells

            if (Player.ChampionName.ToLower() == "talon")
                TalonR = new Spell(SpellSlot.R);
            if (Player.ChampionName.ToLower() == "vayne")
            {
                VayneQ = new Spell(SpellSlot.Q);
                VayneR = new Spell(SpellSlot.R);
            }
            if ((Player.ChampionName.ToLower() == "wukong") || (Player.ChampionName.ToLower() == "monkeyking"))
                WukongW = new Spell(SpellSlot.W);
            if (Player.ChampionName.ToLower() == "khazix")
                KhaZixR = new Spell(SpellSlot.R);
            if (Player.ChampionName.ToLower() == "shaco")
                ShacoQ = new Spell(SpellSlot.Q);

            #endregion

            #region Menu

            Menu = new Menu("LeeBlockimate#", "LeeBlockimateSharp", true).SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            var Spells = Menu.AddSubMenu(new Menu("Spells", "Spells").SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            if (Player.ChampionName.ToLower() == "talon")
                Spells.AddItem(
                    new MenuItem("BlockWithTalonR", "Exploit with R").SetValue(true));
            if (Player.ChampionName.ToLower() == "vayne")
                Spells.AddItem(new MenuItem("BlockWithVayneQR", "Exploit with Vayne Q/R").SetValue(true));
            if ((Player.ChampionName.ToLower() == "wukong") || (Player.ChampionName.ToLower() == "monkeyking"))
                Spells.AddItem(new MenuItem("BlockWithWukongW", "Exploit with Wukong W").SetValue(true));
            if (Player.ChampionName.ToLower() == "khazix")
                Spells.AddItem(new MenuItem("BlockWithKhaZixR", "Exploit with KhaZix R").SetValue(true));
            if (Player.ChampionName.ToLower() == "shaco")
                Spells.AddItem(new MenuItem("BlockWithShacoQ", "Exploit with Shaco Q").SetValue(true));

            Menu.AddToMainMenu();

            #endregion

            #region Subscriptions

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpell;

            #endregion

            Game.PrintChat(
                "<font color='#800040'>[Exploit] LeeBlockimateSharp</font> <font color='#ff6600'>Loaded.</font>");
            Game.PrintChat("<font color='#CC0099'>[LeeBlockimateSharp] Will dodge (Lee Sin/Darius/Garen)'s Ult </font>");
        }

        public static void Obj_AI_Base_OnProcessSpell(Obj_AI_Base enemy, GameObjectProcessSpellCastEventArgs Spell)
        {
            if (enemy.IsMe)
                return;
            if (enemy.IsChampion() && enemy.IsEnemy)
                if ((Spell.SData.Name == "BlindMonkRKick") || (Spell.SData.DisplayName == "BlindMonkRKick") ||
                    (Spell.SData.Name == "GarenR") || (Spell.SData.DisplayName == "GarenR") ||
                    (Spell.SData.Name == "DariusExecute") ||
                    ((Spell.SData.DisplayName == "DariusExecute") && (Spell.Target == Player)))
                    switch (Player.ChampionName.ToLower())
                    {
                        case "talon":
                            if (Menu.Item("BlockiWithTalonR").GetValue<bool>())
                                TalonR.Cast();
                            break;
                        case "vayne":
                            if (Menu.Item("BlockWithVayneQR").GetValue<bool>())
                            {
                                VayneR.Cast();
                                VayneQ.Cast(Game.CursorPos);
                            }
                            break;
                        case "wukong":
                            if (Menu.Item("BlockWithWukongW").GetValue<bool>())
                                WukongW.Cast();
                            break;
                        case "monkeyking":
                            if (Menu.Item("BlockWithWukongW").GetValue<bool>())
                                WukongW.Cast();
                            break;
                        case "khazix":
                            if (Menu.Item("BlockWithKhaZixR").GetValue<bool>())
                            {
                                KhaZixR.Cast();
                                if (!Player.HasBuffOfType(BuffType.Silence))
                                    Notifications.AddNotification(
                                        new Notification("Successfully blocked with (R)", 3000).SetTextColor(
                                            System.Drawing.Color.Chartreuse));
                                else
                                    Notifications.AddNotification(
                                        new Notification("Silenced couldn't block with (R)", 3000).SetTextColor(
                                            System.Drawing.Color.Red));
                            }
                            break;
                        case "shaco":
                            if (Menu.Item("BlockWithShacoQ").GetValue<bool>())
                                ShacoQ.Cast(Game.CursorPos);
                            break;
                    }
        }

        #region Declaration

        private static Spell TalonR, VayneQ, VayneR, WukongW, KhaZixR, ShacoQ;
        private static Menu Menu;
        private static Obj_AI_Hero Player => ObjectManager.Player;

        #endregion
    }
}