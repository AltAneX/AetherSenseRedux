# AetherSense Redux


Turn your Warrior of Light into the Warrior of Butt with regex-powered realtime log parsing. Configure custom vibration patterns for game controllers, bluetooth-enabled sex toys, and more!

- Inspired by [AetherSense](https://github.com/Ms-Tress/AetherSense)
- Original by [digital pet](https://github.com/aka-tamagotchi/AetherSenseRedux/),
- Based on Fix by [AinaSnow](https://github.com/AinaSnow/AetherSenseRedux)
- Powered by [Buttplug](https://buttplug.io/)
- Controlled by you.

### Changes
#### 0.9.0.1
- Fixed for Dalamud 9
- Added option for all devices
- Added option for random devices
- Added a roulette chance option if you like a surprise
- Tidied up a code to bundle it 
- Added in stuff to allow future trigger expansion
- Client reconnects if dropped

### Installation

Unzip into %appdata%\XIVLauncher\devPlugins\ and restart the game.

Requires Intiface Desktop, XIVLauncher, and Dalamud to operate.

### Usage

TODO: Write usage full instructions

**Tip for the RegEx**
put .* at the end and start for a quick match
  `.*You casted.*`

if you want to exclude a word put to exclude the use of sprint but all uses
  `.*You use(?:(?!Sprint).)*$`

If you turn on the log chat to debug you can see what messages the addon will see in the debug tab or in your log file (%appdata%\XIVLauncher\dalamud.log)
### Support

This project is a labor of love, and I don't earn any money for developing it. But if you've gotten something out of it and want to give back, please, buy me a coffee!

### Credits

- Uses [XIVChatTools](https://github.com/digital-pet/XIVChatTools) for extended filtering capabilities.
- Inspired by the original [AetherSense](https://github.com/Ms-Tress/AetherSense) plugin by Ms Tress.
