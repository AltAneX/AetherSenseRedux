using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using AetherSenseReduxToo.Toy.Pattern;
using AetherSenseReduxToo.Toy;
using AetherSenseReduxToo.Enums;
using AetherSenseReduxToo.Game.Trigger;
using AetherSenseReduxToo.UI;

namespace AetherSenseReduxToo
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "AetherSense Redux Too";

        private const string commandName = "/asr";

        private IDalamudPluginInterface Dalamud { get; init; }
        private PluginUI PluginUi { get; init; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginInterface"></param>
        /// <param name="commandManager"></param>
        public Plugin(
            IDalamudPluginInterface pluginInterface,
            ICommandManager commandManager)
        {
            Dalamud = pluginInterface;

            Service.Initialize(pluginInterface);
            Service.Configuration = Dalamud.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Dalamud);
            Service.Configuration.FixDeserialization();

            ToyService.Initialize();
            TriggerManager.Initialize();


            Dalamud.Inject(this);



            // Update the configuration if it's an older version
            if (Service.Configuration.Version == 1)
            {
                Service.Configuration.Version = 2;
                Service.Configuration.FirstRun = false;
                Service.Configuration.Save();
            }
            
            if (Service.Configuration.FirstRun)
            {
                Service.Configuration.LoadDefaults();
            }

            PluginUi = new PluginUI(Service.Configuration, this);

            Service.Commands.AddHandler(commandName, new CommandInfo(OnShowUI)
            {
                HelpMessage = "Opens the Aether Sense Redux configuration window"
            });

            Dalamud.UiBuilder.Draw += DrawUI;
            Dalamud.UiBuilder.OpenMainUi += DrawMainUI;
            Dalamud.UiBuilder.OpenConfigUi += DrawConfigUI;

            ToyService.Start();
            TriggerManager.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            TriggerManager.Shutdown();
            ToyService.Shutdown();
            PluginUi.Dispose();
            Service.Commands.RemoveHandler(commandName);
        }

                                 //(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        private void OnShowUI(string command, string args)
        {
            // in response to the slash command, just display our main ui
            PluginUi.SettingsVisible = true;
        }
        // END EVENT HANDLERS

        // SOME FUNCTIONS THAT DO THINGS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternConfig">A pattern configuration.</param>


        /// 

        /// <summary>
        /// 
        /// </summary>
        


        // END START AND STOP FUNCTIONS

        // UI FUNCTIONS
        /// <summary>
        /// 
        /// </summary>
        private void DrawUI()
        {
            PluginUi.Draw();
        }
        private void DrawConfigUI()
        {
            PluginUi.SettingsVisible = !PluginUi.SettingsVisible;
        }
        private void DrawMainUI()
        {
            PluginUi.SettingsVisible = !PluginUi.SettingsVisible;
        }
        // END UI FUNCTIONS

    }

    
}
