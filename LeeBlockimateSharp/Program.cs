// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeeBlockimateSharp">
//      Copyright (c) LeeBlockimateSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
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
            if (!WorkingChampions.Contains(Player.ChampionName.ToLower()))
                return;
            #region Spells

            if (Player.ChampionName.ToLower() == "talon")
                _talonR = new Spell(SpellSlot.R);
            if (Player.ChampionName.ToLower() == "vayne")
            {
                _vayneQ = new Spell(SpellSlot.Q);
                _vayneR = new Spell(SpellSlot.R);
            }
            if ((Player.ChampionName.ToLower() == "wukong") || (Player.ChampionName.ToLower() == "monkeyking"))
                _wukongW = new Spell(SpellSlot.W);
            if (Player.ChampionName.ToLower() == "khazix")
                _khaZixR = new Spell(SpellSlot.R);
            if (Player.ChampionName.ToLower() == "shaco")
                _shacoQ = new Spell(SpellSlot.Q);

            #endregion

            #region Menu

            _menu = new Menu("LeeBlockimate#", "LeeBlockimateSharp", true).SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            var spells = _menu.AddSubMenu(new Menu("Spells", "Spells").SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            if (Player.ChampionName.ToLower() == "talon")
                spells.AddItem(
                    new MenuItem("BlockWithTalonR", "Exploit with R").SetValue(true));
            if (Player.ChampionName.ToLower() == "vayne")
                spells.AddItem(new MenuItem("BlockWithVayneQR", "Exploit with Vayne Q/R").SetValue(true));
            if ((Player.ChampionName.ToLower() == "wukong") || (Player.ChampionName.ToLower() == "monkeyking"))
                spells.AddItem(new MenuItem("BlockWithWukongW", "Exploit with Wukong W").SetValue(true));
            if (Player.ChampionName.ToLower() == "khazix")
                spells.AddItem(new MenuItem("BlockWithKhaZixR", "Exploit with KhaZix R").SetValue(true));
            if (Player.ChampionName.ToLower() == "shaco")
                spells.AddItem(new MenuItem("BlockWithShacoQ", "Exploit with Shaco Q").SetValue(true));

            _menu.AddToMainMenu();

            #endregion

            #region Subscriptions

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpell;

            #endregion

            Game.PrintChat(
                "<font color='#800040'>[Exploit] LeeBlockimateSharp</font> <font color='#ff6600'>Loaded.</font>");
            Game.PrintChat("<font color='#7A6EFF'>[LeeBlockimateSharp] Will dodge (Lee Sin/Darius/Garen/Camille)'s Ult </font>");
            Notifications.AddNotification(new Notification(Player.ChampionName+" Loaded!", 3500).SetTextColor(System.Drawing.Color.Chartreuse));
        }

        public static void Obj_AI_Base_OnProcessSpell(Obj_AI_Base enemy, GameObjectProcessSpellCastEventArgs spell)
        {
            if (enemy.IsMe)
                return;
            if (enemy.IsChampion() && enemy.IsEnemy)
                if ((BlockSpells.Contains(spell.SData.Name) || BlockSpells.Contains(spell.SData.DisplayName)) &&
                    (spell.Target == Player))
                    switch (Player.ChampionName.ToLower())
                    {
                        case "talon":
                            if (_menu.Item("BlockiWithTalonR").GetValue<bool>())
                            {
                                _talonR.Cast();
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
                        case "vayne":
                            if (_menu.Item("BlockWithVayneQR").GetValue<bool>())
                            {
                                _vayneR.Cast();
                                _vayneQ.Cast(Game.CursorPos);
                                if (!Player.HasBuffOfType(BuffType.Silence))
                                    Notifications.AddNotification(
                                        new Notification("Successfully blocked with (R+Q)", 3000).SetTextColor(
                                            System.Drawing.Color.Chartreuse));
                                else
                                    Notifications.AddNotification(
                                        new Notification("Silenced couldn't block with (R+Q)", 3000).SetTextColor(
                                            System.Drawing.Color.Red));
                            }
                            break;
                        case "wukong":
                            if (_menu.Item("BlockWithWukongW").GetValue<bool>())
                            {
                                _wukongW.Cast();
                                if (!Player.HasBuffOfType(BuffType.Silence))
                                    Notifications.AddNotification(
                                        new Notification("Successfully blocked with (W)", 3000).SetTextColor(
                                            System.Drawing.Color.Chartreuse));
                                else
                                    Notifications.AddNotification(
                                        new Notification("Silenced couldn't block with (W)", 3000).SetTextColor(
                                            System.Drawing.Color.Red));
                            }
                            break;
                        case "monkeyking":
                            if (_menu.Item("BlockWithWukongW").GetValue<bool>())
                            {
                                _wukongW.Cast();
                                if (!Player.HasBuffOfType(BuffType.Silence))
                                    Notifications.AddNotification(
                                        new Notification("Successfully blocked with (W)", 3000).SetTextColor(
                                            System.Drawing.Color.Chartreuse));
                                else
                                    Notifications.AddNotification(
                                        new Notification("Silenced couldn't block with (W)", 3000).SetTextColor(
                                            System.Drawing.Color.Red));
                            }
                            break;
                        case "khazix":
                            if (_menu.Item("BlockWithKhaZixR").GetValue<bool>())
                            {
                                _khaZixR.Cast();
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
                            if (_menu.Item("BlockWithShacoQ").GetValue<bool>())
                            {
                                _shacoQ.Cast(Game.CursorPos);
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
                    }
        }

        #region Declaration

        private static Spell _talonR, _vayneQ, _vayneR, _wukongW, _khaZixR, _shacoQ;
        private static Menu _menu;
        private static Obj_AI_Hero Player => ObjectManager.Player;
        private static readonly string[] BlockSpells = {"BlindMonkRKick", "GarenR", "DariusExecute", "CamilleR"};
        private static readonly string[] WorkingChampions = {"talon", "khazix", "leesin", "wukong", "monkeyking", "vayne", "shaco"};

        #endregion
    }
}