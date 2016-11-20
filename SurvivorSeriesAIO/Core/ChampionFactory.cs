﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChampionFactory.cs.cs" company="SurvivorSeriesAIO">
//      Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using SebbyLib;
using SurvivorSeriesAIO.Champions;

namespace SurvivorSeriesAIO.Core
{
    public static class ChampionFactory
    {
        public static IChampion Load(string name, IRootMenu menu)
        {
            var orbwalker = new Orbwalking.Orbwalker(menu.Orbwalking);

            switch (name)
            {
                case "Malzahar":
                    return new Malzahar(menu, orbwalker);
                case "Brand":
                    return new Brand(menu, orbwalker);
                case "Ryze":
                    return new Ryze(menu, orbwalker);
                case "Ashe":
                    return new Ashe(menu, orbwalker);
                case "Irelia":
                    return new Irelia(menu, orbwalker);

                default:
                    throw new NotSupportedException($"Champion {name} not supported.");
            }
        }
    }
}