using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.WindowsMR.Input;

namespace SA
{
    public static class StatsCalculations
    {
        public static int CalculateBaseDamage(WeaponStats w, CharacterStats st, float multiplier = 1)
        {
            float physical = (w.a_physical * multiplier) - st.physical;
            float strike = (w.a_strike * multiplier) - st.vs_strike;
            float slash = (w.a_slash * multiplier) -  st.vs_slash;
            float thrust = (w.a_thrust * multiplier) -  st.vs_thrust;

            float sum = physical + strike + slash + thrust;

            float magic = (w.a_magic * multiplier) - st.magic;
            float fire = (w.a_fire * multiplier) - st.fire;
            float lightning = (w.a_lightning * multiplier) - st.lightning;
            float dark = (w.a_dark * multiplier) - st.dark;

            sum += magic + fire + lightning + dark;

            if (sum <= 0)
                sum = 1;

            return Mathf.RoundToInt(sum);

        }

    }
}
