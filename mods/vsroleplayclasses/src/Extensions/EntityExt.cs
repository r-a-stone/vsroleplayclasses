﻿using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace vsroleplayclasses.src.Extensions
{
    public static class EntityExt
    {
        public static bool IsIServerPlayer(this Entity me)
        {
            if (!(me is EntityPlayer))
                return false;

            if (((EntityPlayer)me).Player == null)
                return false;

            if (!(((EntityPlayer)me).Player is IServerPlayer))
                return false;

            return true;
        }

        public static bool ChangeCurrentHp(this Entity me, Entity sourceEntity, float amount, EnumDamageType type)
        {
            return me.ReceiveDamage(
                new DamageSource() { 
                Source = sourceEntity is EntityPlayer ? EnumDamageSource.Player : EnumDamageSource.Entity,
                SourceEntity = sourceEntity, 
                Type = type
                },
                amount
                );
        }

        public static bool BindToLocation(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return false;

            return me.GetAsIServerPlayer().BindToLocation();
        }

        public static IServerPlayer GetAsIServerPlayer(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return null;

            return ((IServerPlayer)((EntityPlayer)me).Player);
        }

        public static void SkillUp(this Entity me, Ability ability)
        {
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().SkillUp(ability);
        }

        public static bool GateToBind(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return false;

            return me.GetAsIServerPlayer().GateToBind();
        }

        public static Entity GetTarget(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return null;

            return me.GetAsIServerPlayer().GetTarget();
        }


        public static void DecreaseMana(this Entity me, float mana)
        {
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().DecreaseMana(mana);
        }

        public static void AwardExperience(this Entity me, EnumAdventuringClass experienceType, double experienceAmount)
        {
            if (experienceType == EnumAdventuringClass.None)
                return;

            // Only award to players
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().GrantExperience(experienceType, experienceAmount);
        }

        public static int GetLevel(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return me.GetAsIServerPlayer().GetLevel();

            return 1;
        }

        public static double GetExperienceWorth(this Entity killed, IServerPlayer killer)
        {
            // Only award when killing npcs
            if ((killed is EntityPlayer))
                return 0D;

            return (EntityUtils.GetExperienceRewardAverageForLevel(killed.GetLevel(), killer.GetLevel()));
        }
    }
}
