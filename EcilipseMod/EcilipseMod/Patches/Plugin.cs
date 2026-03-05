using BepInEx;  //modumuzun BepInEx tarafından tanınmasını sağlicak
using BepInEx.Unity.IL2CPP;  //IL2CPP oyunla köprü kurucak 
using HarmonyLib;     //Oyunun kodlarına kanca atıp değiştiricez
using UnityEngine;    // Oyunun kendi konsoluna debug.log ile yazı yzabilmemiz için
using _Code.Characters;
namespace EcilipseMod.Patches
{
    //Bu bir kalıp BeInPlugin kalıbı diyor ki ben bir modum eklentiyim içine 3 parametre alır
                                       //Benzersiz ID, İsim, Versiyon
    [BepInPlugin("com.ecilipse.ninahmod","Ecilipse NINAH mod", "1.0.0")]
    //Bu kalıtım sayesinde Kendi Plugin dosyam BepInEx den gelen plugin özelliklerine sahip oldu
    public class Plugin : BasePlugin
    {
        public static BepInEx.Logging.ManualLogSource modLog; 
        //Modumuzun kendi log kaynağı, oyun loglarından ayrı olarak modumuzun loglarını tutacak
        public override void Load()
        {
            modLog = Log; //BepInEx tarafından sağlanan log kaynağını modLog değişkenine atıyoruz
            modLog.LogInfo("Ecilipse NINAH Mod BepInEx tarafından yüklendi!");
            //Modun yüklendiğine dair bir bilgi mesajı yazdırıyoruz)
            var harmony = new Harmony("com.portfolyo.ecilipsemod");
            harmony.PatchAll();

            modLog.LogInfo("Character Manager Hook u basari ile atildi!");
        }

    }
    //Sıra trigger
    [HarmonyPatch(typeof(CharactersManager),nameof(CharactersManager.LetIn))]
    public class LetIn_Patch
    {
        //Prefix orjinal LetIn kod bloğu çalışmadan hemen önce araya giricek ve kendi kodumuzu çalıştıracağız
        [HarmonyPrefix]
        //Orjinal LetIn methodu 3 parametre alıyor bizde aynı parametreleri buraya yazıyoruz ki oyun bize o bilgileri versin
        public static bool Prefix(CharactersManager __instance,CharacterSOData character,bool exileOldCharacterFromThisPlace)
        {
            Plugin.modLog.LogMessage($"[ECILIPSE MOD] LetIn tetiklendi! Karakter: {character.name}, exileOldCharacterFromThisPlace: {exileOldCharacterFromThisPlace}");
            
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(CharactersManager __instance,CharacterSOData character,bool exileOldCharacterFromThisPlace)
        {
            // character.Place ve character.Room birer ENUM olduğu için ToString() ile yazdırıyoruz
            string roomName = character.Room.ToString();
            string placeName = character.Place.ToString();
            Plugin.modLog.LogWarning($"[ECILIPSE MOD] LetIn (Basarili)! {character.name} su konuma yerlesti -> Oda: {roomName}, Nokta: {placeName}");
        }
    }
}
 