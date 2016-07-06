using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;
using Color = System.Drawing.Color;

namespace KoreanMalzahar
{
    class Program
    {
        public const string ChampionName = "Malzahar";
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        private static bool IsChanneling;
        private static Orbwalking.Orbwalker Orbwalker;
        //Menu
        public static Menu Menu;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        private static float Rtime = 0;
        public static Spell Q, W, E, R;
        private const float SpellQWidth = 400f;
        public static SpellSlot igniteSlot;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Malzahar") return;

            igniteSlot = Player.GetSpellSlot("summonerdot");
            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 750f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R, 700f);

            Q.SetSkillshot(0.75f, 80, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 80, 20, false, SkillshotType.SkillshotCircle);

            Menu = new Menu("KoreanMalzahar", "KoreanMalzahar", true);
            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            #region Combo/Harass/LaneClear/OneShot
            //Combo Menu
            var combo = new Menu("Combo", "Combo");
            Menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("Combo", "Combo"));
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("useIgniteInCombo", "Use Ignite if Killable").SetValue(true));
            //Harass Menu
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("autoharass", "Auto Harrass with E").SetValue(true));
            harass.AddItem(new MenuItem("autoharassuseQ", "Auto Harrass with Q").SetValue(false));
            harass.AddItem(new MenuItem("autoharassminimumMana", "Minimum Mana%").SetValue(new Slider(30)).SetTooltip("Minimum Mana that you need to have to AutoHarass with Q/E."));
            //LaneClear Menu
            var lc = new Menu("Laneclear", "Laneclear");
            Menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("LaneClearMinions", "LaneClear Minimum Minions for Q").SetValue(new Slider(2, 0, 10)));
            lc.AddItem(new MenuItem("laneclearEMinimumMana", "Minimum E Mana%").SetValue(new Slider(30)).SetTooltip("Minimum Mana that you need to have to cast E on LaneClear."));
            lc.AddItem(new MenuItem("laneclearQMinimumMana", "Minimum Q Mana%").SetValue(new Slider(30)).SetTooltip("Minimum Mana that you need to have to cast Q on LaneClear."));

            // Drawing Menu
            var DrawingMenu = new Menu("Drawings", "Drawings");
            Menu.AddSubMenu(DrawingMenu);
            DrawingMenu.AddItem(new MenuItem("drawQ", "Draw Q range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawW", "Draw W range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawE", "Draw E range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("drawR", "Draw R range").SetValue(true));
            // Misc Menu
            var miscMenu = new Menu("Misc", "Misc");
            Menu.AddSubMenu(miscMenu);
            // Todo: Add more KillSteal Variants/Spells
            miscMenu.AddItem(new MenuItem("ksE", "Use E to KillSteal (Currently being coded)").SetValue(true));
            miscMenu.AddItem(new MenuItem("interruptQ", "Interrupt Spells Q", true).SetValue(true));
            miscMenu.AddItem(new MenuItem("useQAntiGapCloser", "Use Q on GapClosers").SetValue(true));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
                miscMenu.AddItem(new MenuItem("gapcloserR" + enemy.ChampionName, enemy.ChampionName).SetValue(false).SetTooltip("Use R on GapClosing Champions"));
            miscMenu.AddItem(new MenuItem("OneShotInfo", "OneShot Combo [Info]").SetTooltip("If you don't have mana to cast Q/W/E/R spells all together it won't cast the spells. Use Combo Instead."));
            miscMenu.AddItem(new MenuItem("oneshot", "Burst Combo").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)).SetTooltip("It will cast Q+E+W+R on enemy when enemy is in E range."));
            Menu.AddToMainMenu();
            #endregion
            // Draw Damage
            #region DrawHPDamage
            var dmgAfterShave = new MenuItem("KoreanMalzahar.DrawComboDamage", "Draw Combo Damage").SetValue(true);
            var drawFill =
                new MenuItem("KoreanMalzahar.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, Color.FromArgb(204, 255, 0, 1)));
            Menu.SubMenu("HPBarDrawings").AddItem(drawFill);
            Menu.SubMenu("HPBarDrawings").AddItem(dmgAfterShave);
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
            DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
            dmgAfterShave.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            #endregion

            #region Subscriptions
            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
            Game.PrintChat("<font color='#800040'>[Korean]Malzahar</font> <font color='#ff6600'>Loaded.</font>");
            #endregion
        }
        private static void OnDraw(EventArgs args)
        {
            if (Menu.Item("drawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.DarkRed, 3);               
            }
            if (Menu.Item("drawW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, 450f, System.Drawing.Color.LightBlue, 3);
            }
            if (Menu.Item("drawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Purple, 3);
            }
            if (Menu.Item("drawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.LightPink, 3);
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }

            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }
            /*if (E.IsReady() && Menu.Item("ksE").GetValue<bool>())
            {
                foreach (var t in HeroManager.Enemies.Where(h => h.IsValidTarget(E.Range) && h.Health < Player.GetSpellDamage(h, SpellSlot.E)))
                {
                    E.Cast(t);
                }
            }*/
            //Combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            //Burst
            if (Menu.Item("oneshot").GetValue<KeyBind>().Active)
            {
                Oneshot();
            }
            //Lane
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
            }
            
            //AutoHarass
          if (Menu.Item("autoharass").GetValue<KeyBind>().Active)
                AutoHarass();
        }
        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }
            if (!Menu.Item("interruptQ", true).GetValue<bool>() || !Q.IsReady())
                return;

            if (t.IsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
        }

        #region Q Range/Placement Calculations (BETA)
        /*private void CastQ(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!Q.IsReady() || !(GetManaPercent() >= minManaPercent))
                return;
            if (target == null)
                return;
            Q.Width = GetDynamicQWidth(target);
            Q.Cast(target);
        }
        public static float GetManaPercent()
        {
            return (ObjectManager.Player.Mana / ObjectManager.Player.MaxMana) * 100f;
        }
        private static float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - (ObjectManager.Player.Distance(target) / Q.Range)) * SpellQWidth);
        }*/
        #endregion

        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.IsChannelingImportantSpell() || Game.Time - Rtime < 2.5 || Player.HasBuff("malzaharrsound"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }
            // Improved AntiGap Closer
            var sender = gapcloser.Sender;
            if (!gapcloser.Sender.IsValidTarget())
            {
                return;
            }

            if (Menu.Item("useQAntiGapCloser").GetValue<bool>() && sender.IsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.End);
            }
            else if (R.IsReady() && Menu.Item("gapcloserR" + gapcloser.Sender.ChampionName).GetValue<bool>() && sender.IsValidTarget(R.Range))
            {
                R.CastOnUnit(sender);
            }
        }
        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            if (igniteSlot != SpellSlot.Unknown || Player.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
            {
                if (Menu.Item("useIgniteInCombo").GetValue<bool>())
                {
                    damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
                }
            }
            double ultdamage = 0;

            if (Q.IsReady())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.IsReady())
            {
                damage += W.GetDamage(enemy);
            }

            if (E.IsReady())
            {
                damage += E.GetDamage(enemy);
            }

            if (R.IsReady())
            {
                ultdamage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }
            return damage + ((float)ultdamage * 2);
        }
        private static void AutoHarass()
        {
            if (Player.ManaPercentage() < Menu.Item("autoharassminimumMana").GetValue<Slider>().Value)
                return;
            var m = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("autoharass").GetValue<bool>())
                    E.CastOnUnit(m);
            if (Menu.Item("autoharassuseQ").GetValue<bool>())
                    Q.Cast(m);
        }
        private static bool HasRBuff()
        {
            return (Player.IsChannelingImportantSpell() || Player.HasBuff("AiZaharNetherGrasp") || Player.HasBuff("MalzaharR") || Player.HasBuff("MalzaharRSound") || R.IsChanneling);
        }
        //Combo
        private static void Combo()
        {
            var useQ = (Menu.Item("useQ").GetValue<bool>());
            var useW = (Menu.Item("useW").GetValue<bool>());
            var useE = (Menu.Item("useE").GetValue<bool>());
            var useR = (Menu.Item("useR").GetValue<bool>());
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!m.IsValidTarget())
            {
                return;
            }
            if (Player.Mana > E.ManaCost + W.ManaCost + R.ManaCost)
            {
                if (useE && E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useQ && Q.IsReady() && Player.Mana > Q.ManaCost && Q.IsInRange(m)) Q.Cast(m);
                if (useW && W.IsReady()) W.Cast(m);
                if (useR && R.IsReady() && m != null && R.IsInRange(m)) R.CastOnUnit(m);
            }
            else
            {
                if (useE && E.IsReady() && E.IsInRange(m)) E.CastOnUnit(m);
                if (useQ && Q.IsReady() && Player.Mana > Q.ManaCost && Q.IsInRange(m)) Q.Cast(m);
                if (useW && W.IsReady() && Player.Mana > W.ManaCost) W.Cast(m);
            }
            if (Menu.Item("useIgniteInCombo").GetValue<bool>())
            {
                if (m.Health < Player.GetSummonerSpellDamage(m, Damage.SummonerSpell.Ignite))
                {
                    Player.Spellbook.CastSpell(igniteSlot, m);
                }
            }
        }
        //Burst
        public static void Oneshot()
        {
            // If player doesn't have mana don't execute the OneShot Combo
            if (Player.Mana < Q.ManaCost + W.ManaCost + E.ManaCost + R.ManaCost)
                return;

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (Q.IsReady()) Q.CastOnUnit(m);
                if (E.IsReady()) E.CastOnUnit(m);
                if (W.IsReady()) W.CastOnUnit(m);
                Player.Spellbook.CastSpell(igniteSlot, m);
                if (R.IsReady() && !E.IsReady() && !W.IsReady()) R.CastOnUnit(m);
        }
        //Lane
        private static void Lane()
        {
            if (Player.ManaPercentage() < Menu.Item("laneclearEMinimumMana").GetValue<Slider>().Value || Player.ManaPercentage() < Menu.Item("laneclearQMinimumMana").GetValue<Slider>().Value)
                return;

            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
            if (Menu.Item("laneclearE").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
            if (Menu.Item("laneclearQ").GetValue<bool>() && Q.IsReady())
            {
                var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
                var farmPos = Q.GetCircularFarmLocation(allMinions, 150);
                if (farmPos.MinionsHit > Menu.Item("LaneClearMinions").GetValue<Slider>().Value)
                    Q.Cast(farmPos.Position);
            }
        }
    }
}
