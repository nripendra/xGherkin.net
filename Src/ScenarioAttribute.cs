using System;
using System.Collections.Generic;
using System.Linq;
using xGherkin.Core;
using Xunit;
using Xunit.Sdk;

namespace xGherkin
{
    public class ScenarioAttribute : FactAttribute
    {
        public string Title
        {
            get { return DisplayName; }
            set { DisplayName = value; }
        }

        public ScenarioAttribute(string title)
        {
            Title = title;
        }

        private ScenarioAttribute()
        {
        }

        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            foreach (ITestCommand c in ScenarioContext.EnumerateTestCommands(method, CreateInstance))
            {
                yield return c;
            }
        }

        private static void CreateInstance(IMethodInfo method)
        {
            ScenarioContext.BackgroundSteps = new List<Core.Steps.GherkinStep>();
            ScenarioContext.ScenarioSteps = new List<Core.Steps.GherkinStep>();

            if (method.IsStatic)
                method.Invoke(null, null);
            else
            {
                System.Reflection.ConstructorInfo defaultConstructor = method.MethodInfo.ReflectedType.GetConstructor(Type.EmptyTypes);
                System.Reflection.MethodInfo backGround = method.MethodInfo.ReflectedType.GetMethods().FirstOrDefault(x => string.Compare(x.Name, "Background", true) == 0);

                if (defaultConstructor == null)
                    throw new InvalidOperationException("Specification class does not have a default constructor");

                object instance = defaultConstructor.Invoke(null);

                if (backGround != null)
                {
                    ScenarioContext.IsBackground = true;

                    try
                    {
                        backGround.Invoke(instance, null);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        ScenarioContext.IsBackground = false;
                    }
                }

                method.Invoke(instance, null);
            }
        }

    }
}
