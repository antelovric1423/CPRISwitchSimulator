using System.Collections.Generic;
using System.Linq;
using System;

namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        private static class ResourceManager
        {
            public static void HandleCapacityCreated()
            {
                Console.WriteLine("HandleCapacityCreated()");
                TopologyModel topology = TopologyModel.Instance;

                AllocateNonAllocatedCells(topology.Cells);
            }
            public static void DeallocateCellResources(Cell cell)
            {
                Console.WriteLine("DeallocateCellResources() cell: " + cell.Id);
                if (cell.State != CellState.ENABLED)
                    return;

                GetAllocatedPath(cell).ForEach((Action<Capacity>)(pathCapacity =>
                {
                    pathCapacity.Deallocate(cell.Id);
                    cell.State = CellState.DISABLED;
                }));
            }
            public static void AllocateCellResources(Cell cell)
            {
                Console.WriteLine("AllocateCellResources() cell: " + cell.Id);
                List<(Capacity, uint)> shortestCapacityPath = GetShortestPathToRec(cell, out string ErrorMessage);
                if (shortestCapacityPath == null)
                {
                    cell.State = CellState.DISABLED;
                    cell.ErrorMessage = ErrorMessage;
                    return;
                }
                Console.WriteLine("--");

                foreach ((Capacity capacity, uint startIndex) in shortestCapacityPath)
                {
                    Console.WriteLine("Allocating cell: " + cell.Id);
                    capacity.Allocate(cell.Id,
                        cell.GetAxcContainerFormat(),
                        cell.GetReqAxcContainerCount(),
                        startIndex);
                }

                Console.WriteLine("--");
                cell.State = CellState.ENABLED;
                cell.ErrorMessage = null;
            }
            private static void AllocateNonAllocatedCells(List<Cell> cells)
            {
                foreach (Cell cell in cells)
                {
                    if (cell.AttachedElement == null || cell.State == CellState.ENABLED)
                        return;

                    AllocateCellResources(cell);
                }
            }
            private static List<Capacity> GetAllocatedPath(Cell cell)
            {
                Console.WriteLine("GetAllocatedPath()");
                Element currentElement = cell.AttachedElement;
                uint allocationRef = cell.Id;
                List<Capacity> capacities = new List<Capacity>();
                List<Element> visitedElements = new List<Element>()
                {
                   currentElement
                };

                while (currentElement != null)
                {
                    Element nextElement = null;

                    foreach (ProcessingPort port in currentElement.Ports.Where(x => x is ProcessingPort).ToList())
                    {
                        if (port.Capacity.IsAllocated(allocationRef))
                        {
                            Console.WriteLine("GetAllocatedPath() Adding port: " + port.Name + " capacity to list");
                            capacities.Add(port.Capacity);
                            return capacities;
                        }
                    }

                    foreach (ConnectorPort port in currentElement.Ports.Where(x => x is ConnectorPort && (x as ConnectorPort).Link != null).ToList())
                    {
                        Link link = port.Link;
                        Element connectedElement = link.GetOppositePort(port).Parent;

                        if (!visitedElements.Contains(connectedElement)
                            && link.Capacity.IsAllocated(allocationRef))
                        {
                            Console.WriteLine("GetAllocatedPath() Adding port: " + port.Name + " capacity to list");
                            nextElement = connectedElement;
                            visitedElements.Add(connectedElement);
                            capacities.Add(link.Capacity);
                            break;
                        }
                    }

                    currentElement = nextElement;
                }

                throw new Exception("Cell is allocated without being terminated in REC");
            }
            private static List<(Capacity, uint)> GetShortestPathToRec(Cell cell, out string errorMessage)
            {
                Console.WriteLine("GetShortestPathToRec() cell: " + cell.Id);
                AxcContainerFormat format = cell.GetAxcContainerFormat();
                uint containerCount = cell.GetReqAxcContainerCount();
                Element lastAllocatableElement = null;
                List<(Element head, List<(Capacity, uint)> path)> possiblePaths = new List<(Element, List<(Capacity, uint)>)>()
                {
                    (cell.AttachedElement, new List<(Capacity, uint)>())
                };
                List<Element> visitedElements = new List<Element>()
                {
                   cell.AttachedElement
                };

                while (possiblePaths.Count != 0)
                {
                    List<(Element head, List<(Capacity, uint)> path)> newPossiblePaths = new List<(Element, List<(Capacity, uint)>)>();

                    foreach ((Element head, List<(Capacity, uint)> path) in possiblePaths)
                    {
                        Element currentElement = head;

                        foreach (ProcessingPort port in currentElement.Ports.Where(x => x is ProcessingPort).ToList())
                        {
                            Capacity capacity = port.Capacity;

                            if (capacity.GetContinuousAvailableAxcContainers(format, containerCount, out uint startContainer))
                            {
                                Console.WriteLine("GetShortestPathToRec() cell: " + cell.Id + " Add port: " + port.Name);
                                path.Add((capacity, startContainer));
                                
                                errorMessage = null;
                                return path;
                            }
                        }

                        foreach (ConnectorPort port in currentElement.Ports.Where(x => x is ConnectorPort && (x as ConnectorPort).Link != null).ToList())
                        {
                            Link link = port.Link;
                            Element connectedElement = link.GetOppositePort(port).Parent;

                            if (!visitedElements.Contains(connectedElement)
                                && link.Capacity.GetContinuousAvailableAxcContainers(format, containerCount, out uint startContainer))
                            {
                                Console.WriteLine("GetShortestPathToRec() cell: " + cell.Id + " Add port: " + port.Name + " link");
                                List<(Capacity, uint)> newPath = new List<(Capacity, uint)>(path);
                                newPath.Add((link.Capacity, startContainer));

                                visitedElements.Add(connectedElement);
                                newPossiblePaths.Add((connectedElement, newPath));

                                lastAllocatableElement = currentElement;
                            }
                        }
                    }

                    possiblePaths = newPossiblePaths;
                }

                Console.WriteLine("GetShortestPathToRec() No allocatable path found for cell: " + cell.Id);
                if (lastAllocatableElement == null)
                    errorMessage = "Insufficient resources on attached element";
                else
                    errorMessage = "Insufficient resources / no allocatable path found after element: " + lastAllocatableElement.Name;

                return null;
            }
        }
    }
}
