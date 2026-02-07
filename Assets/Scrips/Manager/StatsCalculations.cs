using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.WindowsMR.Input;

namespace SA
{
    public static class StatsCalculations
    {
        public static int CalculateBaseDamage(WeaponStats w, CharacterStats st)
        {
            int physical = w.physical - st.physical;
            int strike = w.strike - st.vs_strike;
            int slash = w.slash - st.vs_slash;
            int thrust = w.thrust - st.vs_thrust;

            int sum = physical + strike + slash + thrust;

            int magic = w.magic - st.magic;
            int fire =  w.fire - st.fire;
            int lightning = w.lightning - st.lightning;
            int dark = w.dark - st.dark;

            sum += magic + fire + lightning + dark;

            if(sum <= 0)
               sum = 1;

            return sum;

        }

    }
}
