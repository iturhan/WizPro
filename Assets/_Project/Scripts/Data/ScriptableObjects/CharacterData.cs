// _Project/Scripts/Data/CharacterData.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // LINQ kullanmak için (örn: Any, FirstOrDefault)

[System.Serializable] // Unity'nin serileþtirebilmesi ve Inspector'da görebilmesi için
public class CharacterData {
    #region Fields

    [Header("Identity")]
    public string characterName = "New Hero";
    public Sprite portrait;
    public Sex sex = Sex.Male;
    public RaceSO race;
    public ClassSO characterClass;

    [Header("Core Stats (Base Values - Before any modifiers except sex)")]
    private Dictionary<StatType, int> baseStats = new Dictionary<StatType, int>();

    [Header("Combat & Resources")]
    public int currentHP;
    public int maxHP;
    public int currentStamina;
    public int maxStamina;
    public int baseAC = 10; // AC'nin temel baþlangýç deðeri (Wizardry'de düþük AC daha iyi)

    [Header("Progression & Status")]
    public int experiencePoints = 0;
    public int level = 1;
    public int rank = 0; // RNK
    public int monsterKillStats = 0; // MKS
    public StatusEffectSO currentOverallStatus; // "OK" gibi genel durum için bir referans
    public int unallocatedSkillPoints = 0; // Daðýtýlmamýþ skill puanlarý

    [Header("Economy")]
    public int goldPieces = 0;

    [Header("Skills")]
    public Dictionary<SkillSO, int> learnedSkills = new Dictionary<SkillSO, int>();

    [Header("Unlocked Expert Skills")]
    public List<SkillSO> unlockedExpertSkills = new List<SkillSO>();

    [Header("Active Status Effects / Illnesses")]
    public List<ActiveStatusEffect> activeStatusEffects = new List<ActiveStatusEffect>();

    [Header("Spellcasting Data")]
    public List<SpellSO> knownSpells = new List<SpellSO>();
    public Dictionary<MagicRealm, int> currentSpellPoints = new Dictionary<MagicRealm, int>();
    public Dictionary<MagicRealm, int> maxSpellPoints = new Dictionary<MagicRealm, int>();

    public Dictionary<SkillSO, int> skills = new Dictionary<SkillSO, int>();

    #endregion

    #region Constructors

    public CharacterData(string name, Sex sex, RaceSO race, ClassSO charClass) {
        this.characterName = name;
        this.sex = sex;
        this.race = race;
        this.characterClass = charClass;
        this.level = 1;

        InitializeBaseStats();
        InitializeStartingSkills();
        InitializeSpellPoints(); // Büyü puanlarýný baþlat

        // HP, Stamina baþlangýçta hesaplanmalý Statlar ve Class belirlendikten sonra
        CalculateMaxHP();
        currentHP = maxHP;
        CalculateMaxStamina();
        currentStamina = maxStamina;

        this.race = race; // Önce ata
        this.characterClass = charClass; // Önce ata
        InvalidateSpecialAbilitiesCache(); // Cache'i sýfýrla ki GetCompiled ilk çaðrýda yeniden oluþtursun

        InitializeBaseStats();
        InitializeSkills();
        InitializeStartingSkills();
        // ... (diðer baþlatmalar) ...
        ApplyInitialSpecialAbilityEffects(); // Özel yeteneklerin baþlangýç etkilerini uygula

        if (charClass != null) {
            unallocatedSkillPoints = charClass.skillPointsToAllocatePerLevel > 0 ? charClass.skillPointsToAllocatePerLevel : 0;
        }
    }

    public CharacterData() { } // Serileþtirme için boþ constructor

    #endregion

    #region Initialization Methods

