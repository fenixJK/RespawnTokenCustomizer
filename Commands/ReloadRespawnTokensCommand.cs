namespace RespawnTokenCustomizer.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ReloadRespawnTokensCommand : ICommand
    {
        public string Command => "respawntokensreload";

        public string[] Aliases => new[] { "rtreload", "reloadrespawntokens" };

        public string Description => "Reloads only Respawn Token Customizer config.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("respawntokencustomizer.reload"))
            {
                response = "You do not have the respawntokencustomizer.reload permission.";
                return false;
            }

            if (Plugin.Instance is null)
            {
                response = "Respawn Token Customizer plugin instance was not found.";
                return false;
            }

            IPlugin<IConfig> plugin = Loader.GetPlugin(Plugin.Instance.Prefix);

            if (plugin is null)
            {
                response = "Respawn Token Customizer plugin instance was not found.";
                return false;
            }

            try
            {
                plugin.LoadConfig();
                Plugin.Instance.ReloadRuntime();

                if (plugin.Config.Debug)
                    Log.DebugEnabled.Add(plugin.Assembly);
                else
                    Log.DebugEnabled.Remove(plugin.Assembly);

                response = "Respawn Token Customizer config reloaded.";
                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"Respawn Token Customizer config reload failed: {exception}");
                response = "Respawn Token Customizer config reload failed. Check the server console for details.";
                return false;
            }
        }
    }
}
