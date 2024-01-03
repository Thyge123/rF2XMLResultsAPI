using Newtonsoft.Json;
using rF2XMLTestAPI.Model;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;
using rF2XMLTestAPI.DBContext;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace rF2XMLTestAPI.Manager
{
    public class rFactorXMLManager
    {
        private DriverContext _driverContext;
        private LapsContext _lapsContext;
        private RaceResultContext _raceResultContext;
        public rFactorXMLManager(DriverContext DriverContext, LapsContext LapsContext, RaceResultContext raceResultContext)
        {
            _driverContext = DriverContext;
            _lapsContext = LapsContext;
            _raceResultContext = raceResultContext;
        }
        public Root LoadLatestResult()
        {
            var file = GetLatestFile();
            var jsonObject = ParseFileToJson(file);
            var root = ProcessJsonToRoot(jsonObject);
            var existingTrack = AddTrackToDatabase(root);
            //var existingDriver = AddDriverToDatabase(root);
            UpdateDatabase(root, existingTrack);
            return root;
        }

        public Root LoadCustomFile(string filepath)
        {
            var file = GetCustomFile(filepath);
            var json = ParseFileToJson(file);
            var root = ProcessJsonToRoot(json);
            var existingTrack = AddTrackToDatabase(root);

            return root;
        }

        public List<FileContent> GetAllXmlFilesInDirectoryWithContents(string directoryPath)
        {
            var result = new List<FileContent>();

            try
            {
                if (Directory.Exists(directoryPath))
                {
                    string[] files = Directory.GetFiles(directoryPath, "*.xml");

                    foreach (var file in files)
                    {
                        using (var reader = new StreamReader(file))
                        {
                            var content = reader.ReadToEnd();
                            result.Add(new FileContent { FilePath = file, Content = content });
                        }
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException($"The directory {directoryPath} does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting all XML files in directory with contents.", ex);
            }

            return result;
        }

        private FileInfo GetLatestFile()
        {
            try
            {
                string pattern = "*.xml";
                var filePath = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";
                var dirInfo = new DirectoryInfo(filePath);
                var latestFile = (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();
                string fileContent = File.ReadAllText(latestFile.FullName);
                return latestFile;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the latest file.", ex);
            }
        }

        public FileInfo GetCustomFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return new FileInfo(filePath);
                }
                else
                {
                    throw new FileNotFoundException($"The file {filePath} does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the custom file.", ex);
            }
        }

        public FileInfo[] GetAllXmlFiles()
        {
            try
            {
                string pattern = "*.xml";
                var filePath = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";
                var dirInfo = new DirectoryInfo(filePath);
                return dirInfo.GetFiles(pattern).OrderByDescending(f => f.LastWriteTime).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting all XML files.", ex);
            }
        }

        private JObject ParseFileToJson(FileInfo file)
        {
            try
            {
                using (StreamReader r = new StreamReader(file.FullName))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(r);
                    JObject jsonObject = JObject.Parse(JsonConvert.SerializeXmlNode(doc));

                    string[] keys = { "Practice", "Practice1", "Practice2", "Practice3", "Qualify", "Qualify1", "Qualify2", "Qualify3", "Race", "Race1", "Race2", "Race3", "TestDay" };
                    JToken driverElement = keys.Select(key => jsonObject.SelectToken($"rFactorXML.RaceResults.{key}.Driver")).FirstOrDefault(token => token != null);

                    if (driverElement != null)
                    {
                        jsonObject["rFactorXML"]["RaceResults"].Last.Remove();
                        jsonObject["rFactorXML"]["RaceResults"]["Driver"] = driverElement;
                    }

                    return jsonObject;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while parsing file to JSON.", ex);
            }
        }
        private Root ProcessJsonToRoot(JObject jsonObject)
        {
            try
            {
                Root? root = jsonObject.ToObject<Root>();
                return root;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing JSON.", ex);
            }
        }

        private RaceResults AddTrackToDatabase(Root root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root), "root cannot be null.");
            }

            string TrackCourse = root.rFactorXML.RaceResults.TrackCourse.ToString();

            if (string.IsNullOrEmpty(TrackCourse))
            {
                throw new ArgumentException("TrackCourse cannot be null or empty.", nameof(TrackCourse));
            }

            try
            {
                RaceResults existingTrack = _raceResultContext.RaceResults.AsNoTracking().FirstOrDefault(d => d.TrackCourse == TrackCourse);
                if (existingTrack == null)
                {
                    var entityEntry = _raceResultContext.RaceResults.Add(new RaceResults { TrackCourse = TrackCourse });
                    _raceResultContext.SaveChanges();
                    existingTrack = entityEntry.Entity;
                }
                return existingTrack;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding track to database.", ex);
            }
        }

        private void UpdateDatabase(Root root, RaceResults existingTrack)
        {
            try
            {
                if (root != null && root.rFactorXML.RaceResults.Driver != null)
                {
                    foreach (var driver in root.rFactorXML.RaceResults.Driver)
                    {
                        //Split Driver Name into First and Last Name
                        string[] names = driver.FirstName.Split(' ');
                        if (!names.Any(string.IsNullOrEmpty))
                        {
                            driver.FirstName = names[0];
                            driver.LastName = names[1];
                        }
                        //Check if Driver already exists in Database
                        Driver? existingDriver = _driverContext.Drivers.FirstOrDefault(d => d.FirstName == driver.FirstName && d.LastName == driver.LastName);

                        if (existingDriver == null)
                        {
                            var entityDriver = _driverContext.Drivers.Add(new Driver { FirstName = driver.FirstName, LastName = driver.LastName });
                            //_driverContext.SaveChanges();
                            existingDriver = entityDriver.Entity;
                        }

                        if (driver.Lap != null)
                        {
                            foreach (var lap in driver.Lap)
                            {
                                if (lap != null && lap.LapTime != "--.----")
                                {
                                    var lapEntity = new Lap
                                    {
                                        DriverId = existingDriver != null ? existingDriver.Id : driver.Id,
                                        LapTime = lap.LapTime,
                                        Sector1 = lap.Sector1,
                                        Sector2 = lap.Sector2,
                                        Sector3 = lap.Sector3,
                                        RaceResultsId = existingTrack.Id,
                                        CarClass = driver.CarClass,
                                        CarType = driver.CarType,
                                        Fuel = lap.Fuel,
                                        VehName = driver.VehName
                                    };

                                    _lapsContext.Laps.Add(lapEntity);

                                }
                            }
                        }
                    }
                    _driverContext.SaveChanges();
                    _lapsContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the database.", ex);
            }
        }
    }
}