    private void InitializeBaseStats() {
        if (race == null) {
            Debug.LogError($"{characterName}: RaceSO is not assigned. Cannot initialize base stats. Assigning default 8 to all.");
            foreach (StatType stat in System.Enum.GetValues(typeof(StatType))) {
                baseStats[stat] = 8;
            }
            return;
        }

        baseStats.Clear();
        baseStats[StatType.Strength] = race.baseStrength;
        baseStats[StatType.Intelligence] = race.baseIntelligence;
        baseStats[StatType.Piety] = race.basePiety;
        baseStats[StatType.Vitality] = race.baseVitality;
        baseStats[StatType.Dexterity] = race.baseDexterity;
        baseStats[StatType.Speed] = race.baseSpeed;
        baseStats[StatType.Senses] = race.baseSenses;

        if (sex == Sex.Female) {
            baseStats[StatType.Strength] = Mathf.Max(0, baseStats[StatType.Strength] - 1);
            baseStats[StatType.Senses] += 1;
        }
        EnforceMaxStatRule();
    }

    private void InitializeSkills() {
        skills.Clear();
        // Doldurma iþini Controller yapacak.
    }

    private void InitializeStartingSkills() {
        learnedSkills.Clear();
        if (characterClass == null || characterClass.startingSkillPoints == null) return;

        foreach (var allocation in characterClass.startingSkillPoints) {
            if (allocation.skill != null && characterClass.allowedSkills.Contains(allocation.skill)) {
                learnedSkills[allocation.skill] = Mathf.Clamp(allocation.points, 0, allocation.skill.maxSkillPoints);
            }
            else if (allocation.skill != null) {
                Debug.LogWarning($"{characterName}: Class '{characterClass.className}' trying to initialize starting skill points for '{allocation.skill.skillName}', but this skill is not in its allowedSkills list or the skill is null.");
            }
        }
    }

    private void InitializeSpellPoints() {
        currentSpellPoints.Clear();
        maxSpellPoints.Clear();
        foreach (MagicRealm realm in System.Enum.GetValues(typeof(MagicRealm))) {
            if (realm != MagicRealm.None) {
                currentSpellPoints[realm] = 0;
                maxSpellPoints[realm] = 0;
            }
        }
        CalculateAllMaxSpellPoints();
        RestoreAllSpellPoints();
    }

    #endregion

    #region Stat Methods

    public void SetBaseStat(StatType stat, int value) {
        baseStats[stat] = Mathf.Clamp(value, 0, 18);
        // Stat deðiþtikçe HP, SP gibi baðlý deðerler güncellenebilir.
        // CalculateMaxHP(); CalculateMaxStamina(); CalculateAllMaxSpellPoints();
    }

    public int GetBaseStat(StatType stat) {
        return baseStats.ContainsKey(stat) ? baseStats[stat] : 0;
    }

    private void EnforceMaxStatRule() {
        List<StatType> keys = new List<StatType>(baseStats.Keys);
        foreach (StatType key in keys) {
            baseStats[key] = Mathf.Clamp(baseStats[key], 0, 18);
        }
    }

    public int GetEffectiveStat(StatType statType) {
        int effectiveValue = GetBaseStat(statType);
        // TODO: Ekipman ve aktif durum efektlerinden (buff/debuff) gelen modifikasyonlarý ekle
        return Mathf.Clamp(effectiveValue, 0, 99); // Etkili statlar için farklý bir max deðeri
    }

    #endregion

    #region Combat Calculation Methods

    public int GetArmorClass() {
        int calculatedAC = baseAC;
        int dexValue = GetEffectiveStat(StatType.Dexterity);
        if (dexValue > 10) calculatedAC -= (dexValue - 10) / 2;
        else if (dexValue < 10) calculatedAC += (10 - dexValue) / 2;

        // TODO: Ekipman (zýrh, kalkan), doðal AC ve diðer modifikatörleri ekle

        foreach (var effect in activeStatusEffects) {
            if (effect.SourceEffect != null && effect.SourceEffect.acOverride != -1) {
                calculatedAC = Mathf.Max(calculatedAC, effect.SourceEffect.acOverride);
            }
            // TODO: Add evasionACModifier from StatusEffectSO if present
        }
        return Mathf.Clamp(calculatedAC, -20, 20);
    }

