using xGherkin.Core.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Sdk;

namespace xGherkin.Core
{
    internal static class SenarioContext
    {
        [ThreadStatic]
        public static bool IsBackground;

        [ThreadStatic]
        public static List<GherkinStep> BackgroundSteps = new List<GherkinStep>();

        [ThreadStatic]
        public static List<GherkinStep> SenarioSteps = new List<GherkinStep>();

        [ThreadStatic]
        public static GherkinTable Example;

        public static GherkinStep CurrentStep
        {
            get
            {
                if (IsBackground)
                {
                    return BackgroundSteps.LastOrDefault();
                }
                else
                    return SenarioSteps.LastOrDefault();
            }
        }

        public static void AddStep(GherkinStep newStep)
        {
            Guard.ArgumentNotNull("newStep", newStep);
            
            List<GherkinStep> steps = IsBackground ? BackgroundSteps : SenarioSteps;

            var cur = steps.LastOrDefault();

            Guard.ArgumentValid("newStep", "Then step is not valid for first step. Try using Given or When step.", !(cur == null && newStep is ThenStep));

            if (cur == null)
            {
                steps.Add(newStep);
            }
            else if (cur.IsValidNextStep(newStep))
            {
                cur.IsLastStep = false;
                steps.Add(newStep);
            }
            else
            {
                Guard.ArgumentValid("newStep", newStep.StepType + " is not valid after " + cur.StepType, false);
            }
        }

        public static IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method, Action<IMethodInfo> CreateInstance)
        {
            Exception exception = null;
            try
            {
                CreateInstance(method);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                yield return new FailedTestCommand(method, exception);
                yield break;
            }

            if (method.HasAttribute(typeof(SenarioOutlineAttribute)))
            {
                if (SenarioContext.Example == null)
                {
                    try
                    {
                        throw new Exception("Examples are necessary for senario outline. Try using Senario attribute instead?");
                    }
                    catch(Exception ex)
                    {
                        exception = ex;
                    }

                    yield return new FailedTestCommand(method, exception);
                    yield break;
                }

                foreach (var Row in Example.Rows)
                {
                    yield return CreateCommand(method, Row, Example.Cols);
                }
            }
            else
            {
                yield return CreateCommand(method, null, null);
            }
        }

        private static GherkinCommand CreateCommand(IMethodInfo method, System.Data.DataRow row, IEnumerable<System.Data.DataColumn> columns)
        {
            string displayName;
            StringBuilder message = new StringBuilder();
            List<Action> actionList = new List<Action>();

            var feature = method.Class.GetCustomAttributes(typeof(FeatureAttribute)).Select(x => x.GetPropertyValue<string>("Feature") + "\n\t" + x.GetPropertyValue<string>("Description")).FirstOrDefault();

            var tags = method.Class.GetCustomAttributes(typeof(TagAttribute));

            if (!string.IsNullOrEmpty(feature))
            {
                foreach (Xunit.TraitAttribute tag in tags.Select(x => x.GetInstance<Xunit.TraitAttribute>()))
                {
                    message.Append(string.Format("@{0} ", tag.Value));
                }

                message.AppendLine();

                message.Append("Feature: ").AppendLine(feature);
            }

            message.AppendLine();

            if (BackgroundSteps.Count > 0)
            {
                message.AppendLine("Background:").AppendLine();
            }

            foreach (var step in BackgroundSteps)
            {
                message.AppendLine(step.Description);
                if (step is GivenStep)
                {
                    var givenStep = (step as GivenStep);
                    if (givenStep.DataSamples != null)
                    {
                        message.AppendLine(givenStep.DataSamples.ToConsoleString());
                    }
                }

                actionList.Add(step.Execute);
            }

            displayName = MethodUtility.GetDisplayName(method);


            if (SenarioSteps.Count > 0)
            {
                tags = method.GetCustomAttributes(typeof(TagAttribute));

                message.AppendLine().AppendLine();

                foreach (Xunit.TraitAttribute tag in tags.Select(x => x.GetInstance<Xunit.TraitAttribute>()))
                {
                    message.Append(string.Format("@{0} ", tag.Value));
                }

                message.AppendLine();

                message.AppendLine(string.Format("Senario: {0}", displayName)).AppendLine();
            }

            foreach (var step in SenarioSteps)
            {
                string description = step.Description;

                step.Args.Clear();
                
                if (row != null && columns != null)
                {
                    foreach (var column in columns)
                    {
                        string paramName = string.Format("<{0}>", column.ColumnName);
                        if (description.IndexOf(paramName) > -1)
                        {
                            step.Args.Add(column.ColumnName, row[column]);
                        }

                        description = description.Replace(paramName, row[column] as string);
                    }
                }

                message.AppendLine(description);
                if (step is GivenStep)
                {
                    var givenStep = (step as GivenStep);
                    if (givenStep.DataSamples != null)
                    {
                        message.AppendLine(givenStep.DataSamples.ToConsoleString());
                    }
                }

                actionList.Add(step.Execute);
            }

            if (Example != null)
            {
                message.AppendLine();
                message.AppendLine("Example:").AppendLine().AppendLine(Example.ToConsoleString());
            }

            return new GherkinCommand(method, displayName, message.ToString(), MethodUtility.GetTimeoutParameter(method), (Action)Action.Combine(actionList.ToArray()));
        }
    }
}
