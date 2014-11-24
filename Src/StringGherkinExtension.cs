using xGherkin.Core;
using xGherkin.Core.Steps;

namespace xGherkin
{
    public static class StringGherkinExtension
    {
        public static GherkinStep Do(this string desc, GherkinAction action)
        {
            var newStep = GherkinStep.Create(desc, action);

            if (newStep != null)
            {
                ScenarioContext.AddStep(newStep);
            }

            return newStep;
        }

        public static GivenStep Do(this string desc, GherkinTable table, GherkinAction<GherkinTable> action)
        {
            var newStep = new GivenStep(desc, action);
            newStep.DataSamples = table;

            ScenarioContext.AddStep(newStep);

            return newStep;
        }
    }
}