    public void CalculateMaxHP() {
        if (characterClass == null || race == null) { maxHP = 10; currentHP = Mathf.Clamp(currentHP, 0, maxHP); return; }
        int vitValue = GetEffectiveStat(StatType.Vitality);
        maxHP = (vitValue * 1) + (level * characterClass.hpPerLevelBonus) + (level * 2) + 5;
        if (maxHP <= 0) maxHP = 1;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if (currentHP <= 0 && !HasStatusEffect("Dead")) {/* ApplyStatusEffectByName("Dead"); */}
    }

    public void CalculateMaxStamina() {
        if (characterClass == null || race == null) { maxStamina = 10; currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); return; }
        int strValue = GetEffectiveStat(StatType.Strength);
        int vitValue = GetEffectiveStat(StatType.Vitality);
        maxStamina = (strValue / 2) + (vitValue / 2) + (level * characterClass.stmPerLevelBonus) + (level * 1) + 5;
        if (maxStamina <= 0) maxStamina = 1;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    #endregion

    #region Resistance Methods
    public int GetEffectiveResistance(MagicRealm realm) {
        if (realm == MagicRealm.None) return 0;
        int totalResistance = 0;

        if (race != null && race.baseResistances != null) {
            foreach (var resModifier in race.baseResistances) {
                if (resModifier.realm == realm) { totalResistance += resModifier.value; break; }
            }
        }
        // TODO: Sýnýf, Ekipman, Aktif Durum Efektleri ve Yeteneklerden gelen dirençleri ekle
        if (activeStatusEffects != null) {
            foreach (var activeEffect in activeStatusEffects) {
                if (activeEffect.SourceEffect != null && activeEffect.SourceEffect.resistanceModifiers != null) {
                    foreach (var resModifier in activeEffect.SourceEffect.resistanceModifiers) {
                        if (resModifier.realm == realm) totalResistance += resModifier.value;
                    }
                }
            }
        }
        return Mathf.Clamp(totalResistance, -100, 200);
    }
    #endregion

    #region Skill Methods

    public bool CanLearnOrHasLearnedSkill(SkillSO skill) {
        if (skill == null || characterClass == null) return false;
        if (skill.isExpertSkill) {
            return characterClass.allowedSkills.Contains(skill) && unlockedExpertSkills.Contains(skill);
        }
        return characterClass.allowedSkills.Contains(skill);
    }

    public bool CanAllocatePointsToSkill(SkillSO skill) {
        if (skill == null || characterClass == null || !characterClass.allowedSkills.Contains(skill)) return false;
        if (skill.isExpertSkill && !unlockedExpertSkills.Contains(skill)) return false;
        return true;
    }

    public bool AddSkillPoints(SkillSO skill, int pointsToAdd) {
        if (skill == null || pointsToAdd <= 0 || !CanAllocatePointsToSkill(skill)) return false;
        int currentPoints = learnedSkills.ContainsKey(skill) ? learnedSkills[skill] : 0;
        int newPoints = Mathf.Clamp(currentPoints + pointsToAdd, 0, skill.maxSkillPoints);
        if (newPoints > currentPoints) { learnedSkills[skill] = newPoints; return true; }
        return false;
    }

    public int GetSkillPoints(SkillSO skill) {
        return learnedSkills.ContainsKey(skill) ? learnedSkills[skill] : 0;
    }

    public void UnlockExpertSkill(SkillSO expertSkill) {
        if (expertSkill == null || !expertSkill.isExpertSkill) return;
        if (characterClass != null && characterClass.allowedSkills.Contains(expertSkill) && !unlockedExpertSkills.Contains(expertSkill)) {
            unlockedExpertSkills.Add(expertSkill);
            Debug.Log($"Expert skill '{expertSkill.skillName}' unlocked for {characterName}!");
        }
    }

    public void AwardSkillPoints(int amount) {
        if (amount > 0) unallocatedSkillPoints += amount;
    }

    public int GetUnallocatedSkillPoints() {
        return unallocatedSkillPoints;
    }

