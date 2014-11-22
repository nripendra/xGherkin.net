
namespace xGherkin.Core.Steps
{
    public class WhenStep : GherkinStep
    {
        public WhenStep(string desc, GherkinAction body)
            : base(desc, body)
        {
            StepType = StepTypes.When;
        }

        public override bool IsValidNextStep(GherkinStep step)
        {
            return step is WhenStep || step is ThenStep;
        }
    }
}
