// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeeBlockimateSharp">
//      Copyright (c) LeeBlockimateSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using LeagueSharp;
using LeagueSharp.Common;

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

            #endregion

            #region Menu

            Menu = new Menu("LeeBlockimate#", "LeeBlockimateSharp", true);

            var Spells = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            if (Player.ChampionName.ToLower() == "talon")
                Spells.AddItem(
                    new MenuItem("BlockWithTalonR", "Exploit with R").SetValue(true)
                        .SetTooltip("Will dodge Lee Sin's Ultimate with R (Exploit)"));

            Menu.AddToMainMenu();

            #endregion

            #region Subscriptions

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpell;

            #endregion

            Game.PrintChat(
                "<font color='#800040'>[Exploit] LeeBlockimateSharp</font> <font color='#ff6600'>Loaded.</font>");
        }

        public static void Obj_AI_Base_OnProcessSpell(Obj_AI_Base enemy, GameObjectProcessSpellCastEventArgs Spell)
        {
            if (enemy.IsMe)
                return;
            if (enemy.IsChampion() && enemy.IsEnemy)
                if ((Spell.SData.Name == "BlindMonkRKick") ||
                    ((Spell.SData.DisplayName == "BlindMonkRKick") && (Spell.Target == Player)))
                    TalonR.Cast();
        }

        #region Declaration

        private static Spell TalonR;
        private static Menu Menu;
        private static Obj_AI_Hero Player => ObjectManager.Player;

        #endregion
    }
}