using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStudyTreeView4.Comparer;

namespace WpfStudyTreeView4.Library
{
    public static class CustomOrder
    {
        public static void OrderModel(ObservableCollection<TreeNodeModel> collection)
        {
            List<TreeNodeModel> listOfCollection = collection.OrderBy(x => x.Name, new NaturalStringComparer()).ToList();

            for (int i = 0; i < listOfCollection.Count; i++)
            {
                collection.Move(collection.IndexOf(listOfCollection[i]), i);
            }
        }
    }
}
