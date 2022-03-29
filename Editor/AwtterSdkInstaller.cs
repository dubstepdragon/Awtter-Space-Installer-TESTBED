using System;
using System.IO;

using UnityEditor;

using UnityEngine;
using UnityEngine.Networking;

using AwtterSDK.Editor.Enums;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using System.Collections.Generic;
using AwtterSDK.Editor.Models;
using AwtterSDK.Editor.Interfaces;
using AwtterSDK.Editor.Installations;
using System.Net;
using AwtterSDK.Editor.AssetsTreeView;
using System.Linq;

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
        public static bool LoggedIn;
        public static bool BaseInstalled;
        public static int BaseType;

        public static Texture DefaultTexture;
        public static Queue<string> TexturesToDownload = new Queue<string>();
        public static Dictionary<string, Texture> CachedTextures = new Dictionary<string, Texture>();

        public static bool CheckPackages = true;
        static AwtterSdkInstaller _window;

        [NonSerialized] bool m_Initialized;
        [SerializeField] TreeViewState m_TreeViewState;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        AssetsView m_TreeView;

        Dictionary<string, ICheckInstallStatus> _installationCheckers = new Dictionary<string, ICheckInstallStatus>()
        {
            { "vrcsdk", new VrcSdkInstallation() },
            { "poiyomi", new PoyomiInstallation() },
            { "basemodel", new BaseInstallation() },
        };

        DownloadableContent _downloadableContent = null;
        DownloadableContent _downloadableContentTemplate = new DownloadableContent()
        {
            Models = new List<BaseModel>()
            {
                new BaseModel()
                {
                    Name = "Awtter",
                    ShortName = "awtter",
                    Version = "2.9.42",
                    IconLink = "https://shadedoes3d.com/static/core/img/thumb/Otter.jpg",
                    DownloadLink = "file:///C:/Users/Kille/Desktop/Awtter_v2.9.42_Unity.unitypackage",
                    Dependencies = new string[] { "vrcsdk", "poiyomi" },
                    AvaliableDLC = new List<DLC>()
                    {
                        new DLC()
                        {
                            Name = "Test DLC",
                            DownloadLink = "<DLC AA>",
                            IconLink = "https://upload.wikimedia.org/wikipedia/commons/6/6d/Windows_Settings_app_icon.png",
                            Dependencies = new string [] { "vrcsdk", "poiyomi", "awtter" }
                        }
                    }
                },
                new BaseModel()
                {
                    Name = "Awdeer",
                    ShortName = "awdeer",
                    Version = "2.9.42",
                    IconLink = "https://shadedoes3d.com/static/core/img/thumb/Otter.jpg",
                    DownloadLink = "file:///C:/Users/Kille/Desktop/Awtter_v2.9.42_Unity.unitypackage",
                    Dependencies = new string[] { "vrcsdk", "poiyomi" },
                    AvaliableDLC = new List<DLC>()
                    {
                        new DLC()
                        {
                            Name = "Test DLC",
                            DownloadLink = "<DLC AA>",
                            Dependencies = new string [] { "vrcsdk", "poiyomi", "awdeer" }
                        }
                    }
                }
            },
            Packages = new List<Package>()
            {
                new Package()
                {
                    Name = "VRC SDK",
                    ShortName = "vrcsdk",
                    Version = "2022.02.16.19.13",
                    AutoDetect = true,
                    IconLink = "https://cdn.discordapp.com/attachments/939894161472102462/953747349866434610/o8smnmp6erp21.png",
                    DownloadLink = "https://files.vrchat.cloud/sdk/VRCSDK3-AVATAR-2022.02.16.19.13_Public.unitypackage",
                },
                new Package()
                {
                    Name = "Poiyomi Toon Shader",
                    ShortName = "poiyomi",
                    Version = "7.3.850",
                    IconLink = "https://cdn.discordapp.com/attachments/939894161472102462/953738819566456922/8877018.png",
                    DownloadLink = "https://github.com/poiyomi/PoiyomiToonShader/releases/download/V7.3.050/PoiyomiToon7.3.050.unitypackage"
                }
            }
        };


        [MenuItem("Awtter SDK/Open Installer")]
        static void Init()
        {
            //_window = (AwtterSdkInstaller)EditorWindow.GetWindowWithRect(typeof(AwtterSdkInstaller), new Rect(0, 0, 525, 248), false, "Awtter SDK | Installer");
            _window = (AwtterSdkInstaller)EditorWindow.GetWindow(typeof(AwtterSdkInstaller), false, "Awtter SDK | Installer");
            _window.Show();
            CheckPackages = true;
        }

        internal List<AssetElement> CreateView()
        {
            if (_downloadableContent == null) return new List<AssetElement>();

            List<AssetElement> elements = new List<AssetElement>()
            {
                new AssetElement(string.Empty, string.Empty, -1, 0),
                new AssetElement("Base Models", string.Empty, 0, 1),
            };

            int id = 1;

            for (int x = 0; x < _downloadableContent.Models.Count; x++)
            {
                id++;
                elements.Add(new AssetElement(_downloadableContent.Models[x].Name, _downloadableContent.Models[x].IconLink, 1, id));
                _downloadableContent.Models[x].ViewID = elements.Count - 1;

                id++;
                elements.Add(new AssetElement("DLCS", "", 2, id));
                for (int y = 0; y < _downloadableContent.Models[x].AvaliableDLC.Count; y++)
                {
                    id++;
                    elements.Add(new AssetElement(_downloadableContent.Models[x].AvaliableDLC[y].Name, _downloadableContent.Models[x].AvaliableDLC[y].IconLink, 3, id));
                    _downloadableContent.Models[x].AvaliableDLC[y].ViewID = elements.Count - 1;
                }
            }
            id++;

            elements.Add(new AssetElement("Packages", "", 0, id));
            for (int x = 0; x < _downloadableContent.Packages.Count; x++)
            {
                id++;
                elements.Add(new AssetElement(_downloadableContent.Packages[x].Name, _downloadableContent.Packages[x].IconLink, 1, id));
                _downloadableContent.Packages[x].ViewID = elements.Count - 1;
            }
            return elements;
        }

        public static Texture GetTextureOrDownload(string url)
        {
            if (DefaultTexture == null) DefaultTexture = EditorGUIUtility.FindTexture("Folder Icon");
            if (string.IsNullOrEmpty(url)) return DefaultTexture;

            if (!CachedTextures.ContainsKey(url))
            {
                CachedTextures.Add(url, null);
                AwtterSdkInstaller.TexturesToDownload.Enqueue(url);
            }

            return CachedTextures[url] ?? DefaultTexture;
        }

        void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState = AssetsView.CreateDefaultMultiColumnHeaderState(100);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new MyMultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<AssetElement>(CreateView());

                m_TreeView = new AssetsView(m_TreeViewState, multiColumnHeader, treeModel);
                m_Initialized = true;
            }
        }

        void CheckInstalledPackages()
        {
            _installationCheckers["basemodel"].Check();
            for(int x = 0; x < _downloadableContent.Models.Count; x++)
            {
                bool modelInstalled = _installationCheckers["basemodel"].IsInstalled && _installationCheckers["basemodel"].TypeInstalled == _downloadableContent.Models[x].ShortName;

                if (modelInstalled)
                {
                    BaseInstalled = true;
                    BaseType = x;
                }

                m_TreeView.treeModel.m_Data[_downloadableContent.Models[x].ViewID].Status = modelInstalled ? PackageStatus.Installed : PackageStatus.NotInstalled;
                m_TreeView.treeModel.m_Data[_downloadableContent.Models[x].ViewID].Version = modelInstalled ? _installationCheckers["basemodel"].InstalledVersion ?? "-" : "-";
            }

            foreach (var package in _downloadableContent.Packages)
            {
                if (_installationCheckers.TryGetValue(package.ShortName, out ICheckInstallStatus installStatus))
                {
                    installStatus.Check();
                    m_TreeView.treeModel.m_Data[package.ViewID].Status = installStatus.IsInstalled ? PackageStatus.Installed : PackageStatus.NotInstalled;
                    m_TreeView.treeModel.m_Data[package.ViewID].Version = installStatus.InstalledVersion ?? "-";
                }
            }

            CheckPackages = false;
        }

        string username, password;

        void Login()
        {
            if (!File.Exists("Assets/downloadableContent.json"))
                File.WriteAllText("Assets/downloadableContent.json", JsonUtility.ToJson(_downloadableContentTemplate, true));

            _downloadableContent = JsonUtility.FromJson<DownloadableContent>(File.ReadAllText("Assets/downloadableContent.json"));
            LoggedIn = true;
        }

        void Logout()
        {
            LoggedIn = false;
        }

        void OnGUI()
        {
            if (!LoggedIn)
            {
                GUILayout.Space(150);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical("Account", "window", GUILayout.Height(120), GUILayout.Width(300));
                GUI.enabled = false;
                username = EditorGUILayout.TextField("Email", username);
                password = EditorGUILayout.PasswordField("Password", password);
                GUI.enabled = true;
                if (GUILayout.Button("Login")) Login();
                if (GUILayout.Button("Register")) Application.OpenURL("https://shadedoes3d.com");
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return;
            }

            if (TexturesToDownload.Count != 0)
            {
                var link = TexturesToDownload.Dequeue();
   
                using (WebClient client = new WebClient())
                {
                    byte[] data = client.DownloadData(link);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(data);

                    CachedTextures[link] = tex;
                }
            }
            InitIfNeeded();
            if (CheckPackages) CheckInstalledPackages();
            TopToolBar(toolbarRect);
            m_TreeView.OnGUI(packagesTreeViewRect);
            BottomToolBar(bottomToolbarRect);
        }

        void TopToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Logout")) Logout();
            EditorGUILayout.EndHorizontal();
            CreateBox("Installed assets");
            GUILayout.EndArea();
        }

        void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);
            if (BaseInstalled)
            {
                CreateBox("Install DLCS");
                for(int x = 0; x < _downloadableContent.Models[BaseType].AvaliableDLC.Count; x++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(_downloadableContent.Models[BaseType].AvaliableDLC[x].Name);
                    GUILayout.FlexibleSpace();
                    _downloadableContent.Models[BaseType].AvaliableDLC[x].Install = EditorGUILayout.Toggle(_downloadableContent.Models[BaseType].AvaliableDLC[x].Install);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                CreateBox("Installation options");
                BaseType = EditorGUILayout.Popup("Model base", BaseType, _downloadableContent.Models.Select(x => x.Name).ToArray());
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                CreateBox("Base");
                GUILayout.Label($"Name {_downloadableContent.Models[BaseType].Name}");
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                CreateBox("Avaliable DLCS");
                for (int x = 0; x < _downloadableContent.Models[BaseType].AvaliableDLC.Count; x++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(_downloadableContent.Models[BaseType].AvaliableDLC[x].Name);
                    GUILayout.FlexibleSpace();
                    _downloadableContent.Models[BaseType].AvaliableDLC[x].Install = EditorGUILayout.Toggle(_downloadableContent.Models[BaseType].AvaliableDLC[x].Install);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Install"))
            {
                foreach(var dependency in _downloadableContent.Models[BaseType].Dependencies)
                {
                    if (_installationCheckers.TryGetValue(dependency, out ICheckInstallStatus status) && !status.IsInstalled)
                    {
                        var missingDependency = _downloadableContent.Packages.FirstOrDefault(p => p.ShortName == dependency);
                        if (missingDependency == null)
                        {
                            Debug.LogError($"Not found {dependency} dependency for base {_downloadableContent.Models[BaseType].Name}.");
                        }
                        else
                        {
                            DownloadFile(missingDependency.DownloadLink, $"Downloading file", $"Downloading {dependency}...");
                        }
                    }
                }
                if (!BaseInstalled) DownloadFile(_downloadableContent.Models[BaseType].DownloadLink, "Downloading file", $"Downloading base model {_downloadableContent.Models[BaseType].Name}...");
            }


            GUI.enabled = true;
            GUILayout.EndArea();
        }

        void CreateBox(string text, bool flexible = true)
        {
            EditorGUILayout.BeginHorizontal("box");
            if (flexible) GUILayout.FlexibleSpace();
            GUILayout.Label(text, EditorStyles.boldLabel);
            if (flexible) GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
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
            get { return new Rect(0f, 0f, position.width, 25f); }
        }

        Rect packagesTreeViewRect
        {
            get { return new Rect(0f, 30f, position.width, 140f); }
        }

        Rect bottomToolbarRect
        {
            get { return new Rect(0f, 175f, position.width, position.height - 160f); }
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
