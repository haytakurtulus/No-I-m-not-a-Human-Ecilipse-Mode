🌑 Ecilipse Mod: No I'm Not a Human (v0.1 Alpha)
This project started with a simple question: "Can I actually pull this off?" As a Computer Engineering student and an anime fan, I wanted to see if I could intercept a Unity game's logic to bring my favorite characters into this dark universe. This is a work-in-progress, experimental build focused on Makima (replacing the 'Anxiety' character).

🛠️ The Tech Stack (How I Made It)
C# & Harmony: The backbone. Used for hooking into the game's internal methods without touching the original files.

Il2CppSpy: My detective tool. Essential for reverse engineering the game's code to find the exact methods to "patch."

BepInEx: The bridge that allows my code to run inside the game engine.

Stable Diffusion: Used Text2Img to generate the base character designs and Img2Img to ensure consistent facial expressions for different emotions.

Adobe Photoshop: Where the manual labor happens—fixing AI artifacts, cleaning edges, and managing transparency/padding for UI alignment.

🎞️ Mod Preview

![Animation4](https://github.com/user-attachments/assets/4f6a1143-b3af-4f99-8895-196ac56cea52)
![202603~3](https://github.com/user-attachments/assets/50868fae-6069-4131-9a11-dca588ae22b1)
![202602~2](https://github.com/user-attachments/assets/59839b7c-256c-400e-9ba6-f361d6bcdb2d)
 
✨ Current State & Future Vision
Right now, it's a proof of concept. But I have big plans:

Lore Integration: Why are these characters here? I’m working on custom backstories to explain their presence in this world.

Voice & Dialogue: Replacing original lines with character-accurate quotes.

Animation: Adding frame-by-frame animations for a more "alive" feel.

Alternative Endings: If I can manage it, I want to create custom ending sequences for specific characters.

⚠️ Known Issues (The "Work in Progress" Part)
Being v0.1, it's not perfect. If you're a developer or a more experienced modder, I’d love your feedback or advice!

Stability: The game occasionally crashes on the first launch. I'm still hunting down the cause (likely a race condition during asset loading).

Positioning: Makima's "FullSize" (far distance) pose isn't centering exactly how I want it yet.

Memory: The cache system is solid, but I'm looking for ways to make the initial texture injection even smoother.

📥 Quick Install
Install BepInEx 5.

Drop EcilipseMod.dll and the CustomImages folder into your BepInEx/plugins directory.

Start the game.
