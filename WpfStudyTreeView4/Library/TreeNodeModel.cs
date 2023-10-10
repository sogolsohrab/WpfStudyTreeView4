using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStudyTreeView4.Properties;

namespace WpfStudyTreeView4.Library
{
    public class TreeNodeModel : PropertyChangedBase
    {
        private string name;
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string DisplayedImagePath { get; set; }

        public string ParentName { get; set; }

        public bool IsParentNode { get; set; }

        public ObservableCollection<TreeNodeModel> Items { get; set; }

        public List<TreeNodeModel> AllNodes;

        public TreeNodeModel(string name, string imagePath, string parentName, List<TreeNodeModel> allNodes, bool isParentNode)
        {
            Name = name;
            ParentName = parentName;
            DisplayedImagePath = imagePath;
            IsParentNode = isParentNode;
            AllNodes = allNodes;
            Items = new ObservableCollection<TreeNodeModel>();
        }

        public void UpdateItems()
        {
            Items.Clear();

            foreach (TreeNodeModel node in AllNodes)
            {
                if (node.ParentName == Name)
                {
                    Items.Add(node);
                }
            }
        }

        public TreeNodeModel? GetParent()
        {
            return AllNodes.Find(node => node.Name == ParentName);
        }
    }
}
