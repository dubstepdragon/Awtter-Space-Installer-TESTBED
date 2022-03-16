using AwtterSDK.Editor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace AwtterSDK.Editor.PackagesTreeView
{
	internal class PackagesView : TreeViewWithTreeModel<PackageElement>
	{
		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;
		public bool showControls = true;

		static Texture2D[] s_TestIcons =
		{
			EditorGUIUtility.FindTexture ("Folder Icon"),
		};

		enum MyColumns
		{
			Icon,
			Name,
			Status,
			Version
		}

		public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
		{
			if (root == null)
				throw new NullReferenceException("root");
			if (result == null)
				throw new NullReferenceException("result");

			result.Clear();

			if (root.children == null)
				return;

			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			for (int i = root.children.Count - 1; i >= 0; i--)
				stack.Push(root.children[i]);

			while (stack.Count > 0)
			{
				TreeViewItem current = stack.Pop();
				result.Add(current);

				if (current.hasChildren && current.children[0] != null)
				{
					for (int i = current.children.Count - 1; i >= 0; i--)
					{
						stack.Push(current.children[i]);
					}
				}
			}
		}

		public PackagesView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<PackageElement> model) : base(state, multicolumnHeader, model)
		{
			// Custom setup
			rowHeight = kRowHeights;
			columnIndexForTreeFoldouts = 2;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;

			Reload();
		}


        protected override bool CanStartDrag(CanStartDragArgs args)
        {
			return false;
        }

        // Note we We only build the visible rows, only the backend has the full tree information. 
        // The treeview only creates info for the row list.
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows(root);
			return rows;
		}

		int GetIcon1Index(TreeViewItem<PackageElement> item)
		{
			return 0;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = (TreeViewItem<PackageElement>)args.item;

			for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
			{
				CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
			}
		}

		void CellGUI(Rect cellRect, TreeViewItem<PackageElement> item, MyColumns column, ref RowGUIArgs args)
		{
			CenterRectUsingSingleLineHeight(ref cellRect);
			switch (column)
			{
				case MyColumns.Icon:
					{
						GUI.DrawTexture(cellRect, s_TestIcons[GetIcon1Index(item)], ScaleMode.ScaleToFit);
					}
					break;
				case MyColumns.Name:
					{
						args.rowRect = cellRect;
						args.label = item.data.Name;
						base.RowGUI(args);
					}
					break;
				case MyColumns.Status:
					{
						args.rowRect = cellRect;
						var defColor = GUI.color;
						Rect statusRect = cellRect;

						switch (item.data.Status)
                        {
							case PackageStatus.NotInstalled:
								statusRect.x += GetContentIndent(item);
								statusRect.width = 18f;
								if (statusRect.xMax < cellRect.xMax)
                                {
									GUI.color = Color.red;
									EditorGUI.LabelField(statusRect, "❌");
									GUI.color = defColor;
								}
								args.rowRect = cellRect;
								args.label = "Not Installed";
								base.RowGUI(args);
								break;
							case PackageStatus.Installed:
								statusRect.x += GetContentIndent(item);
								statusRect.width = 18f;
								if (statusRect.xMax < cellRect.xMax)
								{
									GUI.color = Color.green;
									EditorGUI.LabelField(statusRect, "✔️");
									GUI.color = defColor;
								}
								args.rowRect = cellRect;
								args.label = "Installed";
								base.RowGUI(args);
								break;
                        }
					}
					break;
				case MyColumns.Version:
					{
						args.rowRect = cellRect;
						args.label = item.data.Version;
						base.RowGUI(args);
					}
					break;
			}
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
		{
			var columns = new[]
			{
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel"), "Icon"),
					contextMenuText = "Icon",
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Right,
					width = 30,
					minWidth = 30,
					maxWidth = 60,
					autoResize = false,
					allowToggleVisibility = true,
					canSort = false,
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Name"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 150,
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = false,
					canSort = false,
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Status"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 150,
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = false,
					canSort = false,
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Version"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 150,
					minWidth = 60,
					autoResize = false,
					allowToggleVisibility = false,
					canSort = false,
				},
			};

			var state = new MultiColumnHeaderState(columns);
			return state;
		}
	}
}
