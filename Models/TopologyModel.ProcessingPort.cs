namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        public class ProcessingPort : Port
        {
            public ProcessingPort(Element parent, string name, LineRate maxLineRate)
            {
                Parent = parent;
                Name = name;
                MaxLineRate = maxLineRate;
                Capacity = new Capacity();
                Capacity.SetLineRate(maxLineRate);
                Type = PortType.PROCESSING_PORT;
            }

            private Capacity _capacity;
            public Capacity Capacity { get { return _capacity; } private set { _capacity = value; } }
        }
    }
}
