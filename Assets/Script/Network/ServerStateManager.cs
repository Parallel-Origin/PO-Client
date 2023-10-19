using System.Collections;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Network {

    /// <summary>
    ///     This class is used to test the server state...
    ///     It will try to ping the server once the start method is called.
    ///     If it pings the server in the actuall time frame the pingSucessfull(); method will be called... otherwise pingUnsucessfull();
    ///     It will also controll some parts of the graphical user interface to display the server status.
    ///     It uses the IP-Adress of the connection manager.
    /// </summary>
    public class ServerStateManager : MonoBehaviour {

        [SerializeField] public TextMeshProUGUI serverStateText;
        
        [SerializeField] public Color colorWhenSucessfull;
        [SerializeField] public Color colorWhenUnsucessfull;

        [SerializeField] public UnityEvent serverReachable;
        [SerializeField] public UnityEvent serverUnreachable;

        [SerializeField] private Coroutine _ping;

        private void Start() {

            var network = ServiceLocator.Get<ClientNetwork>();
            CheckPing(network.Ip);
        }


        /// <summary>
        /// This method starts the coroutine which will test the ping.
        /// </summary>
        /// <param name="ip"></param>
        public void CheckPing(string ip) { _ping = StartCoroutine(StartPing(ip)); }

        /// <summary>
        ///  This method starts the pinging.
        ///  It tries to ping the ip in a regular intervall till it either gets a unsucessfull or sucessfull event.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private IEnumerator StartPing(string ip) {

            var f = new WaitForSeconds(0.05f);
            var p = new Ping(ip);

            var testCount = 0;
            while (p.isDone == false) {

                if (testCount >= 10) {

                    PingUnsucessfull();
                    StopCoroutine(_ping);
                }

                testCount++;
                yield return f;
            }

            PingSucessfull(p);
        }

        /// <summary>
        ///     This method gets called once the ping was sucessfull.
        ///     I will change the color of the server state UI text and invokes the event.
        /// </summary>
        /// <param name="ping"></param>
        protected void PingSucessfull(Ping ping) {

            serverStateText.color = colorWhenSucessfull;
            serverReachable.Invoke();
        }


        /// <summary>
        ///     This method gets called if the ping was unsucessfull.
        ///     I will change the color of the server state UI text and invokes the event.
        /// </summary>
        protected void PingUnsucessfull() {

            serverStateText.color = colorWhenUnsucessfull;
            serverUnreachable.Invoke();
        }
    }
}