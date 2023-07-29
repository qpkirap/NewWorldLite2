using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VNCreator
{
   public struct GUIParams
    {
        public const string red = "FF3730";
        public const string lightRed = "FF7C7A";
        public const string green = "6ECC24";
        public const string lightGreen = "C1FFB5";

        private List<GUILayoutOption> options;

        public Color? DefaultContentColor { get; private set; }
        public Color? ContentColor { get; private set; }

        public Color? DefaultBgColor { get; private set; }
        public Color? BgColor { get; private set; }

        public Color? DefaultColor { get; private set; }
        public Color? Color { get; private set; }

        public GUILayoutOption[] Options => options?.ToArray();

        public GUIStyle Style { get; private set; }
        public Color? Normal { get; private set; }
        public Color? Hover { get; private set; }
        public Color? Focused { get; private set; }

        public Texture Texture { get; private set; }

        public bool ReadOnly { get; private set; }

        public static GUIParams New() => default;

        #region GUI Options
        public GUIParams WithOptions(params GUILayoutOption[] options)
        {
            InitOptions();

            if (options != null) this.options.AddRange(options);

            return this;
        }

        private void InitOptions()
        {
            if (options == null) options = new List<GUILayoutOption>();
        }

        public GUIParams WithReadOnly(bool state = true)
        {
            ReadOnly = state;

            return this;
        }

        public GUIParams WithWidth(float value)
        {
            InitOptions();

            if (value > 0) options.Add(GUILayout.Width(value));

            return this;
        }

        public GUIParams WithHeight(float value)
        {
            InitOptions();

            if (value > 0) options.Add(GUILayout.Height(value));

            return this;
        }

        public GUIParams WithMaxHeight(float value)
        {
            InitOptions();

            if (value > 0) options.Add(GUILayout.MaxHeight(value));

            return this;
        }

        public GUIParams WithMaxWidth(float value)
        {
            InitOptions();

            if (value > 0) options.Add(GUILayout.MaxWidth(value));

            return this;
        }

        public GUIParams WithMinHeight(float value)
        {
            InitOptions();

            if (value > 0) options.Add(GUILayout.MinHeight(value));

            return this;
        }

        public GUIParams WithMinWidth(float value)
        {
            InitOptions();

            if (value > 0) options.Add(GUILayout.MinWidth(value));

            return this;
        }

        public GUIParams WithExpandHeight(bool value)
        {
            InitOptions();

            options.Add(GUILayout.ExpandHeight(value));

            return this;
        }

        public GUIParams WithExpandWidth(bool value)
        {
            InitOptions();

            options.Add(GUILayout.ExpandWidth(value));

            return this;
        }
        #endregion

        #region GUI Colors
        public GUIParams WithBgColor(string color)
        {
            return WithBgColor(color.ToColor());
        }

        public GUIParams WithBgColor(Color color)
        {
            DefaultBgColor = GUI.backgroundColor;
            BgColor = color;
            return this;
        }

        public GUIParams WithContentColor(string color)
        {
            return WithContentColor(color.ToColor());
        }

        public GUIParams WithContentColor(Color color)
        {
            DefaultContentColor = GUI.contentColor;
            ContentColor = color;
            return this;
        }

        public GUIParams WithColor(string color)
        {
            return WithColor(color);
        }

        public GUIParams WithColor(Color color)
        {
            DefaultColor = GUI.color;
            Color = color;
            return this;
        }
        #endregion

        #region Sprites
        public GUIParams WithTexture(string assetName)
        {
            Texture = EditorUtils.LoadAsset<Texture>(assetName, type: "Texture");

            return this;
        }
        #endregion Sprites

        #region Custom Style
        public GUIStyle CreateStyle(GUIStyle baseStyle = null)
        {
            var style = new GUIStyle(baseStyle ?? Style ?? EditorStyles.label);

            if (Normal.HasValue) style.normal = new GUIStyleState { textColor = Normal.Value };
            if (Hover.HasValue) style.hover = new GUIStyleState { textColor = Hover.Value };
            if (Focused.HasValue) style.focused = new GUIStyleState { textColor = Focused.Value };

            return style;
        }

        public GUIParams WithStyle(GUIStyle style)
        {
            Style = style;
            return this;
        }

        public GUIParams WithFieldTitleColor(Color normal, Color? hover = null, Color? focused = null)
        {
            Normal = normal;
            Hover = hover ?? normal;
            Focused = focused ?? normal;
            return this;
        }
        #endregion
    }
}