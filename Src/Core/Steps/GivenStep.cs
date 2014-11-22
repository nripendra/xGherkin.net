
namespace xGherkin.Core.Steps
{
    public class GivenStep : GherkinStep
    {
        private string desc;

        public GherkinTable DataSamples { get; set; }

        public GivenStep(string desc, GherkinAction body)
            : base(desc, body)
        {
            StepType = StepTypes.Given;
        }

        public GivenStep(string desc, GherkinAction<GherkinTable> action)
            : base(desc, _ => { })
        {
            this.desc = desc;
            this.Body = _ =>
            {
                action(DataSamples);
            };
        }
        public override bool IsValidNextStep(GherkinStep step)
        {
            return step is GivenStep || step is WhenStep || step is ThenStep;
        }
    }
}
