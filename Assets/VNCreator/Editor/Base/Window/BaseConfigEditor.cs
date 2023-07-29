using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VNCreator
{
    public abstract class BaseConfigEditor<TConfig> : BaseConfigEditor
        where TConfig : BaseConfig
    {
        public new TConfig Config => base.Config as TConfig;
        public override Type ConfigType => typeof(TConfig);
    }
    
    public abstract class BaseConfigEditor : BaseEditorWindow, IHasCustomMenu
    {
        [SerializeField] private BaseConfig config;

        protected bool baseParamsOn;

        private int tabIndex;
        private Vector2 tabScroll;

        public virtual List<(string title, Action onDraw)> DataTabs => null;

        public BaseConfig Config => config;
        public abstract Type ConfigType { get; }

        /// <summary> default: <c>true</c> </summary>
        protected virtual bool InitOnEnable => true;

        public override void OnEnable()
        {
            var time = DateTime.Now;

            base.OnEnable();

            if (InitOnEnable || Config)
            {
                Init(Config);
            }

            if (Config)
            {
                Debug.Log($"{Config.GetType().Name}: {(DateTime.Now - time).TotalSeconds}s");
            }
        }

        public void Init(BaseConfig config)
        {
            this.config = config != null ? config : EditorUtils.LoadAsset(ConfigType) as BaseConfig;

            if (Config != null)
            {
                InitConfigData();
            }
        }
        
        protected virtual void InitConfigData()
        {
            CheckConfigEditorData();
        }

        public override void DrawEditor()
        {
            if (!CheckConfigEditorData())
            {
                return;
            }

            if (Config != null && Config.HasConfigFolder())
            {
                /*EditorUtils.DrawBox(onDrawContent: () =>
                {
                    var dataTabs = DataTabs;

                    if (dataTabs == null)
                    {
                        DrawTabs();
                    }
                    else
                    {
                        EditorUtils.DrawTabsContainer(
                            tabIndex: ref tabIndex,
                            tabScroll: ref tabScroll,
                            tabsInRowCount: int.MaxValue,
                            drawParams: GUIParams.New().WithHeight(21),
                            tabs: dataTabs);
                    }
                });*/

                DrawBaseConfigParams();
            }
            else
            {
                DrawCreateConfig();
            }
        }

        protected virtual bool CheckConfigEditorData()
        {
            if (Config != null && !Config.HasConfigFolder())
            {
                var configFilePath = AssetDatabase.GetAssetPath(Config);

                if (!string.IsNullOrEmpty(configFilePath))
                {
                    var folderAsset = EditorUtils.GetFolderAsset(configFilePath);

                    Config.SetValue("configFolder", folderAsset);
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        protected virtual void DrawCreateConfig()
        {
            EditorUtils.DrawButton(
                title: $"Создать {ConfigType.Name}",
                drawParams: GUIParams.New().WithHeight(40).WithColor(GUIParams.green),
                action: CreateConfigCallback);
        }

        public void DrawBaseConfigParams()
        {
            baseParamsOn = EditorUtils.DrawOpenBox("Config Params", baseParamsOn, onDrawContent: () =>
            {
                Config.DrawValueEditor<Object, DefaultAsset>("Config Folder", "configFolder");

                DrawConfigParams();
            });
        }

        protected virtual void DrawConfigParams()
        {
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Save"), on: false, EditorUtils.SaveAssets);
        }

        public override void CreateMenuItems(List<Action> items)
        {
            base.CreateMenuItems(items);

            items.Insert(0, EditorUtils.DrawSaveButton);
        }

        #region [Create] Callbacks
        private void CreateConfigCallback()
        {
            var config = InstantiateConfig() as BaseConfig;

            SaveConfig(config);
        }
        #endregion [Create] Callbacks

        #region [Instantiate, Save, Remove]
        public object InstantiateConfig()
        {
            return EditorUtils.InstantiateEntity(ConfigType);
        }

        public void SaveConfig(object config)
        {
            var defaultName = ConfigType.Name;
            var configFilePath = EditorUtility.SaveFilePanelInProject($"Создание конфига: {defaultName}", defaultName, "asset", "");

            SaveConfig(config, configFilePath);
        }

        public void SaveConfig(object config, string configFilePath)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                var asset = config as BaseConfig;

                AssetDatabase.CreateAsset(asset, configFilePath);

                Init(asset);
            }
        }

        public virtual void RemoveConfig()
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Config));
        }
        #endregion [Instantiate, Save, Remove]

        #region CreateWindow
        
        protected static TWindow CreateWindow<TWindow>(BaseConfig config, float width = 450, float height = 600)
            where TWindow : BaseConfigEditor, new()
        {
            var editor = CreateWindow<TWindow>(width: width, height: height);

            editor.Init(config);

            return editor;
        }

        protected static TWindow CreateWindow<TWindow>(float width = 450, float height = 600)
            where TWindow : BaseEditorWindow, new()
        {
            return EditorUtils.CreateWindow<TWindow>(width: width, height: height);
        }
        #endregion CreateWindow

        public override void CloseEditor()
        {
            EditorWindowBuffer.Reset();

            base.CloseEditor();
        }

        public override void CheckDirty()
        {
            EditorUtils.SetDirty(Config);

            base.CheckDirty();
        }
    }
}