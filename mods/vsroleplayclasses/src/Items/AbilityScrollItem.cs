﻿using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace vsroleplayclasses.src.Items
{
    public class AbilityScrollItem : Item
    {
        public override void OnLoaded(ICoreAPI api)
        {
            if (api.Side != EnumAppSide.Client)
                return;

            ICoreClientAPI capi = api as ICoreClientAPI;
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            if (IsAbilityScribed(inSlot.Itemstack))
            {
                dsc.AppendLine(Lang.Get("Ability: {0}", GetScribedAbilityName(inSlot.Itemstack)));
                return;
            }
            else
            {
                if (HasSpareRuneSlot(inSlot.Itemstack))
                {
                    dsc.AppendLine(Lang.Get("Ability: {0}", "This scroll is incomplete"));
                }
                else
                {
                    dsc.AppendLine(Lang.Get("Ability: {0}", "This scroll appears useless"));
                    return;
                }
            }


            var spellEffectIndex = GetWordOfPower<SpellEffectIndex>(inSlot.Itemstack);
            var spellEffectType = GetWordOfPower<SpellEffectType>(inSlot.Itemstack);
            var targetType = GetWordOfPower<TargetType>(inSlot.Itemstack);
            var resistType = GetWordOfPower<ResistType>(inSlot.Itemstack);
            var powerLevel = GetWordOfPower<PowerLevel>(inSlot.Itemstack);
            var adventureClass = GetWordOfPower<AdventureClass>(inSlot.Itemstack);

            if (spellEffectIndex != SpellEffectIndex.None)
                dsc.AppendLine(Lang.Get("Spell Effect Index: {0}", spellEffectIndex));
            if (spellEffectType != SpellEffectType.None)
                dsc.AppendLine(Lang.Get("Spell Effect Type: {0}", spellEffectType));
            if (targetType != TargetType.None)
                dsc.AppendLine(Lang.Get("Target Type: {0}", targetType));
            if (resistType != ResistType.None)
                dsc.AppendLine(Lang.Get("Resist Type: {0}", resistType));
            if (powerLevel != PowerLevel.None)
                dsc.AppendLine(Lang.Get("Power Level: {0}", powerLevel));
            if (adventureClass != AdventureClass.None)
                dsc.AppendLine(Lang.Get("Adventure Class: {0}", adventureClass));
        }

        public bool IsAbilityScribed(ItemStack itemStack)
        {
            return (GetScribedAbilityId(itemStack) > 0);
        }

        // Seed so client and server can match
        public void SetScribedAbility(ItemStack itemStack, Ability ability)
        {
            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetLong("scribedAbility", ability.Id);
                itemStack.Attributes.SetString("scribedAbilityName", ability.Name);
                if (!itemStack.Attributes.HasAttribute("scribedAbility"))
                    throw new Exception("This should not happen");
                if (!itemStack.Attributes.HasAttribute("scribedAbilityName"))
                    throw new Exception("This should not happen");
            }
        }

        public void SetWordOfPower<T>(ItemStack itemStack, T magicPower) where T : Enum
        {
            MagicPowerSlot magicPowerSlot = (MagicPowerSlot)Enum.Parse(typeof(MagicPowerSlot), typeof(T).Name);

            if (magicPowerSlot == MagicPowerSlot.None)
                throw new Exception("Invalid slot");

            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetInt("wordOfPower_"+ magicPowerSlot, (int)(object)magicPower);
                if (!itemStack.Attributes.HasAttribute("wordOfPower_" + magicPowerSlot))
                    throw new Exception("This should not happen");
            }
        }

        internal T GetWordOfPower<T>(ItemStack itemStack) where T : Enum
        {
            MagicPowerSlot magicPowerSlot = (MagicPowerSlot)Enum.Parse(typeof(MagicPowerSlot), typeof(T).Name);

            if (magicPowerSlot == MagicPowerSlot.None)
                throw new Exception("Invalid slot");

            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("wordOfPower_"+ magicPowerSlot))
                        return default(T);

                    var wordOfPower = itemStack.Attributes.TryGetInt("wordOfPower_" + magicPowerSlot);
                    if (wordOfPower == null)
                        return default(T);


                    return (T)Enum.ToObject(typeof(T), wordOfPower);
                }
                catch (InvalidCastException)
                {

                    return default(T);
                }
            }
            return default(T);
        }
        
        internal long GetScribedAbilityId(ItemStack itemStack)
        {
            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("scribedAbility"))
                        return -1;

                    return itemStack.Attributes.GetLong("scribedAbility", -1); // when deserialized json item it will default to long over int
                }
                catch (InvalidCastException)
                {

                    return -1;
                }
            }
            return -1;
        }

        internal string GetScribedAbilityName(ItemStack itemStack)
        {
            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("scribedAbilityName"))
                        return "Unknown Ability";

                    return itemStack.Attributes.GetString("scribedAbilityName", "Unknown Ability"); 
                }
                catch (InvalidCastException)
                {

                    return "Unknown Ability";
                }
            }
            return "Unknown Ability";
        }

        public bool HasSpareRuneSlot(ItemStack itemstack)
        {
            if (!HasRunePower<SpellEffectIndex>(itemstack))
                return true;
            if (!HasRunePower<SpellEffectType>(itemstack))
                return true;
            if (!HasRunePower<TargetType>(itemstack))
                return true;
            if (!HasRunePower<ResistType>(itemstack))
                return true;
            if (!HasRunePower<PowerLevel>(itemstack))
                return true;
            if (!HasRunePower<AdventureClass>(itemstack))
                return true;

            return false;
        }


        internal bool HasRunePower<T>(ItemStack itemstack) where T : Enum
        {
            return !Object.Equals(GetWordOfPower<T>(itemstack), default(T));
        }
    }
}