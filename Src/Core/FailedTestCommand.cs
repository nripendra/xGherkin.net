using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Sdk;

namespace xGherkin.Core
{
    public class FailedTestCommand : TestCommand, ITestCommand
    {
        private readonly Exception _exception;

        public FailedTestCommand(IMethodInfo method, Exception exception)
            : base(method, method.Name, 0)
        {
            _exception = exception;
        }

        public override MethodResult Execute(object testClass)
        {
            return new FailedResult(testMethod, _exception, DisplayName);
        }
    }
}