    public bool SpendSkillPoints(int amount) {
        if (amount > 0 && unallocatedSkillPoints >= amount) { unallocatedSkillPoints -= amount; return true; }
        if (amount > 0) { Debug.LogWarning($"Trying to spend more skill points ({amount}) than available ({unallocatedSkillPoints})."); }
        return false;
    }

    #endregion

    #region Spell Point Management

    public void CalculateAllMaxSpellPoints() {
        if (characterClass == null) return;
        foreach (MagicRealm realm in System.Enum.GetValues(typeof(MagicRealm))) {
            if (realm == MagicRealm.None) continue;
            int calculatedMaxSP = 0;
            SkillSO realmSkill = GetSkillSOForRealm(realm);
            if (realmSkill != null && learnedSkills.ContainsKey(realmSkill)) {
                calculatedMaxSP += (GetSkillPoints(realmSkill) / 5) + level; // Örnek
            }
            foreach (SpellSchoolType school in characterClass.learnableSpellSchools) {
                SkillSO masterySkill = GetSkillSOForMastery(school);
                if (masterySkill != null && learnedSkills.ContainsKey(masterySkill) && IsRealmAssociatedWithSchool(realm, school)) {
                    calculatedMaxSP += (GetSkillPoints(masterySkill) / 10); // Örnek
                }
            }
            // TODO: Stat etkisi (örn: Intelligence veya Piety)
            maxSpellPoints[realm] = Mathf.Max(0, calculatedMaxSP);
        }
    }

    private SkillSO GetSkillSOForRealm(MagicRealm realm) {
        if (characterClass == null) return null;
        return characterClass.allowedSkills.FirstOrDefault(s => s.magicSkillType == MagicSkillType.Realm && s.associatedMagicRealm == realm);
    }

    private SkillSO GetSkillSOForMastery(SpellSchoolType school) {
        if (characterClass == null) return null;
        return characterClass.allowedSkills.FirstOrDefault(s => s.magicSkillType == MagicSkillType.Mastery && s.associatedMasterySchool == school);
    }

    private bool IsRealmAssociatedWithSchool(MagicRealm realm, SpellSchoolType school) {
        // Bu mantýk oyun kurallarýnýza göre detaylandýrýlmalý
        if (school == SpellSchoolType.Mage && (realm == MagicRealm.Fire || realm == MagicRealm.Water || realm == MagicRealm.Air || realm == MagicRealm.Earth || realm == MagicRealm.Arcane)) return true;
        if (school == SpellSchoolType.Priest && realm == MagicRealm.Divine) return true;
        if (school == SpellSchoolType.Psionic && realm == MagicRealm.Mental) return true;
        if (school == SpellSchoolType.Alchemist && (realm == MagicRealm.Fire || realm == MagicRealm.Water || realm == MagicRealm.Air || realm == MagicRealm.Earth || realm == MagicRealm.Alchemic)) return true;
        return false;
    }

    public void RestoreSpellPoints(MagicRealm realm, int amount) {
        if (currentSpellPoints.ContainsKey(realm) && maxSpellPoints.ContainsKey(realm)) {
            currentSpellPoints[realm] = Mathf.Clamp(currentSpellPoints[realm] + amount, 0, maxSpellPoints[realm]);
        }
    }

    public void RestoreAllSpellPoints() {
        foreach (MagicRealm realm in System.Enum.GetValues(typeof(MagicRealm))) {
            if (realm != MagicRealm.None && currentSpellPoints.ContainsKey(realm) && maxSpellPoints.ContainsKey(realm)) {
                currentSpellPoints[realm] = maxSpellPoints[realm];
            }
        }
    }

    public bool ConsumeSpellPoints(MagicRealm realm, int amount) {
        if (currentSpellPoints.ContainsKey(realm) && currentSpellPoints[realm] >= amount) {
            currentSpellPoints[realm] -= amount;
            return true;
        }
        return false;
    }
    public int GetCurrentSpellPoints(MagicRealm realm) {
        return currentSpellPoints.ContainsKey(realm) ? currentSpellPoints[realm] : 0;
    }
    public int GetMaxSpellPoints(MagicRealm realm) {
        return maxSpellPoints.ContainsKey(realm) ? maxSpellPoints[realm] : 0;
    }

