using System.Collections.Generic;
using UnityEngine;

namespace RedGaint
{
    public class GunHoister : MonoBehaviour
    {
        public List<Gun> gunPrefab;
        public Gun currentGun = null;
        public Gun LoadGun(GlobalEnums.GunType guntype)
        {
            foreach (var gun in gunPrefab)
            {
                if (gun.GetComponent<Gun>().gunType != guntype)
                    continue;
                currentGun = Instantiate(gun, transform);
                return currentGun;
            }
            return null;
        }
    }
}//RedGaint
