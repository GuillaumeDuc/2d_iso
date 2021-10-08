using UnityEngine;

public static class MixSpell
{
    public static GameObject mix(Spell firstSpell, Spell secondSpell)
    {
        // Mix Fire with Fire
        if (firstSpell.type == SpellType.Fire && secondSpell.type == SpellType.Fire)
        {
            return SpellList.getMixedSpell(SpellType.Fire);
        }
        // Mix Fire with Earth
        if (firstSpell.type == SpellType.Fire && secondSpell.type == SpellType.Earth || firstSpell.type == SpellType.Earth && secondSpell.type == SpellType.Fire)
        {
            return SpellList.getMixedSpell(SpellType.Magma);
        }
        // Mix Fire with Wind
        if (firstSpell.type == SpellType.Fire && secondSpell.type == SpellType.Wind || firstSpell.type == SpellType.Wind && secondSpell.type == SpellType.Fire)
        {
            return SpellList.getMixedSpell(SpellType.Electricity);
        }
        // Mix Water with Water
        if (firstSpell.type == SpellType.Water && secondSpell.type == SpellType.Water)
        {
            return SpellList.getMixedSpell(SpellType.Water);
        }
        // Mix Water with Wind
        if (firstSpell.type == SpellType.Water && secondSpell.type == SpellType.Wind || firstSpell.type == SpellType.Wind && secondSpell.type == SpellType.Water)
        {
            return SpellList.getMixedSpell(SpellType.Ice);
        }
        // Mix Wind with Wind
        if (firstSpell.type == SpellType.Wind && secondSpell.type == SpellType.Wind)
        {
            return SpellList.getMixedSpell(SpellType.Wind);
        }
        // None
        return null;
    }
}
