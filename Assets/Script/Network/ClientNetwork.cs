using LiteNetLib;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;

namespace Script.Network {

    public delegate void OnConnected(NetPeer peer);
    public delegate void OnBadUsername(NetPeer peer, ErrorCommand command);
    public delegate void OnBadPassword(NetPeer peer, ErrorCommand command);
    public delegate void OnLogin(NetPeer peer, in Character character);
    public delegate void OnLogout(NetPeer peer);
    public delegate void OnDisconnected(NetPeer peer, DisconnectInfo info);
    
    /// <summary>
    /// Extends the <see cref="ParallelOrigin.Core.Network.Network"/> for adding some extra structure for client only. 
    /// </summary>
    public partial class ClientNetwork : ParallelOrigin.Core.Network.Network{

        protected override void Setup() {
            base.Setup();

            // Otherwhise invoking them causes a null reference
            if(OnConnected == null) OnConnected = peer => { };
            if(OnLogin == null) OnLogin = (NetPeer peer, in Character character) => { };
            if(OnBadUsername == null) OnBadUsername = (peer, command) => { };
            if(OnBadPassword == null) OnBadPassword = (peer, command) => { };
            if(OnLogout == null) OnLogout = peer => { };
            if(OnDisconnected == null) OnDisconnected = (peer, info) => { };
            
            // Forward client events 
            OnReceive((command, peer) => {

                switch (command.Error) {
                    
                    case Error.BadUsername:
                        
                        OnBadUsername(peer, command);
                        break;
                    
                    case Error.BadPassword:

                        OnBadPassword(peer, command);
                        break;
                }
            }, () => new ErrorCommand());
            OnReceive(LoginSucessfull, () => new LoginResponse());
            
            Listener.PeerConnectedEvent += ConnectionSucessfull;
            Listener.PeerDisconnectedEvent += DisconnectedSucessfull;
        }

        public OnConnected OnConnected { get; set; }
        public OnBadUsername OnBadUsername { get; set; }
        public OnBadPassword OnBadPassword { get; set; }
        public OnLogin OnLogin { get; set; }
        public OnLogout OnLogout { get; set; }
        public OnDisconnected OnDisconnected { get; set; }
    }

    /// <summary>
    /// AN partial class for the <see cref="ClientNetwork"/> which mostly contains connection based logic. 
    /// </summary>
    public partial class ClientNetwork {

        /// <summary>
        /// Returns true if the client is still connected to the server. 
        /// </summary>
        /// <returns></returns>
        public bool Connected => Manager.FirstPeer.ConnectionState == ConnectionState.Connected;
        
        /// <summary>
        /// Gets called once a connection was cuessfull.
        /// </summary>
        /// <param name="peer"></param>
        private void ConnectionSucessfull(NetPeer peer) {
            OnConnected(peer);
        }
        
        /// <summary>
        /// Gets invoked once a <see cref="LoginResponse"/> was received and the login was sucessfull. 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        private void LoginSucessfull(LoginResponse response, NetPeer peer) {
            OnLogin(peer, response.Character);
        }

        /// <summary>
        /// Gets called upon disconnection and invokes the fitting event 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="info"></param>
        private void DisconnectedSucessfull(NetPeer peer, DisconnectInfo info) {
            OnDisconnected(peer, info);
        }
    }
}