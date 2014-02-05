using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PivotalTrackerDotNet;
using PivotalTrackerDotNet.Domain;
using WIPStats.Models;

namespace WIPStats.Controllers {
    public class HomeController : Controller {
        private readonly string authenticationToken;
        private const int FEATURE_BACKLOG_ID = 590093;
        private readonly DateTime juniorStarted; // 
        private readonly StoryService storyService;

        private IEnumerable<Iteration> Iterations;
        private Project Project;

        public HomeController() {
            //TODO into non-checked in text file
            authenticationToken = ConfigurationManager.AppSettings["pivotalAuthToken"];
            storyService = new StoryService(authenticationToken);
            juniorStarted = new DateTime(2013, 5, 06);  // actually 13th, but need to hack to allow for offset
            //TODO cache? is this the best way to do this?
            Project = GetProject(FEATURE_BACKLOG_ID);
            Iterations = GetIterationsSinceJuniorJoined(Project.Id);
        }

        public ActionResult Index() {
            return View("Index");
        }

        public ActionResult FeatureWipStats() {
            var featureBacklog = new BacklogViewModel(Project, Iterations);
            return View("FeatureWipStats", featureBacklog);
        }

        public ActionResult CycleTime() {
            //TODO can i get combined?
            //TODO need new viewmodel?
            //TODO input for number of months, new action?
            return View("Index");
        }

        private IEnumerable<Iteration> GetIterationsSinceJuniorJoined(int projectId) {
            var iterations = storyService.GetAllIterations(projectId, 1000, 1).Where(it => this.IterationStartedAfterDanJunior(it.Start) && this.IterationIsDone(it.Finish));
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
            DateTime dateTime;
            if (DateTime.TryParseExact(date, "MM/dd/yyyy HH:mm:ss", null, DateTimeStyles.None, out dateTime)) return dateTime > juniorStarted;
            return false;
        }

        private Project GetProject(int projectId) {
            var project = new ProjectService(authenticationToken).GetProjects().Single(p => p.Id == projectId);
            return (project);
        }
    }
}
