using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MixSpell
{
    public static GameObject mix(Spell firstSpell, Spell secondSpell)
    {
        // Mix Fire with Fire
        if (firstSpell.type == SpellType.Fire && secondSpell.type == SpellType.Fire)
        {
            return SpellList.getMixedSpell(SpellType.SuperFire);
        }
        // Mix Fire with Earth
        if (firstSpell.type == SpellType.Fire && secondSpell.type == SpellType.Earth || firstSpell.type == SpellType.Earth && secondSpell.type == SpellType.Fire)
        {
            return SpellList.getMixedSpell(SpellType.Magma);
        }
        // Mix Water with Water
        if (firstSpell.type == SpellType.Water && secondSpell.type == SpellType.Water)
        {
            return SpellList.getMixedSpell(SpellType.SuperWater);
        }
        // Mix Water with Wind
        if (firstSpell.type == SpellType.Water && secondSpell.type == SpellType.Wind || firstSpell.type == SpellType.Wind && secondSpell.type == SpellType.Water)
        {
            return SpellList.getMixedSpell(SpellType.Ice);
        }
        // None
        return null;
    }
}
