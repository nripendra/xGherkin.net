using System;
using Xunit.Sdk;

namespace xGherkin.Core
{
    public class GherkinCommand : TestCommand, ITestCommand
    {
        private readonly Action _body;
        private readonly string _description;

        public GherkinCommand(IMethodInfo method, string name, string description, int timeout, Action body)
            : base(method, name, timeout)
        {
            _body = body;
            _description = description;
        }

        public override MethodResult Execute(object testClass)
        {
            try
            {
                Console.WriteLine(_description);
                _body();
                return new PassedResult(testMethod, DisplayName);
            }
            catch (Exception ex)
            {
                return new FailedResult(testMethod, ex, DisplayName);
            }
        }
    }
}
