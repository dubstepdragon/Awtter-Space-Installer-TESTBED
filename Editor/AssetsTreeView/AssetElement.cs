using AwtterSDK.Editor.Enums;
using System;
using UnityEditor.TreeViewExamples;

namespace AwtterSDK.Editor.AssetsTreeView
{
	[Serializable]
	internal class AssetElement : TreeElement
	{
		public string Name { get; set; }
		public PackageStatus Status { get; set; } = PackageStatus.None;
		public string Version { get; set; } = "-";
		public string Icon { get; set; }

		public AssetElement(string name, string icon, int depth, int id) : base(name, depth, id)
		{
			Name = name;
			Icon = icon;
		}
	}
}
