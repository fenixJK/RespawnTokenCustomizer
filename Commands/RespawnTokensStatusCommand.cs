namespace RespawnTokenCustomizer.Commands
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class RespawnTokensStatusCommand : ICommand
    {
        public string Command => "respawntokensstatus";

        public string[] Aliases => new[] { "rtstatus", "respawntokens" };

        public string Description => "Prints current main-wave respawn token state.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("respawntokencustomizer.inspect"))
            {
                response = "You do not have the respawntokencustomizer.inspect permission.";
                return false;
            }

            if (Plugin.Instance?.TokenService is null)
            {
                response = "Respawn Token Customizer plugin instance was not found.";
                return false;
            }

            response = Plugin.Instance.TokenService.BuildStatus();
            return true;
        }
    }
}
