using System.Collections.Generic;
using System.Linq;
using PivotalTrackerDotNet.Domain;

namespace WIPStats.Models {
    public class BacklogViewModel {
        public Project Project { get; set; }
        public IEnumerable<Sprint> Sprints { get; set; }
        public int TotalStories { get; set; }
        public int TotalFeatures { get; set; }
        public int TotalPoints { get; set; }
        public string AverageStories { get; set; }
        public string AverageFeatures { get; set; }
        public string AveragePoints { get; set; }

        public BacklogViewModel(Project project, IEnumerable<Iteration> iterations) {
            Project = project;
            Sprints = iterations.Reverse().Select(it => new Sprint(it));
            var noOfIterations = Sprints.Count();

            TotalStories = Sprints.Sum(sp => sp.Stories);
            TotalFeatures = Sprints.Sum(sp => sp.Features);
            TotalPoints = Sprints.Sum(sp => sp.Points);

            AverageStories = AverageText(TotalStories, noOfIterations);
            AverageFeatures = AverageText(TotalFeatures, noOfIterations);
            AveragePoints = AverageText(TotalPoints, noOfIterations);

        }

        private string AverageText(int numerator, int denominator) {
            return (numerator / denominator).ToString();
        }
    }
}