
namespace xGherkin.Core.Steps
{
    public class ThenStep : GherkinStep
    {
        public ThenStep(string desc, GherkinAction body)
            : base(desc, body)
        {
            StepType = StepTypes.Then;
        }

        public override bool IsValidNextStep(GherkinStep step)
        {
            return step is ThenStep;
        }
    }
}
