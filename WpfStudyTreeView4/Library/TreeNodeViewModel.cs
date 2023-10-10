using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStudyTreeView4.Library
{
    public class TreeNodeViewModel
    {
        public TreeNodeModel SelectedItem { get; set; }

        public ObservableCollection<TreeNodeModel> Items { get; set; }

        public List<TreeNodeModel> AllNodes { get; set; }

        public TreeNodeViewModel()
        {
            AllNodes = GetSampleTreeNode();

            Items = new ObservableCollection<TreeNodeModel>(AllNodes.Where(x => x.ParentName == null).ToList());

            var parentNodes = AllNodes.Where(x => x.IsParentNode == true);

            foreach (TreeNodeModel node in parentNodes)
            {
                node.UpdateItems();
            }
        }

        public static List<TreeNodeModel> GetSampleTreeNode()
        {
            List<string> wellMembers = new() { "Wells", "R1_W7", "R1_W12345678912", "R1_W123456789111", "R2_W1", "R2_W11", "R2_W03", "R2_W123456789123456789", "R1_W1", "R1_W012" };
            List<string> polygonMembers = new() { "Polygons" };
            List<string> rockMembers = new() { "Rocks", "R4", "R2", "R1", "R3", "R7", "R6", "R5" };
            List<string> wellStrategyMembers = new() { "Well Strategies", "WS6", "WS2", "WS4", "WS1", "WS3", "WS5" };
            List<string> r1Members = new() { "R1", "R1_11", "R1_02", "R1_13" };
            List<string> r2Members = new() { "R2", "R2_21", "R2_4", "R2_1", "R2_15" };
            List<string> r3Members = new() { "R2_1", "R2_1_2", "R2_1_1", "R2_1_4", "R2_1_3" };

            List<TreeNodeModel> result = new();

            CreateMembers(result, wellMembers, Constants.ImagePath.WellImagePath, Constants.ImagePath.WellChildImagePath);
            CreateMembers(result, polygonMembers, Constants.ImagePath.PolygonImagePath, Constants.ImagePath.PolygonImagePath);
            CreateMembers(result, rockMembers, Constants.ImagePath.RockImagePath, Constants.ImagePath.RockChildImagePath);
            CreateMembers(result, wellStrategyMembers, Constants.ImagePath.WellStrategyImagePath, Constants.ImagePath.WellStrategyChildImagePath);
            CreateMembers(result, r1Members, Constants.ImagePath.RockChildImagePath, Constants.ImagePath.RockChild2ImagePath);
            CreateMembers(result, r2Members, Constants.ImagePath.RockChildImagePath, Constants.ImagePath.RockChild2ImagePath);
            CreateMembers(result, r3Members, Constants.ImagePath.RockChild2ImagePath, Constants.ImagePath.RockChild3ImagePath);

            return result;
        }

        private static void CreateMembers(List<TreeNodeModel> treeNodesList, List<string> membersNameArray, string parentImagePath, string childImagePath)
        {
            if (!treeNodesList.Any(x => x.Name == membersNameArray[0]))
            {
                treeNodesList.Add(new TreeNodeModel(membersNameArray[0], parentImagePath, null, treeNodesList, true));
            }
            else
            {
                treeNodesList.First(x => x.Name == membersNameArray[0]).IsParentNode = true;
            }

            if (membersNameArray.Count > 1)
            {
                foreach (string childName in membersNameArray.GetRange(1, membersNameArray.Count - 1))
                {
                    treeNodesList.Add(new TreeNodeModel(childName, childImagePath, membersNameArray[0], treeNodesList, false));
                }

            }
        }
    }
}
