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
    // Her �rk i�in ba�lang�� temel stat de�erleri
    public int baseStrength = 8;
    public int baseIntelligence = 8;
    public int basePiety = 8;
    public int baseVitality = 8;
    public int baseDexterity = 8;
    public int baseSpeed = 8;
    public int baseSenses = 8;

    [Header("Special Racial Abilities")] // YEN� EKLEND�
    public List<SpecialAbilitySO> racialAbilities;

    [System.Serializable] // Inspector'da g�r�nebilmesi ve serile�tirilebilmesi i�in
    public struct ResistanceModifier {
        public MagicRealm realm;
        public int value; // Pozitif ise diren�, negatif ise zay�fl�k. Y�zde veya sabit puan olabilir.
                          // �imdilik sabit puan olarak d���nelim (�rn: +20 Diren�).
    }

    [Header("Base Magic Resistances")] // YEN� EKLEND�
    public List<ResistanceModifier> baseResistances;
    // Gelecekte eklenebilir:
    // public List<SkillSO> innateRacialSkills;
    // public List<ResistanceModifier> racialResistances;
}
