using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draws text in 3D space.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Text")]
        public GameObject[] textObjects = new GameObject[4];
        public GameObject textPrefab;
        public GameObject textsGroup;

        private float[] textDurations = new float[4];
        private int textIndexEnd;
        private TMPro.TextMeshProUGUI[] texts = new TMPro.TextMeshProUGUI[4];

        /// <summary>
        /// Draws text at a position in 3D space. Text always faces the camera.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="position">The text center.</param>
        /// <param name="color">The font color.</param>
        /// <param name="yOffset">The text center height above the given position.</param>
        /// <param name="duration">The number of seconds the text should be visible for.</param>
        public void DrawText(string text, Vector3 position, Color color, float yOffset = 0.0f, float duration = 0.0f)
        {
            DrawText(text, position, Vector3.one, color, yOffset, duration);
        }

        /// <summary>
        /// Draws text at a position in 3D space with a given scale. Text always faces the camera.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="position">The text center.</param>
        /// <param name="scale">The text scale.</param>
        /// <param name="color">The font color.</param>
        /// <param name="yOffset">The text center height above the given position.</param>
        /// <param name="duration">The number of seconds the text should be visible for.</param>
        public void DrawText(string text, Vector3 position, Vector3 scale, Color color, float yOffset = 0.0f, float duration = 0.0f)
        {
            var textMesh = AllocateText();

            if (textMesh == null)
            {
                return;
            }

            var textObject = textObjects[textIndexEnd];
            textMesh.color = color;
            textMesh.text = text;
            textMesh.enabled = true;
            textObject.transform.position = position + yOffset * Vector3.up;
            textObject.transform.localScale = scale;
            textDurations[textIndexEnd] = duration;
            textIndexEnd += 1;
        }

        private TMPro.TextMeshProUGUI AllocateText()
        {
            if (!gameObject.activeInHierarchy)
            {
                return null;
            }

            if (textIndexEnd > texts.Length - 1)
            {
                var textCount = texts.Length;
                var newTextCount = 2 * textCount;
                var newTextDurations = new float[newTextCount];
                var newTextObjects = new GameObject[newTextCount];
                var newTexts = new TMPro.TextMeshProUGUI[newTextCount];
                System.Array.Copy(textDurations, newTextDurations, textCount);
                System.Array.Copy(textObjects, newTextObjects, textCount);
                System.Array.Copy(texts, newTexts, textCount);

                for (var i = textCount; i < newTextCount; i++)
                {
                    newTextObjects[i] = Instantiate(textPrefab, textsGroup.transform);
                    newTexts[i] = newTextObjects[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                }

                textDurations = newTextDurations;
                textObjects = newTextObjects;
                texts = newTexts;
            }

            return texts[textIndexEnd];
        }

        private void DeallocateText(int index)
        {
            var lastTextIndex = textIndexEnd - 1;

            texts[index].enabled = false;
            textIndexEnd -= 1;

            if (index == lastTextIndex)
            {
                return;
            }

            textDurations[index] = textDurations[lastTextIndex];

            var tempText = texts[index];
            texts[index] = texts[lastTextIndex];
            texts[lastTextIndex] = tempText;

            var tempTextObjects = textObjects[index];
            textObjects[index] = textObjects[lastTextIndex];
            textObjects[lastTextIndex] = tempTextObjects;
        }

        private void StartTextDrawer()
        {
            for (var i = 0; i < textObjects.Length; i++)
            {
                texts[i] = textObjects[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
        }

        private void UpdateTextDrawer()
        {
            var i = 0;

            while (i < textIndexEnd)
            {
                textDurations[i] -= Time.deltaTime;

                if (textDurations[i] <= 0.0f)
                {
                    DeallocateText(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
