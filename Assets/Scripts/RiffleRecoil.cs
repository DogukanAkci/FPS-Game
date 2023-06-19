using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiffleRecoil : MonoBehaviour
{
   [Header("Recoil_Transform")] 
   [SerializeField]
   Transform RecoilPositionTransform;
   [SerializeField] 
   private Transform RecoilRotationTransform;

   [Space(10)] [Header("Recoil_Setting")]
   [SerializeField] 
   private float PositionDampTime;
   [SerializeField] 
   private float RotationDampTime;

   [Space(10)] 
   [SerializeField] private float Recoil1;
   [SerializeField] private float Recoil2;
   [SerializeField] private float Recoil3;
   [SerializeField] private float Recoil4;
   [Space(10)]
   [SerializeField] Vector3 RecoilRotation;
   [SerializeField] Vector3 RecoilKickBack;
   [SerializeField] Vector3 RecoilRotation_Aim;
   [SerializeField] Vector3 RecoilKickBack_Aim;
   [Space(10)]
   [SerializeField] Vector3 CurrentRecoil1;
   [SerializeField] Vector3 CurrentRecoil2;
   [SerializeField] Vector3 CurrentRecoil3;
   [SerializeField] Vector3 CurrentRecoil4;
   [Space(10)]
   [SerializeField] Vector3 RotationOutput;

   public bool aim;

   void FixedUpdate()
   {
      CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, Recoil1 * Time.deltaTime);
      CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, CurrentRecoil1, Recoil2 * Time.deltaTime);
      CurrentRecoil3 = Vector3.Lerp(CurrentRecoil3, Vector3.zero, Recoil3 * Time.deltaTime);
      CurrentRecoil4 = Vector3.Lerp(CurrentRecoil4, CurrentRecoil3, Recoil4 * Time.deltaTime);
      
      RecoilPositionTransform.localPosition = Vector3.Slerp(RecoilPositionTransform.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
      RotationOutput = Vector3.Slerp(RotationOutput, CurrentRecoil1, RotationDampTime * Time.fixedDeltaTime);
      RecoilRotationTransform.localRotation = Quaternion.Euler(RotationOutput);
   }
   public void Fire()
   {
      if (aim)
      {
         CurrentRecoil1 += new Vector3(RecoilRotation_Aim.x, Random.Range(-RecoilRotation_Aim.y, RecoilRotation_Aim.y), Random.Range(-RecoilRotation_Aim.z, RecoilRotation_Aim.z));
         CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack_Aim.x, RecoilKickBack_Aim.x), Random.Range(-RecoilKickBack_Aim.y, RecoilKickBack_Aim.y), RecoilKickBack_Aim.z);
      }
      if (!aim)
      {
         CurrentRecoil1 += new Vector3(RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
         CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
      }
   }
}
