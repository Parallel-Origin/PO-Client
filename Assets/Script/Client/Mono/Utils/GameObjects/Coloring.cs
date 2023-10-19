using System.Collections.Generic;
using UnityEngine;

namespace Script.Client.Mono.Utils.GameObjects {
    /// <summary>
    ///     This class modifies all component in the list with the given hex color.
    /// </summary>
    public class Coloring : MonoBehaviour {
        public string hexColor;
        public List<Component> componentsWithColors;

        public void SetColors(string hexColor) {
            this.hexColor = hexColor;

            Color hex;
            ColorUtility.TryParseHtmlString(hexColor, out hex);

            foreach (var compo in componentsWithColors)
                if (compo.GetType().Equals(typeof(LineRenderer))) {
                    var lr = (LineRenderer) compo;
                    lr.startColor = hex;
                    lr.endColor = hex;
                }
                else if (compo.GetType().Equals(typeof(SpriteRenderer))) {
                    var sr = (SpriteRenderer) compo;

                    var modifiedHex = hex;
                    modifiedHex.a = sr.color.a;

                    sr.color = modifiedHex;
                }
                else if (compo.GetType().Equals(typeof(MeshRenderer))) {
                    var mr = (MeshRenderer) compo;

                    var modifiedHex = hex;
                    modifiedHex.a = mr.material.color.a;

                    mr.material = new Material(mr.material);
                    mr.material.color = modifiedHex;
                }
        }
    }
}