// _Project/Scripts/Data/ScriptableObjects/RaceSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRace", menuName = "WizardryPro/Character/Race")]
public class RaceSO : ScriptableObject {
    [Header("Basic Info")]
    public string raceName;
    [TextArea(3, 5)]
    public string description;
    public Sprite defaultPortrait;

    [Header("Base Stats")]
    // Her ýrk için baþlangýç temel stat deðerleri
    public int baseStrength = 8;
    public int baseIntelligence = 8;
    public int basePiety = 8;
    public int baseVitality = 8;
    public int baseDexterity = 8;
    public int baseSpeed = 8;
    public int baseSenses = 8;

    [Header("Special Racial Abilities")] // YENÝ EKLENDÝ
    public List<SpecialAbilitySO> racialAbilities;

    [System.Serializable] // Inspector'da görünebilmesi ve serileþtirilebilmesi için
    public struct ResistanceModifier {
        public MagicRealm realm;
        public int value; // Pozitif ise direnç, negatif ise zayýflýk. Yüzde veya sabit puan olabilir.
                          // Þimdilik sabit puan olarak düþünelim (örn: +20 Direnç).
    }

    [Header("Base Magic Resistances")] // YENÝ EKLENDÝ
    public List<ResistanceModifier> baseResistances;
    // Gelecekte eklenebilir:
    // public List<SkillSO> innateRacialSkills;
    // public List<ResistanceModifier> racialResistances;
}
