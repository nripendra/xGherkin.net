using System;
using System.Collections.Generic;
using Xunit.Sdk;

namespace xGherkin.Core.Steps
{
    public delegate void GherkinAction(Dictionary<string, object> args);

    public delegate void GherkinAction<T>(T args);

    public abstract class GherkinStep
    {
        public StepTypes StepType { get; protected set; }

        public string Description { get; protected set; }

        public GherkinAction Body { get; protected set; }

        public bool IsLastStep { get; set; }

        public Dictionary<string, object> Args { get; private set; }

        public GherkinStep(string desc, GherkinAction body)
        {
            Description = desc;
            Body = body;
            IsLastStep = true;

            Args = new Dictionary<string, object>();
        }

        public virtual void Execute()
        {
            Body(Args);
        }

        public abstract bool IsValidNextStep(GherkinStep step);

        public static GherkinStep Create(string desc, GherkinAction action)
        {
            //https://github.com/cucumber/cucumber/wiki/Feature-Introduction

            if ((ScenarioContext.CurrentStep is GivenStep || (ScenarioContext.CurrentStep == null)) &&
                desc.StartsWith("Given", StringComparison.OrdinalIgnoreCase))
            {
                return new GivenStep(desc, action);
            }
            else if (desc.StartsWith("When", StringComparison.OrdinalIgnoreCase))
            {
                return new WhenStep(desc, action);
            }
            else if (desc.StartsWith("Then", StringComparison.OrdinalIgnoreCase))
            {
                return new ThenStep(desc, action);
            }
            else if (desc.StartsWith("And", StringComparison.OrdinalIgnoreCase) || desc.StartsWith("But", StringComparison.OrdinalIgnoreCase))
            {
                if (ScenarioContext.CurrentStep is GivenStep)
                {
                    return new GivenStep(desc, action);
                }
                else if (ScenarioContext.CurrentStep is WhenStep)
                {
                    return new WhenStep(desc, action);
                }
                else if (ScenarioContext.CurrentStep is ThenStep)
                {
                    return new ThenStep(desc, action);
                }
                else
                {
                    Guard.ArgumentValid("desc", "Conjunction like 'And', 'But' are valid for first step. Try using Given or When step.", false);
                }
            }

            return null;
        }
    }
}