    #endregion

    #region Spell Learning & Casting

    public bool CanLearnSpell(SpellSO spell) {
        if (spell == null || characterClass == null || knownSpells.Contains(spell)) return false;
        bool canAccessSchool = spell.availableInSpellbooks.Any(school => characterClass.learnableSpellSchools.Contains(school));
        if (!canAccessSchool) return false;
        if (level < GetRequiredCharacterLevelForSpellBookLevel(spell.spellBookLevel)) return false;

        bool masterySkillSufficient = false;
        foreach (SpellSchoolType schoolTypeInSpell in spell.availableInSpellbooks) {
            if (characterClass.learnableSpellSchools.Contains(schoolTypeInSpell)) {
                SkillSO masterySkill = GetSkillSOForMastery(schoolTypeInSpell);
                if (masterySkill != null) {
                    int requiredMasteryPoints = spell.spellBookLevel * 10 + 5; // Örnek
                    if (GetSkillPoints(masterySkill) >= requiredMasteryPoints) {
                        masterySkillSufficient = true;
                        break;
                    }
                }
                else { masterySkillSufficient = true; break; } // Okul için mastery skilli tanýmlý deðilse izin ver
            }
        }
        return masterySkillSufficient;
    }

    private int GetRequiredCharacterLevelForSpellBookLevel(int spellBookLevel) {
        switch (spellBookLevel) {
            case 1: return 1;
            case 2: return 3;
            case 3: return 5;
            case 4: return 8;
            case 5: return 11;
            case 6: return 14;
            case 7: return 17;
            default: return 99;
        }
    }

    public bool LearnSpell(SpellSO spell) {
        if (CanLearnSpell(spell)) {
            knownSpells.Add(spell);
            Debug.Log($"{characterName} learned spell: {spell.spellName}");
            return true;
        }
        // Debug.LogWarning($"{characterName} cannot learn spell: {spell.spellName}. Requirements not met.");
        return false;
    }

    // TODO: CanCastSpell(SpellSO spell, int powerLevel) metodu eklenecek (SP, Silence durumu vb. kontrolü)
    // TODO: CastSpell(SpellSO spell, CharacterData target, int powerLevel) metodu eklenecek

    #endregion

    #region Status Effect Methods

    public void ApplyStatusEffect(StatusEffectSO effectSO, int durationInTurns = -1) {
        if (effectSO == null) return;
        ActiveStatusEffect existingEffect = activeStatusEffects.FirstOrDefault(e => e.SourceEffect.effectName == effectSO.effectName);
        if (existingEffect != null) {
            existingEffect.remainingDuration = (durationInTurns == -1 || existingEffect.remainingDuration == -1) ? -1 : Mathf.Max(existingEffect.remainingDuration, durationInTurns);
            // Debug.Log($"Status effect '{effectSO.effectName}' duration refreshed for {characterName}.");
        }
        else {
            activeStatusEffects.Add(new ActiveStatusEffect(effectSO, durationInTurns));
            // Debug.Log($"Status effect '{effectSO.effectName}' applied to {characterName}.");
            if (effectSO.effectName == "Dead") {
                SetBaseStat(StatType.Vitality, Mathf.Max(0, GetBaseStat(StatType.Vitality) - 1));
                currentHP = 0;
            }
        }
        // TODO: Stat / AC / Dirençleri yeniden hesapla
    }

    public void RemoveStatusEffect(string effectName) {
        int removedCount = activeStatusEffects.RemoveAll(effect => effect.SourceEffect.effectName == effectName);
        // if (removedCount > 0) Debug.Log($"Status effect '{effectName}' removed from {characterName}.");
        // TODO: Stat / AC / Dirençleri yeniden hesapla
    }

    public bool HasStatusEffect(string effectName) {
        return activeStatusEffects.Any(effect => effect.SourceEffect != null && effect.SourceEffect.effectName == effectName);
    }

