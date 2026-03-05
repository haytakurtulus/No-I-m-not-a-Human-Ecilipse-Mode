using _Code.Characters;
using _Code.Rooms;
using _Code.Utils.UI.ImageAnimating;
using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;

namespace EcilipseMod.Patches
{
    // Kapıya gelen karakterlerin png sini değiştirmek için harmony patchi (sınıf adı ve metod)
    [HarmonyPatch(typeof(CharacterSOData), nameof(CharacterSOData.GetEmotion))]
    public class GetEmotion_Patch
    {
        // Bir kere yüklenen resmi cache de tutuyoruz
        private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        // Yüklenen karakterlerin bedenlerini not alıyoruz, böylece aynı karakterin bedenini birden fazla kez değiştirmiyoruz
        private static HashSet<string> bodyModifiedCharacters = new HashSet<string>();

        [HarmonyPostfix]
        // Metot çağrıldıktan sonra çalışacak kod bloğu
        //__instance: GetEmotion metodunu çağıran karakterin verisi, emotion: O anki duygu durumu, __result: GetEmotion metodunun döndürdüğü AnimationData (yüz ifadelerini içeren veri)
        //ref AnimationData __result: GetEmotion metodunun döndürdüğü AnimationData nesnesi ona müdahale ederek orjinali değiştiriyoruz
        public static void Postfix(CharacterSOData __instance, EDialogEmotionState emotion, ref AnimationData __result)
        {
            
            if (CharacterDatabase.CharacterMap.TryGetValue(__instance.name, out string animeName))
                { 
            // Veritabanından çekilen karaktere göre eylem gerçekleştir
            string emotionName = emotion.ToString();
            string emotionSuffix = "normal";
            switch (emotionName)
            {
                case "Happy":
                case "HappyEntrance":
                    emotionSuffix = "Happy";
                    break;
                case "Sad":
                case "SadEntrance":
                case "Sad_Head":
                    emotionSuffix = "Angry";
                    break;
                default:
                    emotionSuffix = "Normal";
                     break;
                    
            }
                
            string faceFileName = $"{animeName}_{emotionSuffix}.png";
            
            //Kontrol et eğer bu yüz ifadesi daha önce yüklenmemişse, diskte ara ve yükle
            if (!spriteCache.ContainsKey(faceFileName))
                {
                    string facePath = Path.Combine(Paths.PluginPath, "CustomImages", faceFileName);
                    if (File.Exists(facePath))
                    {
                        //resmin tamamını byte byte oku
                        byte[] fileData = File.ReadAllBytes(facePath);
                        // yeni bir Texture2D oluştur ve içine oku
                        Texture2D customTexture = new Texture2D(2, 2);
                        // resmi Texture2D'ye yükle
                        ImageConversion.LoadImage(customTexture, fileData);

                        // Yüz ifadeleri için boyut ve merkez 
                        Sprite customSprite = Sprite.Create(customTexture, new Rect(0, 0, customTexture.width, customTexture.height), new Vector2(0.5f, 0.5f), 50f);
                        spriteCache[faceFileName] = customSprite;
                    }
                }
                // Eğer resim hafızada varsa ve GetEmotion metodunun döndürdüğü AnimationData null değilse, png değiştir
                if (spriteCache.ContainsKey(faceFileName) && __result != null)
                {
                    int frameCount = 1;
                    //başlangıç 1 olmak üzere o spesifik yüz ifadesi kadar frame oluşturuyoruz (Çoğu karakter için 1'dir, ama bazıları için 2 veya daha fazla olabilir)
                    __result.Frames=new Il2CppReferenceArray<Sprite>(frameCount);
                    for (int i = 0; i < frameCount; i++)
                    {
                        __result.Frames[i] = spriteCache[faceFileName];
                    }
                }
            
            // ==============================================================
            // AŞAMA 2: Uzaktan duruş ve ceset 
            // ==============================================================


            // O karakter,daha önce güncellemediysek
            if (!bodyModifiedCharacters.Contains(__instance.name))
                {
                    //Karakterin Poses tanımı var mı
                    if (__instance.Poses != null)
                    {
                        //Tek bir döngü ile gerekli tüm pozlar güncellenir
                        for (int i = 0; i < __instance.Poses.Length; i++)
                        {
                            var roomState = __instance.Poses[i];
                            string stateName = roomState.Name.ToString();

                            Plugin.modLog.LogWarning($"Karakter: {__instance.name} | Aranan Durum (State): {stateName}");

                            // Default tanımı
                            bool resimiDegistir = true;
                            string poseSuffix = "Idle";
                            float pivotX = 0.5f;
                            float pivotY = 0.5f;
                            float sizePPU = 80f;

                            switch (stateName)
                            {
                                // --- Orjinal hali korumak için ---
                                case "Bags":
                                    resimiDegistir = false; 
                                    break;

                                // --- Eve giremeyen npc ler için ---
                                case "FullSize":
                                    poseSuffix = "Idle";
                                    pivotX = 0.5f;
                                    pivotY = 0.5f;
                                    sizePPU = 80f; 
                                    break;

                                
                                case "Staying":
                                case "Something":
                                    poseSuffix = "Idle";
                                    pivotX = 0.5f;
                                    pivotY = 0.3f;
                                    sizePPU = 60f; 
                                    break;

                                
                                case "Sitting":
                                case "Dude_Chilling":
                                    poseSuffix = "Idle";
                                    pivotX = 4.0f;
                                    pivotY = 2.0f;
                                    sizePPU = 15f; 
                                    break;

                                
                                case "Lying":
                                    poseSuffix = "Idle";
                                    pivotX = 0.7f;
                                    pivotY = 0.5f;
                                    sizePPU = 90f; 
                                    break;

                                // --- Tüm bu tanımlardan emin değilim ama ölümü çağrıştıranları buraya atadım ---
                                case "Corpse":
                                case "Dead":
                                case "Twisted":
                                case "Hanged":
                                case "Burned":
                                case "LostChildCorpse":
                                    poseSuffix = "Corpse";
                                    pivotX = 0.5f;
                                    pivotY = 0.1f;
                                    sizePPU = 60f; 
                                    break;

                                
                                default:
                                    poseSuffix = "Idle";
                                    pivotX = 0.5f;
                                    pivotY = 0.5f;
                                    sizePPU = 80f;
                                    break;
                            }

                            // Oyunun bags tanımını koruma amaçlı
                            if (!resimiDegistir)
                            {
                                continue; 
                            }

                            

                            string bodyFileName = $"{animeName}_{poseSuffix}.png";
                            string cacheKey = $"{bodyFileName}_{stateName}";

                            // Eğer hafızada resim yoksa
                            if (!spriteCache.ContainsKey(cacheKey))
                            {
                                string bodyPath = Path.Combine(Paths.PluginPath, "CustomImages", bodyFileName);
                                if (File.Exists(bodyPath))
                                {
                                    byte[] fileData = File.ReadAllBytes(bodyPath);
                                    Texture2D customTexture = new Texture2D(2, 2);
                                    ImageConversion.LoadImage(customTexture, fileData);

                                    Sprite customSprite = Sprite.Create(customTexture,
                                        new Rect(0, 0, customTexture.width, customTexture.height),
                                        new Vector2(pivotX, pivotY),
                                        sizePPU);

                                    spriteCache[cacheKey] = customSprite;
                                }
                            }

                          
                            if (spriteCache.ContainsKey(cacheKey))
                            {
                                roomState.Sprite = spriteCache[cacheKey];
                                Plugin.modLog.LogMessage($"Yerlesti: {stateName} (PivotX: {pivotX}, PPU: {sizePPU})");
                            }
                        }
                    }
                    bodyModifiedCharacters.Add(__instance.name);
                }
            }
            
        }
    }

}