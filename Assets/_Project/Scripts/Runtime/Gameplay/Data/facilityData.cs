using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "facilityData", menuName = "Scriptable Objects/facilityData")]
public class facilityData : ScriptableObject
{
   public List<FacilityInfo> facilityDatas;
   
   public FacilityInfo GetFacilityInfo(string facilityName)
   {
       return facilityDatas.Find(x => x.facilityName == facilityName);
   }
}
