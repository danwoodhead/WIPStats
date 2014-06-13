using NUnit.Framework;
using WIPStats.Controllers;

namespace WIPStats.Test {

    [TestFixture]
    public class HomeControllerTests {
        
        private const int MY_WORK_BACKLOG_ID = 1012782;
        private const int HOME_BACKLOG_ID = 1015186;

        private HomeController home;

        [SetUp]
        public void SetUp() {
            home = new HomeController();
        }

        //[Test]
        //public void GetProjectsToBackupIdsReturnsMyWorkAndHomeIds() {
        //    var foo = home.GetProjectsToBackupIds();
        //    Assert.Contains(MY_WORK_BACKLOG_ID, foo);
        //    Assert.Contains(HOME_BACKLOG_ID, foo);
        //}

    }
}
