using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;
using rF2XMLTestAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace rF2XMLTestAPI.Manager.Tests
{
    [TestClass]
    public class TracksManagerTests
    {
        private TracksManager _tracksManager;
        private RaceResultContext _raceResultContext;
        private Context _context;

        [TestInitialize]
        public void Setup()
        {
            var connectionString = Secrets.connectionString;
            // Use InMemory database for testing
            var options = new DbContextOptionsBuilder<RaceResultContext>().UseSqlServer(connectionString)
                .Options;

            var options2 = new DbContextOptionsBuilder<Context>().UseSqlServer(connectionString)
                .Options;
            // Create a context (connection to the InMemory database)
            _raceResultContext = new RaceResultContext(options);
            _context = new Context(options2);

            // Create the manager to be tested
            _tracksManager = new TracksManager(_raceResultContext, _context);
        }

            [TestMethod]
        public void GetAllTracks_WhenCalled_ReturnsAllTracks()
        {
            // Arrange

            // Act
            if (_tracksManager != null)
            {
                var result = _tracksManager.GetAllTracks();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType<IEnumerable<RaceResults>>(result);

                Assert.AreEqual(2, result.Count());
            }
        }

        [TestMethod]
        public void GetLapsAtTrack_WhenTrackCourseExists_ReturnsLapsAtTrack()
        {
            // Arrange
            string trackCourse = "Lime Rock Park -- All Chicanes";

            // Act
            if (_tracksManager != null)
            {
                var result = _tracksManager.GetLapsAtTrack(trackCourse);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<IEnumerable<object>>(result);
            Assert.AreEqual(218, result.Count());
            }
        }

        [TestMethod]
        public void GetLapsAtTrack_WhenTrackCourseDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            string trackCourse = "NonExistentTrackCourse";

            // Act
            if (_tracksManager != null)
            {
                var result = _tracksManager.GetLapsAtTrack(trackCourse);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType<IEnumerable<object>>(result);
                Assert.AreEqual(0, result.Count());
            }
        }
        [TestMethod]
        public void GetLapsAtTrack_WhenTrackCourseIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            string nullTrackCourse = null;

            // Act and Assert
            Assert.ThrowsException<ArgumentNullException>(() => _tracksManager.GetLapsAtTrack(nullTrackCourse));
        }

        [TestMethod]
        public void GetLapsAtTrack_WhenTrackCourseIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            string emptyTrackCourse = string.Empty;

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => _tracksManager.GetLapsAtTrack(emptyTrackCourse));
        }
    }
}