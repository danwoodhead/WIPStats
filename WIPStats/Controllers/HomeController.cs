using System;
using System.Collections.Generic;
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

        public HomeController() {
            authenticationToken = "8c84200774373cdb629213a90491cfce";
            storyService = new StoryService(authenticationToken);
            juniorStarted = new DateTime(2013, 5, 06);  // actually 13th, but need to hack to allpw for offset
        }

        public ActionResult Index() {

            var project = GetProject(FEATURE_BACKLOG_ID);

            var iterations = GetIterationsSinceJuniorJoined(project.Id);

            var featureBacklog = new BacklogViewModel(project, iterations);

            return View("Index", featureBacklog);
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
