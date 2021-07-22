using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CPRISwitchSimulator.ViewModels;

namespace CPRISwitchSimulator
{
    public partial class MainWindow : Window
    {
        private enum ActionType
        {
            NONE,
            ADD_EQUIPMENT,
            ADD_CONNECTION
        }
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            viewModel = new MainViewModel();
            ResetValues();

            DataContext = viewModel;
            Console.WriteLine(CanvasMain.Name);
        }
        private void ResetValues()
        {
            pickedVM = null;
            draggingPickedElement = false;
            currentAction = ActionType.NONE;
            StatusBarTextBlock.Text = "Status";
        }
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (currentAction == ActionType.ADD_EQUIPMENT)
                        addElementTypeStr = null;
                    break;
                default:
                    break;
            }

            ResetValues();
        }
        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            addElementTypeStr = (string)button.Tag;
            currentAction = ActionType.ADD_EQUIPMENT;
            StatusBarTextBlock.Text = "Place " + addElementTypeStr + " element on the work surface by clicking on it.";
        }
        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            currentAction = ActionType.ADD_CONNECTION;
            addElementTypeStr = null;

            StatusBarTextBlock.Text = "Add connection between 2 CPRI elements by clicking on them.";
        }
        private void CanvasMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas)
            {
                if (currentAction == ActionType.ADD_EQUIPMENT)
                {
                    Point mousePoint = Mouse.GetPosition(CanvasMain);

                    if (addElementTypeStr == (CloudButton.Tag as string))
                        AddCloud(mousePoint);
                    else if (addElementTypeStr == (CellButton.Tag as string))
                        AddCell(mousePoint);
                    else
                        AddElement(mousePoint);

                    ResetValues();
                }

            }
        }
        private void CanvasMain_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (pickedVM == null || !draggingPickedElement)
                return;

            Point position = e.GetPosition(sender as IInputElement);
            double newY = position.Y - dragOffset.Y;
            double newX = position.X - dragOffset.X;

            if (newY > -elementImgDimensions / 5 && newY < (CanvasMain.ActualHeight - elementImgDimensions))
                pickedVM.PosY = newY;

            if(newX > -elementImgDimensions / 5 && newX < (CanvasMain.ActualWidth - elementImgDimensions))
                pickedVM.PosX = newX;
        }
        private void CanvasMain_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggingPickedElement)
                pickedVM = null;

            CanvasMain.ReleaseMouseCapture();

            draggingPickedElement = false;
        }
        private void ElementConnection_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction != ActionType.NONE)
            {
                ResetValues();
                return;
            }

            ElementConnectionMainViewModel connectionVM = (sender as FrameworkElement).DataContext as ElementConnectionMainViewModel;

            if (e.ClickCount == 2)
            {
                ElementConnectionConfigurator connectionConfigurator = new ElementConnectionConfigurator(connectionVM);
                connectionConfigurator.ShowDialog();

                if (connectionConfigurator.ConfiguratorResult == ElementConnectionConfigurator.Result.DELETE)
                    viewModel.Delete(connectionVM);
            }
        }
        private void CloudConnection_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction != ActionType.NONE)
            {
                ResetValues();
                return;
            }

            if (e.ClickCount == 2)
            {
                CloudConnectionViewModel connectionVM = (sender as FrameworkElement).DataContext as CloudConnectionViewModel;
                CloudConnectionConfigurator connectionConfigurator = new CloudConnectionConfigurator(connectionVM);
                connectionConfigurator.ShowDialog();

                if (connectionConfigurator.ConfiguratorResult == CloudConnectionConfigurator.Result.DELETE)
                    viewModel.Delete(connectionVM);
            }
        }
        private void CellConnection_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction != ActionType.NONE)
            {
                ResetValues();
                return;
            }

            if (e.ClickCount == 2)
            {
                CellConnectionViewModel connectionVM = (sender as FrameworkElement).DataContext as CellConnectionViewModel;
                CellConnectionConfigurator connectionConfigurator = new CellConnectionConfigurator(connectionVM);
                connectionConfigurator.ShowDialog();

                if (connectionConfigurator.ConfiguratorResult == CellConnectionConfigurator.Result.DELETE)
                    viewModel.Delete(connectionVM);
            }
        }
        private void ElementImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction == ActionType.ADD_EQUIPMENT)
            {
                ResetValues();
                return;
            }

            ElementMainViewModel elementVM = (sender as FrameworkElement).DataContext as ElementMainViewModel;

            if (e.ClickCount == 1)
            {
                if (currentAction == ActionType.NONE)
                {
                    pickedVM = elementVM;
                    dragOffset = e.GetPosition(CanvasMain);
                    dragOffset.Y -= elementVM.PosY;
                    dragOffset.X -= elementVM.PosX;
                    _ = CanvasMain.CaptureMouse();
                    draggingPickedElement = true;
                }
                else if (currentAction == ActionType.ADD_CONNECTION)
                {
                    ElementVM_AddConnection_HandleSingleClick(elementVM);
                }
            }
            else if (e.ClickCount == 2)
            {
                if (currentAction == ActionType.ADD_CONNECTION)
                    return;

                ElementConfigurator cpriElementConfigurator = new ElementConfigurator(elementVM.Element);
                cpriElementConfigurator.ShowDialog();

                if (cpriElementConfigurator.ConfiguratorResult == ElementConfigurator.Result.DELETE)
                    viewModel.Delete(elementVM);
            }
        }
        private void ElementVM_AddConnection_HandleSingleClick(ElementMainViewModel elementVM)
        {
            if (pickedVM == null || pickedVM == elementVM)
            {
                pickedVM = elementVM;
                return;
            }

            if (pickedVM is CloudViewModel)
            {
                if (elementVM.Element.Type == TopologyModel.ElementType.REC)
                    ConfigureCloudConnection(pickedVM as CloudViewModel, elementVM);
                else
                    _ = MessageBox.Show("Core network can only be attached to REC");
            }
            if (pickedVM is CellMainViewModel)
            {
                if (elementVM.Element.Type == TopologyModel.ElementType.RE)
                    ConfigureCellConnection(pickedVM as CellMainViewModel, elementVM);
                else
                    _ = MessageBox.Show("Cell can only be attached to RE");
            }
            else if (pickedVM is ElementMainViewModel)
            {
                ConfigureElementConnection(pickedVM as ElementMainViewModel, elementVM);
            }

            ResetValues();
        }
        private void CloudImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction == ActionType.ADD_EQUIPMENT)
            {
                ResetValues();
                return;
            }

            CloudViewModel cloudVM = (sender as FrameworkElement).DataContext as CloudViewModel;

            if (e.ClickCount == 1)
            {
                if (currentAction == ActionType.NONE)
                {
                    pickedVM = cloudVM;
                    dragOffset = e.GetPosition(CanvasMain);
                    dragOffset.Y -= cloudVM.PosY;
                    dragOffset.X -= cloudVM.PosX;
                    _ = CanvasMain.CaptureMouse();
                    draggingPickedElement = true;
                }
                else if (currentAction == ActionType.ADD_CONNECTION)
                {
                    if (pickedVM == null || pickedVM == cloudVM)
                    {
                        pickedVM = cloudVM;
                        return;
                    }

                    if (pickedVM is ElementMainViewModel
                        && (pickedVM as ElementMainViewModel).Element.Type == TopologyModel.ElementType.REC)
                        ConfigureCloudConnection(cloudVM, pickedVM as ElementMainViewModel);
                    else
                        _ = MessageBox.Show("Core network can only be attached to REC");

                    ResetValues();
                }

            }
            else if (e.ClickCount == 2)
            {
                if (currentAction == ActionType.ADD_CONNECTION)
                    return;

                CloudConfigurator cloudConfigurator = new CloudConfigurator(cloudVM);
                cloudConfigurator.ShowDialog();

                if (cloudConfigurator.ConfiguratorResult == CloudConfigurator.Result.DELETE)
                    viewModel.Delete(cloudVM);
            }
        }
        private void CellImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction == ActionType.ADD_EQUIPMENT)
            {
                ResetValues();
                return;
            }

            CellMainViewModel cellVM = (sender as FrameworkElement).DataContext as CellMainViewModel;

            if (e.ClickCount == 1)
            {
                if (currentAction == ActionType.NONE)
                {
                    pickedVM = cellVM;
                    dragOffset = e.GetPosition(CanvasMain);
                    dragOffset.Y -= cellVM.PosY;
                    dragOffset.X -= cellVM.PosX;
                    _ = CanvasMain.CaptureMouse();
                    draggingPickedElement = true;
                }
                else if (currentAction == ActionType.ADD_CONNECTION)
                {
                    if (pickedVM == null || pickedVM == cellVM)
                    {
                        pickedVM = cellVM;
                        return;
                    }

                    if (pickedVM is ElementMainViewModel
                        && (pickedVM as ElementMainViewModel).Element.Type == TopologyModel.ElementType.RE)
                        ConfigureCellConnection(cellVM, pickedVM as ElementMainViewModel);
                    else
                        _ = MessageBox.Show("Cell can only be attached to RE");

                    ResetValues();
                }

            }
            else if (e.ClickCount == 2)
            {
                if (currentAction == ActionType.ADD_CONNECTION)
                    return;

                CellConfigurator cellConfigurator = new CellConfigurator(cellVM);
                cellConfigurator.ShowDialog();

                if (cellConfigurator.ConfiguratorResult == CellConfigurator.Result.DELETE)
                    viewModel.Delete(cellVM);
            }
        }
        private void ConfigureElementConnection(ElementMainViewModel elementVM1, ElementMainViewModel elementVM2)
        {
            ElementConnectionMainViewModel connection = viewModel.GetConnection(elementVM1, elementVM2); ;

            if (connection == null)
                connection = viewModel.CreateConnection(elementVM1, elementVM2, elementImgDimensions / 2);

            ElementConnectionConfigurator connectionConfigurator = new ElementConnectionConfigurator(connection);
            connectionConfigurator.ShowDialog();

            if (connectionConfigurator.ConfiguratorResult != ElementConnectionConfigurator.Result.OK)
                viewModel.Delete(connection);
        }
        private void ConfigureCloudConnection(CloudViewModel cloudVM, ElementMainViewModel elementVM)
        {
            CloudConnectionViewModel connection = viewModel.GetConnection(cloudVM, elementVM); ;

            if (connection == null)
                connection = viewModel.CreateConnection(cloudVM, elementVM, cloudImgDimensions / 2, elementImgDimensions / 2);

            CloudConnectionConfigurator connectionConfigurator = new CloudConnectionConfigurator(connection);
            connectionConfigurator.ShowDialog();

            if (connectionConfigurator.ConfiguratorResult != CloudConnectionConfigurator.Result.OK)
                viewModel.Delete(connection);
        }
        private void ConfigureCellConnection(CellMainViewModel cellVM, ElementMainViewModel elementVM)
        {
            CellConnectionViewModel connection = viewModel.GetConnection(cellVM, elementVM);

            if (connection == null)
            {
                try
                {
                    connection = viewModel.CreateConnection(cellVM, elementVM, cellImgDimensions / 2, elementImgDimensions / 2);
                }
                catch(Exception e)
                {
                    _ = MessageBox.Show(e.Message);
                    return;
                }
            }

            CellConnectionConfigurator connectionConfigurator = new CellConnectionConfigurator(connection);
            _ = connectionConfigurator.ShowDialog();

            if (connectionConfigurator.ConfiguratorResult != CellConnectionConfigurator.Result.OK)
                viewModel.Delete(connection);
        }
        private void AddElement(Point mousePoint)
        {
            double posY = mousePoint.Y - (elementImgDimensions / 2);
            double posX = mousePoint.X - (elementImgDimensions / 2);
            ElementMainViewModel elementVM = viewModel.CreateElement(addElementTypeStr, posX, posY);
            ElementConfigurator cpriElementConfigurator = new ElementConfigurator(elementVM.Element);

            cpriElementConfigurator.ShowDialog();

            if (cpriElementConfigurator.ConfiguratorResult != ElementConfigurator.Result.OK)
            {
                viewModel.Delete(elementVM);
                return;
            }
        }
        private void AddCloud(Point mousePoint)
        {
            double posY = mousePoint.Y - (cloudImgDimensions / 2);
            double posX = mousePoint.X - (cloudImgDimensions / 2);
            CloudViewModel cloudVM = viewModel.CreateCloud(posX, posY);
            CloudConfigurator cloudConfigurator = new CloudConfigurator(cloudVM);

            cloudConfigurator.ShowDialog();

            if (cloudConfigurator.ConfiguratorResult != CloudConfigurator.Result.OK)
            {
                viewModel.Delete(cloudVM);
                return;
            }
        }
        private void AddCell(Point mousePoint)
        {
            double posY = mousePoint.Y - (cellImgDimensions / 2);
            double posX = mousePoint.X - (cellImgDimensions / 2);
            CellMainViewModel cellVM = viewModel.CreateCell(posX, posY);
            CellConfigurator cellConfigurator = new CellConfigurator(cellVM);

            cellConfigurator.ShowDialog();

            if (cellConfigurator.ConfiguratorResult != CellConfigurator.Result.OK)
            {
                viewModel.Delete(cellVM);
                return;
            }
        }
        
        private ActionType currentAction;
        private readonly MainViewModel viewModel;
        private string addElementTypeStr;
        private PositionedComponent pickedVM;
        private bool draggingPickedElement;
        private Point dragOffset;
        private readonly double elementImgDimensions = 60;
        private readonly double cellImgDimensions = 50;
        private readonly double cloudImgDimensions = 80;
    }
}
