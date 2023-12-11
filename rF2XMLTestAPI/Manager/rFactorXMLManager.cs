using Newtonsoft.Json;
using rF2XMLTestAPI.Model;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;
using rF2XMLTestAPI.DBContext;
using Newtonsoft.Json.Linq;
using System.Linq;

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
            UpdateDatabase(root, existingTrack);
            return root;
        }


        public List<FileContent> GetAllXmlFilesInDirectoryWithContents(string directoryPath)
        {
            var result = new List<FileContent>();

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.xml");

                foreach (var file in files)
                {
                    var content = File.ReadAllText(file);
                    result.Add(new FileContent { FilePath = file, Content = content });
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"The directory {directoryPath} does not exist.");
            }

            return result;
        }

        private FileInfo GetLatestFile()
        {
            string pattern = "*.xml";
            var filePath = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";
            var dirInfo = new DirectoryInfo(filePath);
            return (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();
        }

        public  FileInfo[] GetAllXmlFiles()
        {
            string pattern = "*.xml";
            var filePath = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";
            var dirInfo = new DirectoryInfo(filePath);
            return dirInfo.GetFiles(pattern).OrderByDescending(f => f.LastWriteTime).ToArray();
        }

        private JObject ParseFileToJson(FileInfo file)
        {
            using (StreamReader r = new StreamReader(file.FullName))
            {
                XmlDocument doc = new XmlDocument();
                string xml = r.ReadToEnd();
                doc.LoadXml(xml);

                string jsonText = JsonConvert.SerializeXmlNode(doc, Formatting.Indented);

                JObject jsonObject = JObject.Parse(jsonText);

                string[] keys = { "Practice", "Practice1","Practice2","Practice3", "Qualify", "Qualify1", "Qualify2", "Qualify3", "Race", "Race1", "Race2", "Race3", "TestDay" };
                JToken driverElement = null;

                foreach (var key in keys)
                {
                    if (jsonObject["rFactorXML"]["RaceResults"][key] != null)
                    {
                        driverElement = jsonObject["rFactorXML"]["RaceResults"][key]["Driver"];
                        break;
                    }
                }

                jsonObject["rFactorXML"]["RaceResults"].Last.Remove();
                jsonObject["rFactorXML"]["RaceResults"]["Driver"] = driverElement;

                return jsonObject;
            }
        }
        private Root ProcessJsonToRoot(JObject jsonObject)
        {
            string modifiedJsonString = jsonObject.ToString();
            Root? root = JsonConvert.DeserializeObject<Root>(modifiedJsonString);
            return root;
        }

        private RaceResults AddTrackToDatabase(Root root)
        {
            string TrackCourse = root.rFactorXML.RaceResults.TrackCourse.ToString();

            RaceResults existingTrack = _raceResultContext.RaceResults.FirstOrDefault(d => d.TrackCourse == TrackCourse);
            if (existingTrack == null)
            {
                var entityEntry = _raceResultContext.RaceResults.Add(new RaceResults { TrackCourse = TrackCourse });
                _raceResultContext.SaveChanges();
                existingTrack = entityEntry.Entity;
            }
            return existingTrack;
        }

         private void UpdateDatabase(Root root, RaceResults existingTrack)
       {
            string trackVenue = root.rFactorXML.RaceResults.TrackCourse.ToString();

            if (root != null && root.rFactorXML.RaceResults.Driver != null)
            {
                foreach (var driver in root.rFactorXML.RaceResults.Driver)
                {
                    //Split Driver Name into First and Last Name
                    string[] names = driver.FirstName.Split(' ');
                    if (names.Length > 1)
                    {
                        driver.FirstName = names[0];
                        driver.LastName = names[1];
                    }

                    //Check if Driver already exists in Database
                    Driver? existingDriver = _driverContext.Drivers.FirstOrDefault(d => d.FirstName == driver.FirstName && d.LastName == driver.LastName);

                    //Driver? existingDriver = _driverContext.Drivers.FirstOrDefault(d => d.Name == driver.Name);

                    if (existingDriver == null)
                    {
                        _driverContext.Drivers.Add(new Driver { FirstName = driver.FirstName, LastName = driver.LastName });
                        _driverContext.SaveChanges();
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
                
                _lapsContext.SaveChanges();
            }
        }
    }
}
