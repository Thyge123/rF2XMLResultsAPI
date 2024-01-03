using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Manager;
using rF2XMLTestAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rF2XMLTestAPI.Manager.Tests
{
    [TestClass()]
    public class rFactorXMLManagerTests
    {
            private DriverContext _driverContext;
            private LapsContext _lapsContext;
            private RaceResultContext _raceResultContext;
            private rFactorXMLManager _manager;

        [TestInitialize]
        public void Setup()
            {
            var connectionString = Secrets.connectionString;
            // Use InMemory database for testing
            var options = new DbContextOptionsBuilder<RaceResultContext>().UseSqlServer(connectionString)
                .Options;

            var options2 = new DbContextOptionsBuilder<DriverContext>().UseSqlServer(connectionString)
                .Options;

            var options3 = new DbContextOptionsBuilder<LapsContext>().UseSqlServer(connectionString).Options;
            // Create a context (connection to the InMemory database)
            _raceResultContext = new RaceResultContext(options);
            _driverContext = new DriverContext(options2);
            _lapsContext = new LapsContext(options3);

            // Create the manager to be tested
            _manager = new rFactorXMLManager(_driverContext, _lapsContext, _raceResultContext);
        }

            [TestMethod]
            public void LoadLatestResult_ReturnsRoot()
            {
                // Arrange

                // Act
                var result = _manager.LoadLatestResult();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(Root));
        }

            [TestMethod]
            public void GetAllXmlFilesInDirectoryWithContents_ReturnsListOfFileContent()
            {
                // Arrange
                string directoryPath = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";

                // Act
                var result = _manager.GetAllXmlFilesInDirectoryWithContents(directoryPath);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(List<FileContent>));
            }
        }
    }

