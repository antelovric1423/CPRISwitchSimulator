using System;
using System.Collections.Generic;
using CPRISwitchSimulator.Helpers;

namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        private static readonly uint UnusedAxcContainerRef = 0;
        public enum AxcContainerFormat
        {
            NOT_SET,
            FORMAT_20BIT,
            FORMAT_24BIT,
            FORMAT_30BIT
        }

        public class AxcContainer
        {
            public AxcContainer()
            {
                AllocationRef = UnusedAxcContainerRef;
            }

            public uint AllocationRef { get; set; }
        }
        /** Capacity representing 15 IQ words of CPRI basic Frame
         * 
         * Number of bits (Capacity) depends on CPRI line rate.
         * Capacity is always a multiple of 120 bits. Capacity is divided into AxC containers which contain 
         * IQ samples for some sector/cell.
         * All AxC containers must have the same AxC container format within basic frame. 
         */
        public class Capacity // Basic frame capacity
        {
            public Capacity()
            {
                AxcContainers = new AxcContainer[0];
                Format = AxcContainerFormat.NOT_SET;
                _capacityBits = 0;
            }
            public void SetLineRate(LineRate lineRate)
            {
                CheckResourcesNotAllocated();

                switch (lineRate)
                {
                    case LineRate.CPRI_2_4G:
                        _capacityBits = 480;
                        break;
                    case LineRate.CPRI_4_9G:
                        _capacityBits = 960;
                        break;
                    case LineRate.CPRI_9_8G:
                        _capacityBits = 1920;
                        break;
                    case LineRate.CPRI_10_1G:
                        _capacityBits = 2400;
                        break;
                    case LineRate.CPRI_24_3G:
                        _capacityBits = 5760;
                        break;
                    default:
                        throw new Exception("Invalid line rate for capacity initialization: " + lineRate);
                }

                InitializeAxcContainers();
            }
            public void Allocate(uint allocationRef, AxcContainerFormat format, uint noOfAxcContainers, uint startContainer)
            {
                Console.WriteLine("Allocate() allocationRef: " + allocationRef + ", format: " + format + ", count: " + noOfAxcContainers + ", start cont: " + startContainer);
                if (allocationRef == UnusedAxcContainerRef)
                    throw new ArgumentOutOfRangeException("Requested allocationRef cannot be 0");

                Format = format;

                if (startContainer + noOfAxcContainers > AxcContainers.Length)
                    throw new ArgumentOutOfRangeException("Requested allocation of AxC containers is out of range");

                for (uint i = startContainer; i < startContainer + noOfAxcContainers; i++)
                {
                    Console.WriteLine("Setting index " + i + " to " + allocationRef);
                    AxcContainers[i].AllocationRef = allocationRef;
                }
            }
            public void Deallocate(uint allocationRef)
            {
                for (uint i = 0; i < AxcContainers.Length; i++)
                {
                    if (AxcContainers[i].AllocationRef == allocationRef)
                        AxcContainers[i].AllocationRef = UnusedAxcContainerRef;
                }
            }

            /* Get index of first of a number of consecutive available axc containers in capacity.
             * 
             * format - requested container format
             * noOfAxcContainers - requested number of containers
             * startContainer - index of first of consecutive requested containers
             * 
             * Return false if conflicting format is sent or requested number of containers is not available,
             * otherwise return true.
             */
            public bool GetContinuousAvailableAxcContainers(AxcContainerFormat format, uint noOfAxcContainers, out uint startContainer)
            {
                startContainer = 0;

                if (Format == AxcContainerFormat.NOT_SET)
                    return noOfAxcContainers <= GetAxcContainerCount(format);
                else if (Format != format)
                    return false;

                uint noOfContinuousAvailableContainers = 0;

                for (uint i = 0; i < AxcContainers.Length; i++)
                {
                    if (AxcContainers[i].AllocationRef == UnusedAxcContainerRef)
                    {
                        if(noOfContinuousAvailableContainers == 0)
                            startContainer = i;

                        noOfContinuousAvailableContainers++;

                        if (noOfContinuousAvailableContainers == noOfAxcContainers)
                            return true;
                    }
                    else
                    {
                        noOfContinuousAvailableContainers = 0;
                    }
                }

                return false;
            }
            public bool IsAllocated(uint allocationRef)
            {
                foreach (var it in AxcContainers)
                {
                    if (it.AllocationRef == allocationRef)
                        return true;
                }

                return false;
            }
            public List<uint> GetAllocationRefs()
            {
                List<uint> list = new List<uint>();

                foreach(var it in AxcContainers)
                {
                    if (!list.Contains(it.AllocationRef))
                        list.Add(it.AllocationRef);
                }

                return list;
            }
            private void CheckResourcesNotAllocated()
            {
                foreach (var container in AxcContainers)
                {
                    if (container.AllocationRef != UnusedAxcContainerRef)
                        throw new Exception("Cannot change line rate while resources are allocated");
                }
            }
            private void InitializeAxcContainers()
            {
                uint bitCount;

                if (_format == AxcContainerFormat.NOT_SET || _capacityBits == 0)
                    return;

                switch(_format)
                {
                    case AxcContainerFormat.FORMAT_20BIT:
                        bitCount = 20;
                        break;
                    case AxcContainerFormat.FORMAT_24BIT:
                        bitCount = 24;
                        break;
                    case AxcContainerFormat.FORMAT_30BIT:
                        bitCount = 30;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Invalid value of _format: " + _format);
                }

                AxcContainers = ArrayInitializator.InitializeArray<AxcContainer>(_capacityBits / bitCount);
            }
            private uint GetAxcContainerCount(AxcContainerFormat format)
            {
                switch (format)
                {
                    case AxcContainerFormat.FORMAT_20BIT:
                        return _capacityBits / 20;
                    case AxcContainerFormat.FORMAT_30BIT:
                        return _capacityBits / 30;
                    default:
                        throw new ArgumentOutOfRangeException("Format not set");
                }

            }

            private uint _capacityBits;
            private AxcContainerFormat _format;
            public AxcContainer[] AxcContainers { get; private set; }
            public AxcContainerFormat Format
            {
                get { return _format; }
                private set
                {
                    if (Format != value)
                    {
                        CheckResourcesNotAllocated();
                        _format = value;
                        InitializeAxcContainers();
                    }
                }
            }
        }
    }
}
