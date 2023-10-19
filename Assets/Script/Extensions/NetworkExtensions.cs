using LiteNetLib;
using Script.Network;
using Script.Server;
using LoginCommand = ParallelOrigin.Core.Network.LoginCommand;

namespace Script.Extensions {
    
    /// <summary>
    /// Some extension methods for the <see cref="Network"/> and <see cref="ClientNetwork"/>
    /// </summary>
    public static class NetworkExtensions {

        /// <summary>
        /// Sends a login request to the server... the server should respond 
        /// </summary>
        /// <param name="network"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void Login(this ClientNetwork network, string username, string password) {

            var loginCommand = new LoginCommand{Username = username, Password = password};
            network.Send(ref loginCommand);
        }
    }
}