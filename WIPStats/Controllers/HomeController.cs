using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using PivotalTrackerDotNet;
using PivotalTrackerDotNet.Domain;
using WIPStats.Models;

namespace WIPStats.Controllers {
    public class HomeController : Controller {
        private string pivotalAuthToken;
        private string PivotalAuthToken {
            get {
                if (String.IsNullOrEmpty(pivotalAuthToken)) pivotalAuthToken = this.GetPivotalAuthToken();
                return pivotalAuthToken;
            }
        }
        private readonly int xfeatureBacklogID = Convert.ToInt32(ConfigurationManager.AppSettings["xFeatureBacklogID"]);
        private readonly int xplatformBacklogID = Convert.ToInt32(ConfigurationManager.AppSettings["xPlatformBacklogID"]);

        private StoryService myStoryService;
        private StoryService MyStoryService {
            get {
                return this.myStoryService ?? (this.myStoryService = new StoryService(this.PivotalAuthToken));
            }
        }

        public string GetPivotalAuthToken() {
            return (System.IO.File.ReadAllText(Server.MapPath(@"~/token.txt")));
        }

        public ActionResult Index() {
            return View("Index");
        }

        public ActionResult CurrentSprintStats() {
            return this.View("CurrentSprintStats");
        }

        public ActionResult CastingProjectsStats() {
            return this.View("CastingProjectsStats");
        }

        public ActionResult FeatureWipStats() {
            var project = GetProject(this.xfeatureBacklogID);
            var iterations = GetIterationsSinceJuniorJoined(project.Id);
            var featureBacklog = new BacklogViewModel(project, iterations);
            return View("FeatureWipStats", featureBacklog);
        }

        public ActionResult CycleTime() {
            var featureProject = GetProject(this.xfeatureBacklogID);
            var platformProject = this.GetProject(this.xplatformBacklogID);
            var featureIterations = this.GetMostRecentIterationsByProjectAndNumber(featureProject.Id, 6);
            var platformIterations = this.GetMostRecentIterationsByProjectAndNumber(platformProject.Id, 6);
            //TODO can i get combined?
            //TODO need new viewmodel?
            //TODO input for number of months, new action?
            return View("Index");
        }


        private IEnumerable<Iteration> GetMostRecentIterationsByProjectAndNumber(int projectId, int numberToGet) {
            var foo = this.MyStoryService.GetAllIterations(projectId, 1000, 1).Where(it => this.IterationIsDone(it.Finish));
            var latestFinishedIteration = foo.Max(it => it.Number);
            var iterations = foo.Where(it => it.Number >= latestFinishedIteration - numberToGet);
            return (iterations);
        }

        private IEnumerable<Iteration> GetIterationsSinceJuniorJoined(int projectId) {
            var foo = this.MyStoryService;
            var iterations = foo.GetAllIterations(projectId, 1000, 1).Where(it => this.IterationStartedAfterDanJunior(it.Start) && this.IterationIsDone(it.Finish));
            return (iterations);
        }

        private bool IterationIsDone(string date) {
            DateTime dateTime;
            if (DateTime.TryParseExact(date, "MM/dd/yyyy HH:mm:ss", null, DateTimeStyles.None, out dateTime)) {
                return dateTime < DateTime.Now;
            }
            return false;
        }

        private bool IterationStartedAfterDanJunior(string date) {
            var juniorStarted = new DateTime(2013, 5, 06);  // actually 13th, but need to hack to allow for offset
            DateTime dateTime;
            if (DateTime.TryParseExact(date, "MM/dd/yyyy HH:mm:ss", null, DateTimeStyles.None, out dateTime)) return dateTime > juniorStarted;
            return false;
        }

        private Project GetProject(int projectId) {
            var project = new ProjectService(this.PivotalAuthToken).GetProjects().Single(p => p.Id == projectId);
            return (project);
        }
    }
}
