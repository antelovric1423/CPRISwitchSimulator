using CPRISwitchSimulator.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CPRISwitchSimulator
{
    public class MainViewModel : NotifierBase
    {
        public MainViewModel()
        {
            EquipmentNameTypeMap = new (string, TopologyModel.ElementType)[]
          {
            ("REC" , TopologyModel.ElementType.REC),
            ("RE" , TopologyModel.ElementType.RE),
            ("Switch" , TopologyModel.ElementType.SWITCH)
          };
            ElementViewModels = new ObservableCollection<ElementMainViewModel>();
            CloudViewModels = new ObservableCollection<CloudViewModel>();
            CellViewModels = new ObservableCollection<CellMainViewModel>();
            ElementConnectionViewModels = new ObservableCollection<ElementConnectionMainViewModel>();
            CloudConnectionViewModels = new ObservableCollection<CloudConnectionViewModel>();
            CellConnectionViewModels = new ObservableCollection<CellConnectionViewModel>();
        }
        private TopologyModel.ElementType GetElementType(string elementTypeStr)
        {
            foreach (var it in EquipmentNameTypeMap)
            {
                if (it.Item1 == elementTypeStr)
                    return it.Item2;
            }

            return TopologyModel.ElementType.NOT_SET;
        }
        public ElementMainViewModel CreateElement(string elementTypeStr, double posX, double posY)
        {
            TopologyModel.ElementType elementType = GetElementType(elementTypeStr);

            if (elementType == TopologyModel.ElementType.NOT_SET)
                return null;

            ElementMainViewModel elementVM = new ElementMainViewModel(TopologyModel.Instance.CreateElement(elementType), posX, posY);

            ElementViewModels.Add(elementVM);

            return elementVM;
        }
        public CloudViewModel CreateCloud(double posX, double posY)
        {
            CloudViewModel cloudVM = new CloudViewModel(posX, posY);

            CloudViewModels.Add(cloudVM);

            return cloudVM;
        }
        public CellMainViewModel CreateCell(double posX, double posY)
        {
            CellMainViewModel cellVM = new CellMainViewModel(TopologyModel.Instance.CreateCell(), posX, posY);

            CellViewModels.Add(cellVM);

            return cellVM;
        }
        public ElementConnectionMainViewModel CreateConnection(ElementMainViewModel element1, ElementMainViewModel element2, double posCorrection)
        {
            ElementConnectionMainViewModel connection = new ElementConnectionMainViewModel(element1, element2, posCorrection);
            ElementConnectionViewModels.Add(connection);
            return connection;
        }
        public CloudConnectionViewModel CreateConnection(CloudViewModel cloudVM, ElementMainViewModel elementVM, double cloudPosCorrection, double elementPosCorrection)
        {
            CloudConnectionViewModel connection = new CloudConnectionViewModel(cloudVM, elementVM, cloudPosCorrection, elementPosCorrection);
            CloudConnectionViewModels.Add(connection);
            return connection;
        }
        public CellConnectionViewModel CreateConnection(CellMainViewModel cellVM, ElementMainViewModel elementVM, double cellPosCorrection, double elementPosCorrection)
        {
            foreach(CellConnectionViewModel cellConnectionVM in CellConnectionViewModels)
            {
                if(cellConnectionVM.CellVM == cellVM)
                    throw new Exception("Cell is already attached to different element!");
            }

            TopologyModel.AttachCell(cellVM.Cell, elementVM.Element);
            CellConnectionViewModel connection = new CellConnectionViewModel(cellVM, elementVM, cellPosCorrection, elementPosCorrection);
            CellConnectionViewModels.Add(connection);
            return connection;
        }
        public ElementConnectionMainViewModel GetConnection(ElementMainViewModel elementVM1, ElementMainViewModel elementVM2)
        {
            foreach (var it in ElementConnectionViewModels)
                if ((it.Element1 == elementVM1 && it.Element2 == elementVM2)
                    || (it.Element1 == elementVM2 && it.Element2 == elementVM1))
                    return it;

            return null;
        }
        public CloudConnectionViewModel GetConnection(CloudViewModel cloudVM, ElementMainViewModel elementVM)
        {
            foreach (var it in CloudConnectionViewModels)
                if (it.CloudVM == cloudVM && it.ElementVM == elementVM)
                    return it;

            return null;
        }
        public CellConnectionViewModel GetConnection(CellMainViewModel cellVM, ElementMainViewModel elementVM)
        {
            foreach (var it in CellConnectionViewModels)
                if (it.CellVM == cellVM && it.ElementVM == elementVM)
                    return it;

            return null;
        }
        public void Delete(ElementMainViewModel VM)
        {
            RemoveConnections(VM);
            TopologyModel.Instance.Delete(VM.Element);
            _ = ElementViewModels.Remove(VM);
        }
        public void Delete(CloudViewModel VM)
        {
            RemoveConnections(VM);
            _ = CloudViewModels.Remove(VM);
        }
        public void Delete(CellMainViewModel VM)
        {
            RemoveConnections(VM);
            _ = CellViewModels.Remove(VM);
            TopologyModel.Instance.Delete(VM.Cell);
        }
        public void Delete(ElementConnectionMainViewModel VM)
        {
            TopologyModel.RemoveLinks(VM.Element1.Element.GetLinksTowardElement(VM.Element2.Element));
            ElementConnectionViewModels.Remove(VM);
        }
        public void Delete(CloudConnectionViewModel VM)
        {
            CloudConnectionViewModels.Remove(VM);
        }
        public void Delete(CellConnectionViewModel VM)
        {
            TopologyModel.DetachCell(VM.CellVM.Cell);
            CellConnectionViewModels.Remove(VM);
        }
        private void RemoveConnections(ElementMainViewModel VM)
        {
            var ElementConnectionsToRemove = ElementConnectionViewModels.Where(x => x.Element1 == VM || x.Element2 == VM).ToList();
            var CloudConnectionsToRemove = CloudConnectionViewModels.Where(x => x.ElementVM == VM).ToList();
            var CellConnectionsToRemove = CellConnectionViewModels.Where(x => x.ElementVM == VM).ToList();

            foreach (var it in ElementConnectionsToRemove)
                Delete(it);
            foreach (var it in CloudConnectionsToRemove)
                Delete(it);
            foreach (var it in CellConnectionsToRemove)
                Delete(it);
        }
        private void RemoveConnections(CloudViewModel VM)
        {
            var CloudConnectionsToRemove = CloudConnectionViewModels.Where(x => x.CloudVM == VM).ToList();

            foreach (var it in CloudConnectionsToRemove)
                Delete(it);
        }
        private void RemoveConnections(CellMainViewModel VM)
        {
            var CellConnectionsToRemove = CellConnectionViewModels.Where(x => x.CellVM == VM).ToList();

            foreach (var it in CellConnectionsToRemove)
                Delete(it);
        }
        private (string, TopologyModel.ElementType)[] EquipmentNameTypeMap { get; set; }
        public ObservableCollection<ElementMainViewModel> ElementViewModels { get; set; }
        public ObservableCollection<CellMainViewModel> CellViewModels { get; set; }
        public ObservableCollection<CloudViewModel> CloudViewModels { get; set; }
        public ObservableCollection<ElementConnectionMainViewModel> ElementConnectionViewModels { get; set; }
        public ObservableCollection<CloudConnectionViewModel> CloudConnectionViewModels { get; set; }
        public ObservableCollection<CellConnectionViewModel> CellConnectionViewModels { get; set; }
    }
}