    public ActiveStatusEffect GetActiveStatusEffect(string effectName) {
        return activeStatusEffects.FirstOrDefault(effect => effect.SourceEffect != null && effect.SourceEffect.effectName == effectName);
    }

    public void UpdateStatusEffectDurations() {
        List<ActiveStatusEffect> effectsToRemove = new List<ActiveStatusEffect>();
        foreach (var effect in activeStatusEffects) {
            if (effect.remainingDuration > 0) {
                effect.remainingDuration--;
                if (effect.remainingDuration == 0) effectsToRemove.Add(effect);
            }
        }
        foreach (var effectToRemove in effectsToRemove) {
            activeStatusEffects.Remove(effectToRemove);
            // Debug.Log($"Status effect '{effectToRemove.SourceEffect.effectName}' expired for {characterName}.");
        }
        // if (effectsToRemove.Count > 0) { /* TODO: Yeniden hesapla */ }
    }

    #endregion

    private List<SpecialAbilitySO> _compiledAbilitiesCache = null; // Performans için cache

    #region Special Ability Methods // YENÝ BÖLÜM

    /// <summary>
    /// Karakterin ýrkýndan ve sýnýfýndan gelen tüm özel yeteneklerin birleþik listesini döndürür.
    /// Performans için sonuç cache'lenir. Irk veya Sýnýf deðiþirse cache sýfýrlanmalýdýr.
    /// </summary>
    public List<SpecialAbilitySO> GetCompiledSpecialAbilities() {
        if (_compiledAbilitiesCache == null) // Eðer cache boþsa veya geçersizse yeniden oluþtur
        {
            _compiledAbilitiesCache = new List<SpecialAbilitySO>();
            if (race != null && race.racialAbilities != null) {
                _compiledAbilitiesCache.AddRange(race.racialAbilities);
            }
            if (characterClass != null && characterClass.classAbilities != null) {
                _compiledAbilitiesCache.AddRange(characterClass.classAbilities);
            }
            // Potansiyel olarak ayný ID'ye sahip yetenekleri birleþtirebilir veya sadece benzersizleri alabiliriz.
            // Þimdilik hepsini alalým, çünkü ayný ID'li ama farklý kaynaklý yetenekler olabilir (nadir).
            // Veya: _compiledAbilitiesCache = _compiledAbilitiesCache.Distinct().ToList(); // Eðer sadece benzersiz ID'ler isteniyorsa
        }
        return _compiledAbilitiesCache;
    }

    /// <summary>
    /// Karakterin belirli bir özel yeteneðe sahip olup olmadýðýný kontrol eder.
    /// </summary>
    public bool HasSpecialAbility(SpecialAbilityType abilityType) {
        if (abilityType == SpecialAbilityType.None) return false;
        return GetCompiledSpecialAbilities().Any(ability => ability != null && ability.uniqueID == abilityType);
    }

    /// <summary>
    /// Karakterin sahip olduðu belirli bir özel yeteneðin SO'sunu döndürür (varsa).
    /// </summary>
    public SpecialAbilitySO GetSpecialAbility(SpecialAbilityType abilityType) {
        if (abilityType == SpecialAbilityType.None) return null;
        return GetCompiledSpecialAbilities().FirstOrDefault(ability => ability != null && ability.uniqueID == abilityType);
    }

    // Irk veya sýnýf deðiþtiðinde çaðrýlacak metot
    public void InvalidateSpecialAbilitiesCache() {
        _compiledAbilitiesCache = null;
    }

