using BepInEx;
using System;
using UnityEngine;
using Utilla;
using System.Collections;
using System.Reflection;
using UnityEngine.XR;

namespace GorillaSabers
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private static XRNode rNode = XRNode.RightHand;
        private static bool BButtonPress;
        GameObject Saber;
        GameObject Blade;
        GameObject PlayerHand;
        bool BladeEnabled;
        AudioSource SaberOn;
        AudioSource SaberOff;
        AudioSource SaberIdle;
        GameObject Blade1;
        Material Blade2;
        float r = PlayerPrefs.GetFloat("redValue");
        float g = PlayerPrefs.GetFloat("greenValue");
        float b = PlayerPrefs.GetFloat("blueValue");
        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            var str = Assembly.GetExecutingAssembly().GetManifestResourceStream("GorillaSabers.Resources.saber");
            if (str == null)
                return;

            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            if (bundle == null)
                return;

            var asset = bundle.LoadAsset<GameObject>("Saber");
            asset = Instantiate(asset);
            Saber = asset;
            Blade = Saber.transform.Find("Blade").gameObject;
            SaberOn = Saber.transform.Find("On").GetComponent<AudioSource>();
            SaberOff = Saber.transform.Find("Off").GetComponent<AudioSource>();
            SaberIdle = Saber.transform.Find("SaberIdle").GetComponent<AudioSource>();
            PlayerHand = GameObject.Find("Global/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/");
            Saber.transform.SetParent(PlayerHand.transform);
            Saber.transform.localPosition = new Vector3(0.05f, 0.02f, -0.13f);
            Saber.transform.localRotation = Quaternion.Euler(70, 0, 0);
            Saber.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            Blade.transform.localScale = new Vector3(1, 0, 1);
        }

        public static IEnumerator Vibration(float amplitude, float duration)
        {
            float startTime = Time.time;
            uint channel = 0U;
            InputDevice device;
            device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            while (Time.time < startTime + duration)
            {
                device.SendHapticImpulse(channel, amplitude, duration);
                yield return new WaitForSeconds(duration * 0.9f);
            }
            yield break;
        }

        void OnEnable()
        {
            Saber.SetActive(true);
        }

        void OnDisable()
        {
            Saber.SetActive(false);
        }
        void Update()
        {
            InputDevice rightController = InputDevices.GetDeviceAtXRNode(rNode);

            rightController.TryGetFeatureValue(CommonUsages.primaryButton, out BButtonPress);

            if (BButtonPress)
            {
                
                if (BladeEnabled)
                {
                    Blade.transform.localScale = new Vector3(1, 0, 1);
                    Invoke("BladesDisabled", 0.3f);
                    SaberOff.Play();
                    SaberIdle.Stop();
                    StartCoroutine(Vibration(1f, 2f));
                }
                else
                {
                    Blade.transform.localScale = new Vector3(1, 1, 1);
                    Invoke("BladesEnabled", 0.3f);
                    SaberOn.Play();
                    StartCoroutine(Vibration(1f, 2f));

                }
            }
            
        }
        void BladesEnabled()
        {

            BladeEnabled = true;
            SaberIdle.Play();
        }
        void BladesDisabled()
        {
            BladeEnabled = false;
        }
        

    }
}
