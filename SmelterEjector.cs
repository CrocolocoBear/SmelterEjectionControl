using System.Reflection;
using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace SmelterEjectionControl
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class SmelterEjectionControlPlugin : BaseUnityPlugin
    {
        const string pluginGUID = "com.crocolocobear.smelterejector";
        const string pluginName = "SmelterEjector";
        const string pluginVersion = "1.0.0";

        private readonly Harmony HarmonyInstance = new Harmony(pluginGUID);

        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(pluginName);
        
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(SmelterEjectionControlPlugin));
            Logger.LogInfo("SmelterEjectionControl: ItemDrop physics patch loaded!"); 
        }
        
        [HarmonyPatch(typeof(ItemDrop), "Awake")] 
        [HarmonyPostfix]
        static void ModifySmelterDrops(ItemDrop instance)
        {
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb == null) return;
            Collider[] hits = Physics.OverlapSphere(instance.transform.position, 2f);
            foreach (Collider hit in hits)
            {
                Smelter smelter = hit.GetComponentInParent<Smelter>();
                if (smelter != null)
                {
                    float dist = Vector3.Distance(instance.transform.position, smelter.m_outputPoint.position);
                    if (dist < 2.0f) 
                    {
                        rb.AddForce(-Vector3.up * 15f, ForceMode.Impulse);
                        break; 
                    }
                }
            }
        }
    }
}