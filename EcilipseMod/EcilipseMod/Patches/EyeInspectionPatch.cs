using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using UnityEngine;
using _Code.DialogSystem;   // Oyunun arayüz kodları
using _Code.Characters;
using EcilipseMod.Patches;     // Oyunun karakter kodları

namespace EcilipseMod 
{
    [HarmonyPatch(typeof(DialogSignsView), nameof(DialogSignsView.ShowSign))]
    public class EyeInspectionPatch
    {
        // Resimleri her seferinde diskten okumamak için Cache sözlüğü
        private static Dictionary<string, Sprite> eyeSpriteCache = new Dictionary<string, Sprite>();

        [HarmonyPostfix]
        public static void Postfix(DialogSignsView __instance, CharacterSOData character, ECharacterSign sign)
        {
            // Karater dictionary de varsa çekmeyi deniyoruz
            if (CharacterDatabase.CharacterMap.TryGetValue(character.name,out string animeName)&& sign ==ECharacterSign.Eye)
            {
                
                bool isDoppelganger = character._isImposter;

                //Duruma göre yüklenecek PNG adı
                string irisFileName = isDoppelganger ? $"{animeName}_Iris_Fake.png" : $"{animeName}_Iris_Normal.png";

                // Daha önce hafızaya alınmadıysa
                if (!eyeSpriteCache.ContainsKey(irisFileName))
                {
                    // "CustomImages" klasöründen resmi bul (BepInEx yolunu kendi yapına göre ayarla)
                    string irisPath = Path.Combine(BepInEx.Paths.PluginPath, "CustomImages", irisFileName);
                    if (File.Exists(irisPath))
                    {
                        byte[] fileData = File.ReadAllBytes(irisPath);
                        //Tuval oluşturduk
                        Texture2D customTexture = new Texture2D(2, 2);
                        //Oluşturulan 2 boyutlu tuvale okunan png yi loadladık
                        ImageConversion.LoadImage(customTexture, fileData);

                        
                        float targetPPU = 150f;
                        // okunup tuvale dökülen png yi sprite a çevirmek için kesme ve konum tanımlaması
                        Sprite customSprite = Sprite.Create(
                            customTexture,
                            new Rect(0, 0, customTexture.width, customTexture.height),
                            new Vector2(0.5f, 0.5f),
                            targetPPU
                        );
                        eyeSpriteCache[irisFileName] = customSprite;
                    }
                }

                //Oyunun yüklediği iris yerine kendi irisimiz
                if (eyeSpriteCache.ContainsKey(irisFileName))
                {
                    __instance._eyesContainer._iris.sprite = eyeSpriteCache[irisFileName];
                }
            }
        }
    }
}