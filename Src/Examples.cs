﻿using xGherkin.Core;

namespace xGherkin
{
    public static class Examples
    {
        public static void SetTo(GherkinTable gherkinTable)
        {
            ScenarioContext.Example = gherkinTable;
        }
    }
}
