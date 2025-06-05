// _Project/Scripts/Data/Enums/CharacterEnums.cs

public enum Sex {
    Male,
    Female
}

public enum StatType {
    Strength,
    Intelligence,
    Piety,
    Vitality,
    Dexterity,
    Speed,
    Senses
}

public enum SkillCategory {
    Weaponry,
    Physical,
    Academia,
    Expert, // Yeni eklendi
    Magic    // Belki gelecekte büyü okullarý için (Wizardry, Divinity vb.)
             // Þimdilik Academic altýnda tutuluyorlar, ama ayrýlabilir.
}

public enum ConditionSeverity // Hastalýklarýn/Durumlarýn ciddiyetini belirtmek için
{
    Minor,      // Dinlenmeyle geçer
    Moderate,   // Basit büyüler veya iksirler gerektirir
    Major,      // Güçlü büyüler veya özel eþyalar gerektirir
    Critical,   // Ölümcül veya kalýcý etkiler
    Special     // "OK" gibi özel durumlar
}

public enum MagicRealm {
    None,       // Genel direnç veya direnç yok
    Fire,
    Water,
    Air,
    Earth,
    Mental,
    Divine,
    Arcane,     // Genel Büyücülük direnci (Wizardry skilliyle ilgili)
    Psionic,    // Psionic skilliyle ilgili
    Alchemic    // Alchemy skilliyle ilgili
    // Diðer özel büyü türleri eklenebilir
}

public enum SpellSchoolType // Büyü Okullarý/Kitaplarý
{
    None,
    Priest,
    Mage,
    Psionic,
    Alchemist
}

public enum SpellTargetingType // Büyünün Hedefleme Þekli
{
    NoTarget,           // Hedef gerektirmez (örn: Light)
    Self,
    SingleAlly,
    SingleEnemy,
    PartyAllies,        // Tüm parti üyeleri
    GroupEnemies,       // Belirli bir düþman grubu
    AreaOfEffectEnemy,  // Belirli bir alandaki düþmanlar (örn: Fireball)
    AreaOfEffectAlly,   // Belirli bir alandaki dostlar
    ConeEnemy,          // Koni þeklinde düþmanlar
    AllEnemies,         // Tüm düþmanlar
    LockOrTrap,         // Kilit veya tuzak
    Spot3D,             // 3D bir nokta (örn: Summon Elemental)
    Item                // Bir eþya üzerine (örn: Enchant Item - gelecekte)
}



