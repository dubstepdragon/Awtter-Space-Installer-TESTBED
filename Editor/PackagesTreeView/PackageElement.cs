using AwtterSDK.Editor.Enums;
using System;
using UnityEditor.TreeViewExamples;

namespace AwtterSDK.Editor.PackagesTreeView
{
	[Serializable]
	internal class PackageElement : TreeElement
	{
		public string Name { get; set; }
		public PackageStatus Status { get; set; } = PackageStatus.None;
		public string Version { get; set; } = "-";

		public bool IsDraggable { get; set; } = false;

		public PackageElement(string name, PackageStatus status, string version, int depth, int id) : base(name, depth, id)
		{
			Name = name;
			Status = status;
			Version = version;
		}
	}
}
