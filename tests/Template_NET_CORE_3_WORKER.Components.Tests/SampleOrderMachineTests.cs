namespace Template_NET_CORE_3_WORKER.Components.Tests
{
    using System;

    using Automatonymous.Graphing;
    using Automatonymous.Visualizer;

    using Template_NET_CORE_3_WORKER.Components.MassTransit.StateMachines;

    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class SampleOrderMachineTests
    {
        private readonly ITestOutputHelper _output;

        public SampleOrderMachineTests(ITestOutputHelper output)
        {
            this._output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact]
        public void Show_state_machine_as_viz()
        {
            var orderStateMachine = new OfferStateMachine();

            var graph = orderStateMachine.GetGraph();

            var generator = new StateMachineGraphvizGenerator(graph);

            var dots = generator.CreateDotFile();
            
            this._output.WriteLine(dots);
        }
    }
}