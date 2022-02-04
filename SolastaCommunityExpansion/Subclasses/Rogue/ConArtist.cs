﻿using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Subclasses.Rogue
{
    internal class ConArtist : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("fdf8dc11-5006-489e-951c-92a8d72ca4c0");
        private readonly CharacterSubclassDefinition Subclass;

        #region DcIncreaseAffinity
        private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;
        private static FeatureDefinitionMagicAffinity DcIncreaseAffinity => _dcIncreaseAffinity ??= new FeatureDefinitionMagicAffinityBuilder(
                        "MagicAffinityRoguishConArtistDC",
                        GuidHelper.Create(SubclassNamespace, "MagicAffinityRoguishConArtistDC").ToString(),
                        GetSpellDCPresentation().Build())
                            .SetCastingModifiers(0, Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc, false, false, false).AddToDB();
        #endregion

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal ConArtist()
        {
            // Make Con Artist subclass
            CharacterSubclassDefinitionBuilder conArtist = new CharacterSubclassDefinitionBuilder("RoguishConArtist", GuidHelper.Create(SubclassNamespace, "RoguishConArtist").ToString());
            GuiPresentationBuilder conPresentation = new GuiPresentationBuilder(
                "Subclass/&RoguishConArtistTitle",
                "Subclass/&RoguishConArtistDescription");
            conPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainInsight.GuiPresentation.SpriteReference);
            conArtist.SetGuiPresentation(conPresentation.Build());

            GuiPresentationBuilder conAffinity = new GuiPresentationBuilder(
                "Subclass/&AbilityAffinityRogueConArtistTitle",
                "Subclass/&AbilityAffinityRogueConArtistDescription");
            FeatureDefinitionAbilityCheckAffinity abilityAffinity = BuildAbilityAffinity(
                new List<(string, string)> {
                    (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Performance)
                },
                RuleDefinitions.CharacterAbilityCheckAffinity.Advantage, "AbilityAffinityRogueConArtist", conAffinity.Build());
            conArtist.AddFeatureAtLevel(abilityAffinity, 3);

            FeatureDefinitionCastSpellBuilder spellCasting = new FeatureDefinitionCastSpellBuilder("CastSpellConArtist", GuidHelper.Create(SubclassNamespace, "CastSpellConArtist").ToString());
            spellCasting.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass);
            spellCasting.SetSpellCastingAbility(AttributeDefinitions.Charisma);
            spellCasting.SetSpellList(DatabaseHelper.SpellListDefinitions.SpellListWizard);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolConjuration);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEnchantment);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolIllusion);
            spellCasting.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection);
            spellCasting.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
            spellCasting.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            spellCasting.SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);
            spellCasting.SetKnownSpells(4, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);
            spellCasting.SetSlotsPerLevel(3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);
            GuiPresentationBuilder spellcastGui = new GuiPresentationBuilder(
                "Subclass/&RoguishConArtistSpellcastingTitle",
                "Subclass/&RoguishConArtistSpellcastingDescription");
            spellCasting.SetGuiPresentation(spellcastGui.Build());
            conArtist.AddFeatureAtLevel(spellCasting.AddToDB(), 3);

            GuiPresentationBuilder feintGui = new GuiPresentationBuilder(
                "Subclass/&RoguishConArtistFeintTitle",
                "Subclass/&RoguishConArtistFeintDescription");
            EffectDescriptionBuilder feintBuilder = new EffectDescriptionBuilder();
            feintBuilder.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Distance, 12, RuleDefinitions.TargetType.Individuals, 1, 0, ActionDefinitions.ItemSelectionType.None);
            feintBuilder.SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            feintBuilder.SetSavingThrowData(true, false, AttributeDefinitions.Wisdom, true, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Charisma,
                15, false, new List<SaveAffinityBySenseDescription>());
            GuiPresentationBuilder feintGuiCondition = new GuiPresentationBuilder(
                "Subclass/&RoguishConArtistFeintConditionTitle",
                "Subclass/&RoguishConArtistFeintConditionDescription");
            feintGuiCondition.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionTrueStrike.GuiPresentation.SpriteReference);
            feintBuilder.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetConditionForm(new AdvantageBuilder("RogueConArtistFeintCondition",
                GuidHelper.Create(SubclassNamespace, "RogueConArtistFeintCondition").ToString(),
                DatabaseHelper.ConditionDefinitions.ConditionTrueStrike, feintGuiCondition.Build()).AddToDB(), ConditionForm.ConditionOperation.Add,
                false, false, new List<ConditionDefinition>()).Build());
            //feintBuilder.AddEffectForm(new EffectFormBuilder().SetConditionForm(DatabaseHelper.ConditionDefinitions.))
            FeatureDefinitionPower feint = new FeatureDefinitionPowerBuilder("RoguishConArtistFeint", GuidHelper.Create(SubclassNamespace, "RoguishConArtistFeint").ToString(),
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Charisma, RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
                false, false, AttributeDefinitions.Charisma, feintBuilder.Build(), feintGui.Build(), false /* unique instance */).AddToDB();
            conArtist.AddFeatureAtLevel(feint, 9);
            conArtist.AddFeatureAtLevel(DcIncreaseAffinity, 13);

            FeatureDefinitionProficiency proficiency = new FeatureDefinitionProficiencyBuilder("RoguishConArtistMentalSavingThrows", GuidHelper.Create(SubclassNamespace, "RoguishConArtistMentalSavingThrows").ToString(),
                RuleDefinitions.ProficiencyType.SavingThrow,
                new List<string>() { AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom },
                new GuiPresentationBuilder(
                    "Subclass/&RoguishConArtistMentalSavingThrowsTitle",
                    "Subclass/&RoguishConArtistMentalSavingThrowsDescription").Build()).AddToDB();
            conArtist.AddFeatureAtLevel(proficiency, 17);

            // add subclass to db and add subclass to rogue class
            Subclass = conArtist.AddToDB();
        }

        public class AdvantageBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            public AdvantageBuilder(string name, string guid, ConditionDefinition original, GuiPresentation guiPresentation) : base(original, name, guid)
            {
                Definition.SetGuiPresentation(guiPresentation);
                Definition.SetField("specialInterruptions", new List<RuleDefinitions.ConditionInterruption>() {
                    RuleDefinitions.ConditionInterruption.Attacked,
                });
                Definition.SetAdditionalDamageWhenHit(true);
                Definition.SetAdditionalDamageDieType(RuleDefinitions.DieType.D8);
                Definition.SetAdditionalDamageDieNumber(3);
                Definition.SetAdditionalDamageQuantity(ConditionDefinition.DamageQuantity.Dice);
            }
        }

        private static GuiPresentationBuilder GetSpellDCPresentation()
        {
            return new GuiPresentationBuilder("Subclass/&MagicAffinityRoguishConArtistDCTitle", "Subclass/&MagicAffinityRoguishConArtistDC" + Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc + "Description");
        }

        public static FeatureDefinitionAbilityCheckAffinity BuildAbilityAffinity(IEnumerable<(string abilityScoreName, string proficiencyName)> abilityProficiencyPairs,
            RuleDefinitions.CharacterAbilityCheckAffinity affinityType, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(name,SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetAbilityAffinities(abilityProficiencyPairs, 0, RuleDefinitions.DieType.D8, affinityType)
                .AddToDB();
        }

        public static void UpdateSpellDCBoost()
        {
            if (DcIncreaseAffinity)
            {
                DcIncreaseAffinity.SetSaveDCModifier(Main.Settings.OverrideRogueConArtistImprovedManipulationSpellDc);
                DcIncreaseAffinity.SetGuiPresentation(GetSpellDCPresentation().Build());
            }
        }
    }
}
