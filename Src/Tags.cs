using Xunit;

namespace xGherkin
{
    public class TagAttribute : TraitAttribute
    {
        public TagAttribute(string tag)
            : base("Tag", tag)
        {
        }
    }

    public class TaskAttribute : TagAttribute
    {
        public TaskAttribute(string id)
            : base("Task-" + id)
        {
        }
    }

    public class BugAttribute : TagAttribute
    {
        public BugAttribute(string id)
            : base("Bug-" + id)
        {
        }
    }

    public class SprintAttribute : TagAttribute
    {
        public SprintAttribute(string id)
            : base("Sprint-" + id)
        {
        }
    }

    public class IssueAttribute : TagAttribute
    {
        public IssueAttribute(string id)
            : base("Issue-" + id)
        {
        }
    }



    public class PBIAttribute : TagAttribute
    {
        public PBIAttribute(string id)
            : base("PBI-" + id)
        {
        }
    }
}
