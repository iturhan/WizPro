// _Project/Scripts/Data/ScriptableObjects/SpellSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpell", menuName = "WizardryPro/Magic/Spell")]
public class SpellSO : ScriptableObject {
    [Header("Basic Info")]
    public string spellName;
    [TextArea(3, 10)]
    public string description;
    public Sprite icon;
    [Range(1, 7)]
    public int spellBookLevel = 1; // Büyünün öðrenilebileceði minimum seviye (kitap seviyesi)

    [Header("School & Realm")]
    // Bu büyü hangi okullarýn/kitaplarýn parçasý?
    public List<SpellSchoolType> availableInSpellbooks;
    public MagicRealm magicRealm; // Büyünün ait olduðu ana büyü alaný

    [Header("Casting Mechanics")]
    public SpellTargetingType targetingType = SpellTargetingType.NoTarget;
    public int baseSpellPointCost = 1;
    [Range(1, 8)] // Genellikle 7, bazý büyüler için 8
    public int maxPowerLevel = 7;
    // Her güç seviyesi için SP maliyeti. Eðer boþsa, baseSpellPointCost * powerLevel kullanýlýr.
    // Örn: [3, 5, 7, 10, 14, 19, 25] -> Güç seviyesi 1 için 3 SP, GL 7 için 25 SP.
    public List<int> spellPointCostByPowerLevel;

    [Header("Effects & Duration (Basit Metin Gösterimi - Faz 2'de Detaylanacak)")]
    // Örnek: "10/L", "4/L", "N/A"
    public string effectPotencyFormulaText = "N/A";
    // Örnek: "Instant", "1/L", "24 Hours", "5 Minutes/L"
    public string durationFormulaText = "Instant";

    [Header("Requirements & Flags")]
    public bool canCastInCombat = true;
    public bool canCastOutOfCombat = true;
    // Gelecekte: Gerekli eþyalar (örn: Alchemist için powders), Minimum Mastery Skill vb.

    // FAZ 2'DE EKLENECEK DAHA DETAYLI ETKÝ SÝSTEMÝ:
    // public List<SpellEffectEntry> spellEffects;
}

// Ýleride kullanýlacak yapý (Faz 2)
// [System.Serializable]
// public class SpellEffectEntry
// {
//     public SpellEffectType effectType; // Damage, Heal, ApplyStatus, ModifyStat etc.
//     public float baseValue;
//     public float perCasterLevelValue;
//     public float perPowerLevelMultiplier; // Güç seviyesine göre etki çarpaný
//     public StatType relatedStat; // Etkisi hangi stattan etkileniyor
//     public StatusEffectSO statusToApply;
//     public string durationFormula; // Bu etkiye özel süre
//     public SpellTargetingType specificTargetingOverride; // Eðer bu efektin hedeflemesi farklýysa
// }

/*
spellPointCostByPowerLevel: Eðer bu liste boþ býrakýlýrsa, basit bir hesaplama (örn: baseSpellPointCost * seçilenGüçSeviyesi) kullanýlabilir. Ama Wizardry genellikle her seviye için özel maliyetler belirler.
effectPotencyFormulaText ve durationFormulaText: Þimdilik büyülerin etkilerini ve sürelerini metin olarak tutacaðýz. Bir sonraki fazda bunlarý daha yapýsal bir "Spell Effect System" ile deðiþtireceðiz.
*/
