using System;
using System.Collections;
using System.Text;
using Script.Client.Mono.Utils.GameObjects;
using UnityEngine;

namespace Script.Client.Mono.Utils.News {
    /// <summary>
    ///     This class is used to download the changelog/news from a txt file stored in the google drive.
    ///     It checks if there was already the new / changelog shown and if so it wont be displayed.
    /// </summary>
    [RequireComponent(typeof(PrefabSpawner))]
    public class NewsReceiver : MonoBehaviour {
        [SerializeField] private int showAfter;
        [SerializeField] private string newsDownloadUrl;

        [SerializeField] private string titleString;
        [SerializeField] private string newsString;

        [SerializeField] private GameObject popup;
        [SerializeField] private PrefabSpawner prefabSpawner;

        private void Start() {
            if (!prefabSpawner)
                prefabSpawner = GetComponent<PrefabSpawner>();
        }

        /// <summary>
        ///     Starts a coroutine to receive the news from the google drive link.
        /// </summary>
        public void GetNews() { StartCoroutine(Download()); }


        /// <summary>
        ///     Starts a IEnumerator to download the news txt file from the url.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Download() {
            var webRequest = new WWW(newsDownloadUrl);
            yield return webRequest;

            Extract(webRequest);
            ShowNews();
        }

        /// <summary>
        ///     Extracts the string data from the downloaded text file.
        ///     Futhermore the first row will be designated as the title and the rest as the news body.
        /// </summary>
        /// <param name="webRequest"></param>
        protected void Extract(WWW webRequest) {
            var split = webRequest.text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            titleString = split[1];

            var stringBuilder = new StringBuilder();

            for (var index = 2; index < split.Length; index++) {
                var row = split[index];
                if (row != null)
                    stringBuilder.AppendLine(row);
            }

            newsString = stringBuilder.ToString();
        }

        /// <summary>
        ///     Just checks if the news are valid... using the player prefs for that.
        ///     If the news already once appeared they will be saved there => false will be returned.
        /// </summary>
        /// <returns></returns>
        protected bool NewNews() { return !(PlayerPrefs.HasKey(titleString) && PlayerPrefs.GetString(titleString, "false") == newsString); }


        /// <summary>
        ///     Spawns the news popup and checks if the news are valid.
        /// </summary>
        protected void ShowNews() {
            if (!NewNews()) return;

            //var newsField = prefabSpawner.Spawn(popup.gameObject);
            //var news = newsField.GetComponent<UIField>();

            /*
            news.Headline = titleString;
            news.Text = newsString;*/

            PlayerPrefs.SetString(titleString, newsString);
        }
    }
}