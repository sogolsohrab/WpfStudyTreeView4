using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfStudyTreeView4.Library;

namespace WpfStudyTreeView4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TreeNodeViewModel ViewModel = new TreeNodeViewModel();

        public MainWindow()
        {
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
            FillTree();
        }


        #region Private Methods

        private void FillTree()
        {
            SortTree();
            Tree.ItemTemplate = CreateDataTemplate();
            Tree.ItemContainerStyle = CreateStyle();
            Tree.ItemsSource = ViewModel.Items;
            Tree.SelectedItemChanged += OnSelectedItemChanged;
        }

        private void SortTree()
        {
            CustomOrder.OrderModel(ViewModel.Items);

            var parentNodes = ViewModel.AllNodes.Where(x => x.IsParentNode == true);
            foreach (var item in parentNodes)
            {
                CustomOrder.OrderModel(item.Items);
            }
        }

        private static HierarchicalDataTemplate CreateDataTemplate()
        {
            HierarchicalDataTemplate dataTemplate = new()
            {
                ItemsSource = new Binding() { Path = new PropertyPath("Items") }
            };

            FrameworkElementFactory stackPanel = new(typeof(StackPanel))
            {
                Name = "parentStackPanel"
            };
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory image = new(typeof(Image));
            image.SetValue(Image.MarginProperty, new Thickness(2));
            image.SetValue(Image.WidthProperty, 16.0);
            image.SetValue(Image.HeightProperty, 16.0);
            image.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("DisplayedImagePath") });
            stackPanel.AppendChild(image);

            FrameworkElementFactory textBlock = new(typeof(TextBlock));
            textBlock.SetValue(TextBlock.MarginProperty, new Thickness(5));
            textBlock.SetValue(TextBlock.FontSizeProperty, 12.0);
            textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Name") });
            stackPanel.AppendChild(textBlock);

            dataTemplate.VisualTree = stackPanel;

            return dataTemplate;
        }

        private Style CreateStyle()
        {
            var style = new Style { TargetType = typeof(TreeViewItem) };
            var eventSetter = new EventSetter(PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseRightButtonDown));
            style.Setters.Add(eventSetter);

            return style;
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedItem == null)
            {
                return;
            }

            ContextMenu contextMenu = new();
            MenuItem renameMenuItem = CreateMenuItem("Rename", Constants.ImagePath.RenameIconImagePath, RenameMenuItem_Click);
            MenuItem deleteMenuItem = CreateMenuItem("Delete", Constants.ImagePath.DeleteIconImagePath, DeleteMenuItem_Click);

            if (ViewModel.SelectedItem.ParentName == null)
            {
                contextMenu.Items.Add(renameMenuItem);
            }
            else
            {
                contextMenu.Items.Add(renameMenuItem);
                contextMenu.Items.Add(deleteMenuItem);
            }
            ((TreeViewItem)sender).ContextMenu = contextMenu;
        }

        private static MenuItem CreateMenuItem(String header, String imagePath, RoutedEventHandler routedEventHandler)
        {
            MenuItem menuItem = new()
            {
                Header = header,
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Relative))
                }
            };
            menuItem.Click += routedEventHandler;

            return menuItem;
        }

        private void UpdatePanelsVisibility(Border selectedBorder)
        {
            List<Border> bordersList = new() { HomePanel, RenamePanel, DeletePanel };
            foreach (var border in bordersList)
            {
                border.Visibility = Visibility.Collapsed;
            }

            selectedBorder.Visibility = Visibility.Visible;
        }
        #endregion Private Methods


        #region Events

        #region SelectedItemChanged
        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedItem = (TreeNodeModel)e.NewValue;
        }
        #endregion SelectedItemChanged


        #region Delete Events
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanelsVisibility(DeletePanel);
        }

        private void DeleteAbortButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanelsVisibility(HomePanel);
        }

        private void DeleteProceedButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AllNodes.Remove(ViewModel.SelectedItem);
            ViewModel.SelectedItem.GetParent()?.UpdateItems();
            SortTree();
            UpdatePanelsVisibility(HomePanel);
        }
        #endregion Delete Events


        #region Rename Events
        private void RenameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanelsVisibility(RenamePanel);
            renameTextBox.Text = ViewModel.SelectedItem.Name ?? "";
        }

        private void RenameAbortButton_Click(object sender, RoutedEventArgs e)
        {
            errorLabel.Content = string.Empty;
            UpdatePanelsVisibility(HomePanel);
        }

        private void RenameProceedButton_Click(object sender, RoutedEventArgs e)
        {

            bool IsEditingPossible = CheckRenamingPossibility();
            if (IsEditingPossible)
            {
                var similarParentNodes = ViewModel.AllNodes.Where(x => x.ParentName == ViewModel.SelectedItem.Name);
                if (similarParentNodes.Any())
                {
                    foreach (var node in similarParentNodes)
                    {
                        node.ParentName = renameTextBox.Text;
                    }
                }
                ViewModel.SelectedItem.Name = renameTextBox.Text;
                SortTree();
                UpdatePanelsVisibility(HomePanel);
                errorLabel.Content = string.Empty;
            }
            else
            {
                UpdatePanelsVisibility(RenamePanel);
            }
            renameTextBox.Clear();
        }

        private bool CheckRenamingPossibility()
        {
            // Check Duplicate Nodes
            bool IsDuplicatedNode = false;
            IsDuplicatedNode = ViewModel.AllNodes.Any(x => x.Name == renameTextBox.Text);

            // Check Validity
            bool IsRenamingPossible = false;
            if (renameTextBox.Text.Length < 2)
            {
                errorLabel.Content = "** Please choose a name with more than 2 characters!";
            }
            else if (IsDuplicatedNode)
            {

                errorLabel.Content = "** Duplicated name. Please choose a new one!";
            }
            else
            {
                IsRenamingPossible = true;
            }

            return IsRenamingPossible;
        }
        #endregion Rename Events

        #endregion Events


        #region Window style Events
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }
        #endregion Window style Events

    }
}
