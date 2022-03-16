using System;
using System.IO;

using UnityEditor;

using UnityEngine;
using UnityEngine.Networking;

using AwtterSDK.Editor.Enums;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using AwtterSDK.Editor.PackagesTreeView;
using UnityEditor.TreeViewExamples;
using System.Collections.Generic;

namespace AwtterSDK
{
    class AwtterSdkAssets : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AwtterSdkInstaller.CheckPackages = true;
        }
    }

    public class AwtterSdkInstaller : EditorWindow
    {
        public static bool CheckPackages = true;

        PackageElement _vrcSdk = new PackageElement("Vrc SDK", PackageStatus.None, "-", 0, 1);
        PackageElement _poyiomi = new PackageElement("Poyiomi", PackageStatus.None, "-", 0, 2);
        PackageElement _baseModel = new PackageElement("Base Model", PackageStatus.None, "-", 0, 3);

        bool _vrcSdkInstalled, _poiyomiInstalled, _baseInstalled;
        string _vrcSdkVersion, _poyiomiVersion, _baseVersion;
        BaseType _selectedBase = BaseType.None;

        static AwtterSdkInstaller _window;

        [NonSerialized] bool m_Initialized;
        [SerializeField] TreeViewState m_TreeViewState;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        PackagesView m_TreeView;

        [MenuItem("Awtter SDK/Open Installer")]
        static void Init()
        {
            _window = (AwtterSdkInstaller)EditorWindow.GetWindowWithRect(typeof(AwtterSdkInstaller), new Rect(0, 0, 525, 208), false, "Awtter SDK | Installer");
            _window.Show();
            CheckPackages = true;
        }

        void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState = PackagesView.CreateDefaultMultiColumnHeaderState(100);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new MyMultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<PackageElement>(new List<PackageElement>() 
                {
                    new PackageElement("-", PackageStatus.None, "-", -1, 0),
                    this._vrcSdk,
                    this._poyiomi,
                    this._baseModel,
                });

                m_TreeView = new PackagesView(m_TreeViewState, multiColumnHeader, treeModel);
                m_Initialized = true;
            }
        }

        void CheckInstalledPackages()
        {
            _vrcSdkInstalled = Directory.Exists("Assets/VRCSDK");
            if (_vrcSdkInstalled)
            {
                if (File.Exists("Assets/VRCSDK/version.txt"))
                    _vrcSdkVersion = File.ReadAllText("Assets/VRCSDK/version.txt");
            }
            else
                _vrcSdkVersion = null;
            m_TreeView.treeModel.m_Data[1].Status = _vrcSdkInstalled ? PackageStatus.Installed : PackageStatus.NotInstalled;
            m_TreeView.treeModel.m_Data[1].Version = _vrcSdkVersion ?? "-";

            _poiyomiInstalled = Directory.Exists("Assets/_PoiyomiShaders");
            if (_poiyomiInstalled)
            {
                if (File.Exists("Assets/_PoiyomiShaders/Shaders/Pro/Shaders/S_Poiyomi_Toon.shader"))
                {
                    var content = File.ReadAllLines("Assets/_PoiyomiShaders/Shaders/Pro/Shaders/S_Poiyomi_Toon.shader");
                    var versionLine = content.FirstOrDefault(p => p.Contains("shader_master_label"));
                    if (versionLine != null)
                    {
                        var line = versionLine.Split('(', ')')[1];
                        var sLine = line.Split(' ').FirstOrDefault(p => p.Contains("V"));
                        _poyiomiVersion = sLine.Substring(1, sLine.IndexOf('<') - 1);
                    }
                }
            }
            else
                _poyiomiVersion = null;
            m_TreeView.treeModel.m_Data[2].Status = _poiyomiInstalled ? PackageStatus.Installed : PackageStatus.NotInstalled;
            m_TreeView.treeModel.m_Data[2].Version = _poyiomiVersion ?? "-";

            _baseInstalled = Directory.Exists("Assets/1) Model (fbx)");
            if (_baseInstalled)
            {
                foreach (var file in new DirectoryInfo("Assets/1) Model (fbx)").GetFiles("*.fbx"))
                {
                    var sp = file.Name.Split(' ');
                    _baseVersion = sp.FirstOrDefault(p => p.Contains("."));
                    break;
                }
            }
            else
                _baseVersion = null;
            m_TreeView.treeModel.m_Data[3].Status = _baseInstalled ? PackageStatus.Installed : PackageStatus.NotInstalled;
            m_TreeView.treeModel.m_Data[3].Version = _baseVersion ?? "-";
            m_TreeView.treeModel.Changed();
            CheckPackages = false;
        }

        void OnGUI()
        {
            InitIfNeeded();
            if (CheckPackages) CheckInstalledPackages();
            TopToolBar(toolbarRect);
            m_TreeView.OnGUI(packagesTreeViewRect);
            BottomToolBar(bottomToolbarRect);
        }

        void TopToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);
            CreateBox("Install status");
            GUILayout.EndArea();
        }

        void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);
            CreateBox("Install options");
            _selectedBase = (BaseType)EditorGUILayout.EnumPopup("Model", _selectedBase);
            if (_selectedBase == BaseType.None) GUI.enabled = false;
            if (GUILayout.Button("Install"))
            {
                if (!_vrcSdkInstalled)
                    DownloadFile("https://files.vrchat.cloud/sdk/VRCSDK3-AVATAR-2022.02.16.19.13_Public.unitypackage", "Downloading VrcSDK", "Downloading in progress...");
                if (!_poiyomiInstalled)
                    DownloadFile("https://github.com/poiyomi/PoiyomiToonShader/releases/download/V7.3.050/PoiyomiToon7.3.050.unitypackage", "Downloading Poiyomi", "Downloading in progress...");
                if (!_baseInstalled)
                    DownloadFile(_selectedBase == BaseType.Awtter ?
                        "file:///C:/Users/Kille/Desktop/Awtter_v2.9.42_Unity.unitypackage" :
                        "", "Downloading Base Model", "Downloading in progress...");
                CheckPackages = true;
            }
            GUI.enabled = true;
            GUILayout.EndArea();
        }

        void CreateBox(string text)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(text, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void DownloadFile(string url, string progressTitle, string progressInfo)
        {
            string fileName = Path.GetFileName(url);

            UnityWebRequest www = new UnityWebRequest(url);
            www.downloadHandler = new DownloadHandlerBuffer();
            AsyncOperation request = www.SendWebRequest();

            while (!request.isDone)
            {
                EditorUtility.DisplayProgressBar(progressTitle, progressInfo, www.downloadProgress);
            }

            EditorUtility.ClearProgressBar();
            if (www.error == null)
            {
                File.WriteAllBytes(Path.Combine("Assets", fileName), www.downloadHandler.data);

                AssetDatabase.ImportPackage(Path.Combine("Assets", fileName), false);
                AssetDatabase.DeleteAsset(Path.Combine("Assets", fileName));
            }
            else
            {
                Debug.Log(www.error);
            }
        }

        Rect toolbarRect
        {
            get { return new Rect(20f, 0f, position.width - 40f, 20f); }
        }

        Rect packagesTreeViewRect
        {
            get { return new Rect(20, 30, position.width - 40, 100f); }
        }

        Rect bottomToolbarRect
        {
            get { return new Rect(20f, 135, position.width - 40f, 80f); }
        }
    }

    internal class MyMultiColumnHeader : MultiColumnHeader
    {
        Mode m_Mode;

        public enum Mode
        {
            LargeHeader,
            DefaultHeader,
            MinimumHeaderWithoutSorting
        }

        public MyMultiColumnHeader(MultiColumnHeaderState state)
            : base(state)
        {
            mode = Mode.DefaultHeader;
        }

        public Mode mode
        {
            get
            {
                return m_Mode;
            }
            set
            {
                m_Mode = value;
                switch (m_Mode)
                {
                    case Mode.LargeHeader:
                        canSort = true;
                        height = 37f;
                        break;
                    case Mode.DefaultHeader:
                        canSort = true;
                        height = DefaultGUI.defaultHeight;
                        break;
                    case Mode.MinimumHeaderWithoutSorting:
                        canSort = false;
                        height = DefaultGUI.minimumHeight;
                        break;
                }
            }
        }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            // Default column header gui
            base.ColumnHeaderGUI(column, headerRect, columnIndex);

            // Add additional info for large header
            if (mode == Mode.LargeHeader)
            {
                // Show example overlay stuff on some of the columns
                if (columnIndex > 2)
                {
                    headerRect.xMax -= 3f;
                    var oldAlignment = EditorStyles.largeLabel.alignment;
                    EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
                    GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
                    EditorStyles.largeLabel.alignment = oldAlignment;
                }
            }
        }
    }
}