    /// <summary>
    /// Karakter oluþturulduðunda veya yüklendiðinde özel yeteneklerin anlýk etkilerini uygular
    /// (örn: kalýcý skill bonuslarý, AC bonuslarý).
    /// </summary>
    private void ApplyInitialSpecialAbilityEffects() {
        foreach (SpecialAbilitySO ability in GetCompiledSpecialAbilities()) {
            if (ability == null) continue;

            switch (ability.parameterType) {
                case SpecialAbilityParameterType.SkillBonus:
                    if (ability.skillToBonus != null && CanLearnOrHasLearnedSkill(ability.skillToBonus)) // Sýnýf gerçekten bu skille eriþebiliyor mu?
                    {
                        // Bu, yeteneðe kalýcý bir bonus ekler.
                        // Mevcut yetenek puanlarýný artýrmak yerine, yeteneðin "etkili" deðerini hesaplarken bu bonusu eklemek daha iyi olabilir.
                        // Veya karakterin "bonusSkillPoints" gibi bir Dictionary'sinde tutulabilir: Dictionary<SkillSO, int> bonusFromAbilities
                        Debug.Log($"{characterName} gains skill bonus for {ability.skillToBonus.skillName} from {ability.abilityName}: +{ability.skillBonusAmount}");
                        // Basitçe ekleyelim þimdilik, ama bu kalýcý bir artýþ yapar:
                        // AddSkillPoints(ability.skillToBonus, ability.skillBonusAmount); // Bu yaklaþým "unallocated" puanlarý etkilemez.
                        // Daha iyi bir yöntem: Bu bonusu `GetEffectiveSkillPoints` gibi bir yerde hesaba katmak.
                    }
                    break;
                case SpecialAbilityParameterType.ResistanceBonus:
                    // Bu, GetEffectiveResistance içinde zaten dolaylý olarak ele alýnacak
                    // eðer SpecialAbilitySO'nun resistanceModifiers listesi varsa ve bu listeye göre
                    // GetEffectiveResistance güncellenirse. Ya da burada direk bir stat'a eklenebilir.
                    // Örn: Mage_BonusToAllResistances için (bu durum için SO'da bonusToAllRealms = true olmalý)
                    if (ability.uniqueID == SpecialAbilityType.Mage_BonusToAllResistances) {
                        Debug.Log($"{characterName} gains +{ability.resistanceBonusValue}% to all resistances from {ability.abilityName}.");
                        // Bu etki GetEffectiveResistance içinde uygulanmalý.
                    }
                    break;
                // Diðer anlýk uygulanacak yetenekler (örn: Faerie AC bonusu)
                case SpecialAbilityParameterType.SpecificMechanic:
                    if (ability.uniqueID == SpecialAbilityType.Faerie_BonusToBaseArmorClass) {
                        // baseAC doðrudan deðiþtirilebilir veya GetArmorClass içinde bu bonus hesaba katýlabilir.
                        // baseAC += ability.acBonusValue; // acBonusValue SpecialAbilitySO'da tanýmlanmalý
                        Debug.Log($"{characterName} gains AC bonus from {ability.abilityName}.");
                    }
                    break;
            }
        }
        // Baþlangýç etkileri uygulandýktan sonra HP/AC vb. yeniden hesaplanabilir.
        // CalculateMaxHP(); GetArmorClass();
    }


    #endregion

    public void UpdateCharacterTick() {
        if (HasSpecialAbility(SpecialAbilityType.Fighter_StaminaRegeneration)) {
            SpecialAbilitySO regenAbility = GetSpecialAbility(SpecialAbilityType.Fighter_StaminaRegeneration);
            currentStamina = Mathf.Min(maxStamina, currentStamina + (int)regenAbility.regenerationRatePerTurnOrSecond);
        }
        // ... diðer regenler ...
        UpdateStatusEffectDurations(); // Durum efektlerini de güncelle
    }

    public void RecalculateBaseStats() {
        // Zaten var olan ve doðru çalýþan özel (private) metodunuzu çaðýrýr.
        InitializeBaseStats();
    }

}

// Karakter üzerindeki aktif bir durumu/hastalýðý temsil eden yardýmcý sýnýf
[System.Serializable]
public class ActiveStatusEffect {
    public StatusEffectSO SourceEffect; // Serileþtirme için public yapýldý
    public int remainingDuration;

    // Unity'nin serileþtiricisi için parametresiz constructor gerekebilir
    public ActiveStatusEffect() { }

    public ActiveStatusEffect(StatusEffectSO source, int duration) {
        this.SourceEffect = source;
        this.remainingDuration = duration;
    }
}
