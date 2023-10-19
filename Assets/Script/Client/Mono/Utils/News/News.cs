using TMPro;
using UnityEngine;

namespace Script.Client.Mono.Utils.News {
    /// <summary>
    ///     This class is attached to a news popup as a reference to the title & news text fields.
    /// </summary>
    public class News : MonoBehaviour {
        public TextMeshProUGUI title;
        public TextMeshProUGUI news;

        public void Fill(string title, string news) {
            this.title.SetText(title);
            this.news.SetText(news);
        }
    }
}