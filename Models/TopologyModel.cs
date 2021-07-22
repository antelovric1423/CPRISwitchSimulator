using System;
using System.Collections.Generic;
using System.Linq;
using CPRISwitchSimulator.Helpers;
using System.ComponentModel;

namespace CPRISwitchSimulator
{
    public partial class TopologyModel : NotifierBase
    {
        private static readonly uint FirstKeyToGenerate = 1;
        public static TopologyModel Instance { get; } = new TopologyModel();
        private TopologyModel()
        {
            Elements = new List<Element>();
            Cells = new List<Cell>();
        }
        public static List<PortType> GetPortTypesPerElementType(ElementType elementType)
        {
            List<PortType> types = new List<PortType>();

            if (elementType == ElementType.NOT_SET)
                throw new ArgumentOutOfRangeException();

            types.Add(PortType.CONNECTOR_PORT);

            if (elementType == ElementType.REC)
                types.Add(PortType.PROCESSING_PORT);

            return types;

        }
        public static void AddPort(Element element, string name, PortType type, LineRate lineRate)
        {
            element.AddPort(type, name, lineRate);

            if (type == PortType.PROCESSING_PORT)
                ResourceManager.HandleCapacityCreated();
        }
        public static void RemovePort(Port port)
        {
            Capacity capacity = null;
            List<uint> allocRefs;
            List<Cell> cells = new List<Cell>();

            if (port is ProcessingPort processingPort)
                capacity = processingPort.Capacity;
            else if ((port as ConnectorPort).Link != null)
                throw new Exception("Link to port must be removed before port can be deleted");

            if (capacity != null)
            {
                allocRefs = capacity.GetAllocationRefs();
                cells = Instance.Cells.Where(x => allocRefs.Contains(x.Id)).ToList();
                foreach (Cell cell in cells)
                    ResourceManager.DeallocateCellResources(cell);
            }

            port.Parent.RemovePort(port);

            foreach (Cell cell in cells)
                ResourceManager.AllocateCellResources(cell);
        }
        public static void CreateLink(ConnectorPort port1, ConnectorPort port2)
        {
            Link link = new Link(port1, port2);
            port1.Link = link;
            port2.Link = link;

            ResourceManager.HandleCapacityCreated();
        }
        public static void RemoveLink(Link link)
        {
            Console.WriteLine("Remove link between " + link.Port1.Name + " and " + link.Port2.Name);
            RemoveLinks(new List<Link> { link });
        }
        public static void RemoveLinks(List<Link> links)
        {
            List<uint> allocRefs = new List<uint>();

            foreach (Link link in links)
                allocRefs.AddRange(link.Capacity.GetAllocationRefs());

            List<Cell> cells = Instance.Cells.Where(x => allocRefs.Contains(x.Id)).ToList();

            foreach (Cell cell in cells)
                ResourceManager.DeallocateCellResources(cell);

            foreach (Link link in links)
                RemoveLinkRefs(link);

            foreach (Cell cell in cells)
                ResourceManager.AllocateCellResources(cell);
        }
        public static void AttachCell(Cell cell, Element element)
        {
            if (element == null || element.Type != ElementType.RE)
                throw new ArgumentOutOfRangeException("Cell can only be attached to RE elements!");

            if (cell.AttachedElement != null)
                ResourceManager.DeallocateCellResources(cell);

            cell.AttachedElement = element;

            ResourceManager.AllocateCellResources(cell);
        }
        public static void DetachCell(Cell cell)
        {
            if (cell.AttachedElement != null)
                ResourceManager.DeallocateCellResources(cell);

            cell.AttachedElement = null;
        }
        public Element CreateElement(ElementType type)
        {
            Element element = new Element(GenerateElementId(), type);

            Elements.Add(element);
            Elements = Elements.OrderBy(o => o.Id).ToList();

            return element;
        }
        public Cell CreateCell()
        {
            Cell cell = new Cell(GenerateCellId());

            Cells.Add(cell);
            Cells = Cells.OrderBy(o => o.Id).ToList();

            cell.PropertyChanged += new PropertyChangedEventHandler(Cell_PropertyChanged);

            return cell;
        }
        public void Delete(Element element)
        {
            if (element.HasLinks())
                throw new Exception("Links must be removed before element can be deleted");

            _ = Elements.Remove(element);
        }
        public void Delete(Cell cell)
        {
            ResourceManager.DeallocateCellResources(cell);
            _ = Cells.Remove(cell);
        }
        private static void RemoveLinkRefs(Link link)
        {
            ConnectorPort port1 = link.Port1;
            ConnectorPort port2 = link.Port2;

            port1.Link = null;
            port2.Link = null;
        }
        private uint GenerateElementId()
        {
            uint idToCheck = FirstKeyToGenerate;

            foreach (Element element in Elements)
            {
                if (element.Id != idToCheck)
                    break;

                idToCheck++;
            }

            return idToCheck;
        }
        private uint GenerateCellId()
        {
            uint idToCheck = FirstKeyToGenerate;

            foreach (Cell cell in Cells)
            {
                if (cell.Id != idToCheck)
                    break;

                idToCheck++;
            }

            return idToCheck;
        }
        private void Cell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell cell = sender as Cell;

            if (cell.AttachedElement == null)
                return;

            if (e.PropertyName == "RatType" || e.PropertyName == "Bandwidth")
            {
                if (cell.State == CellState.ALLOCATED)
                    ResourceManager.DeallocateCellResources(cell);

                ResourceManager.AllocateCellResources(cell);
            }
        }

        public List<Element> Elements { get; private set; }
        public List<Cell> Cells { get; private set; }
    }
}
