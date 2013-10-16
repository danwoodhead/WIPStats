using System.Linq;
using PivotalTrackerDotNet.Domain;

namespace WIPStats.Models {
    public class Sprint {

        public string IterationNumber { get; set; }
        public int Stories { get; set; }
        public int Features { get; set; }
        public int Points { get; set; }

        public Sprint(Iteration iteration) {
            IterationNumber = iteration.Number.ToString();
            Stories = iteration.Stories.Count();
            Features = iteration.Stories.Count(st => st.StoryType == StoryType.Feature);
            Points = iteration.Stories.Sum(st => st.Estimate.HasValue ? st.Estimate.Value : 0);
        }
    }
}