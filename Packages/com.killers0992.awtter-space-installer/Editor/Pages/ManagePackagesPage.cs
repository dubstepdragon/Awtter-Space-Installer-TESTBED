using AwtterSDK.Editor.Interfaces;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AwtterSDK.Editor.Pages
{
    public class ManagePackagesPage : IPage
    {
        private Vector2 _packagesScroll = Vector2.zero;
        private AwtterSpaceInstaller _main;
        private GUIStyle CustomLabel;

        public void Load(AwtterSpaceInstaller main)
        {
            _main = main;
            CustomLabel = new GUIStyle(GUI.skin.label);
            CustomLabel.richText = true;
            CustomLabel.alignment = TextAnchor.MiddleCenter;
        }

        public void DrawGUI(Rect pos)
        {
            EditorGUILayout.Space(10);
            _packagesScroll = GUILayout.BeginScrollView(_packagesScroll, false, true, GUILayout.Height(281));
            Utils.CreateBox("Base");
            GUILayout.BeginHorizontal();
            GUILayout.Box(TextureCache.GetTextureOrDownload(AwtterSpaceInstaller.CurrentBase.Icon), GUILayout.Height(32), GUILayout.Width(32));
            GUILayout.Label(AwtterSpaceInstaller.CurrentBase.Name, CustomLabel, GUILayout.Height(32));
            GUILayout.EndHorizontal();
            GUI.color = AwtterSpaceInstaller.CurrentBase.IsOutdated ? Color.yellow : Color.green;
            GUI.enabled = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Version {(AwtterSpaceInstaller.CurrentBase.IsOutdated ? $"{AwtterSpaceInstaller.CurrentBase.Version} > {AwtterSpaceInstaller.CurrentBase.Version}" : AwtterSpaceInstaller.CurrentBase.InstalledVersion)}");
            GUILayout.FlexibleSpace();
            GUILayout.Button(AwtterSpaceInstaller.CurrentBase.IsOutdated ? $"🗱 Update" : $"✔ Installed", _main.Shared.WindowCustomButton3, GUILayout.Height(26), GUILayout.Width(150));
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUI.color = Color.white;
            GUILayout.Space(30);

            if (AwtterSpaceInstaller.AvaliableDlcs.Count != 0)
                Utils.CreateBox("DLCS");
            foreach (var dlc in AwtterSpaceInstaller.AvaliableDlcs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(TextureCache.GetTextureOrDownload(dlc.Icon), GUILayout.Height(32), GUILayout.Width(32));
                GUILayout.Label(dlc.Name, CustomLabel, GUILayout.Height(32));
                GUILayout.EndHorizontal();
                GUI.color = dlc.Install ? Color.yellow : dlc.IsInstalled ? Color.green : dlc.IsOutdated ? Color.yellow : Color.cyan;
                GUI.enabled = !dlc.IsInstalled;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Version {(dlc.IsInstalled ? dlc.IsOutdated ? $"{dlc.Version} > {dlc.Version}" : dlc.InstalledVersion : dlc.Version)}");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(dlc.IsInstalled ? $"✔ Installed" : dlc.IsOutdated ? $"🗱 Update" : $"⇩ Install", _main.Shared.WindowCustomButton3, GUILayout.Height(26), GUILayout.Width(150)))
                {
                    dlc.Install = !dlc.Install;
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                GUI.color = Color.white;
                GUILayout.Space(30);
            }

            if (AwtterSpaceInstaller.AvaliableTools.Count != 0)
                Utils.CreateBox("Tools");
            foreach (var tool in AwtterSpaceInstaller.AvaliableTools)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(TextureCache.GetTextureOrDownload(tool.Icon), GUILayout.Height(32), GUILayout.Width(32));
                GUILayout.Label(tool.Name, CustomLabel, GUILayout.Height(32));
                GUILayout.EndHorizontal();
                GUI.color = tool.Install ? Color.yellow : tool.IsInstalled ? Color.green : tool.IsOutdated ? Color.yellow : Color.cyan;
                GUI.enabled = !tool.IsInstalled;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Version {(tool.IsInstalled ? tool.IsOutdated ? $"{tool.Version} > {tool.Version}" : tool.InstalledVersion : tool.Version)}");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(tool.IsInstalled ? $"✔ Installed" : tool.IsOutdated ? $"🗱 Update" : $"⇩ Install", _main.Shared.WindowCustomButton3, GUILayout.Height(26), GUILayout.Width(150)))
                {
                    tool.Install = !tool.Install;
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                GUI.color = Color.white;
                GUILayout.Space(30);
            }
            GUILayout.EndScrollView();

            GUI.enabled = AwtterSpaceInstaller.AvaliableDlcs.Any(x => x.Install) || AwtterSpaceInstaller.AvaliableTools.Any(x => x.Install);
            GUI.color = (AwtterSpaceInstaller.AvaliableDlcs.Any(x => x.Install) || AwtterSpaceInstaller.AvaliableTools.Any(x => x.Install)) ? Color.green : Color.gray;
            if (GUILayout.Button("▶   Run SDK Installer", _main.Shared.WindowCustomButton3, GUILayout.Height(27)))
                AwtterSpaceInstaller.IsInstalling = true;
            GUI.color = Color.white;
            GUI.enabled = true;
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button($"Show scenes"))
            {
                AwtterSpaceInstaller.ViewManagePackages = !AwtterSpaceInstaller.ViewManagePackages;
            }
            EditorGUILayout.EndVertical();
        }

        public void Reset()
        {
            _packagesScroll = Vector2.zero;
        }
    }
}
