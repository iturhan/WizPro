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
    Magic    // Belki gelecekte b�y� okullar� i�in (Wizardry, Divinity vb.)
             // �imdilik Academic alt�nda tutuluyorlar, ama ayr�labilir.
}

public enum ConditionSeverity // Hastal�klar�n/Durumlar�n ciddiyetini belirtmek i�in
{
    Minor,      // Dinlenmeyle ge�er
    Moderate,   // Basit b�y�ler veya iksirler gerektirir
    Major,      // G��l� b�y�ler veya �zel e�yalar gerektirir
    Critical,   // �l�mc�l veya kal�c� etkiler
    Special     // "OK" gibi �zel durumlar
}

public enum MagicRealm {
    None,       // Genel diren� veya diren� yok
    Fire,
    Water,
    Air,
    Earth,
    Mental,
    Divine,
    Arcane,     // Genel B�y�c�l�k direnci (Wizardry skilliyle ilgili)
    Psionic,    // Psionic skilliyle ilgili
    Alchemic    // Alchemy skilliyle ilgili
    // Di�er �zel b�y� t�rleri eklenebilir
}

public enum SpellSchoolType // B�y� Okullar�/Kitaplar�
{
    None,
    Priest,
    Mage,
    Psionic,
    Alchemist
}

public enum SpellTargetingType // B�y�n�n Hedefleme �ekli
{
    NoTarget,           // Hedef gerektirmez (�rn: Light)
    Self,
    SingleAlly,
    SingleEnemy,
    PartyAllies,        // T�m parti �yeleri
    GroupEnemies,       // Belirli bir d��man grubu
    AreaOfEffectEnemy,  // Belirli bir alandaki d��manlar (�rn: Fireball)
    AreaOfEffectAlly,   // Belirli bir alandaki dostlar
    ConeEnemy,          // Koni �eklinde d��manlar
    AllEnemies,         // T�m d��manlar
    LockOrTrap,         // Kilit veya tuzak
    Spot3D,             // 3D bir nokta (�rn: Summon Elemental)
    Item                // Bir e�ya �zerine (�rn: Enchant Item - gelecekte)
}



