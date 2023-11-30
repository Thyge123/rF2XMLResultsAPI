using Newtonsoft.Json;
using rF2XMLTestAPI.Model;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;
using rF2XMLTestAPI.DBContext;
using Newtonsoft.Json.Linq;

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

        private FileInfo GetLatestFile()
        {
            string pattern = "*.xml";
            var filePath = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";
            var dirInfo = new DirectoryInfo(filePath);
            return (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();
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

                JToken driverElement = jsonObject["rFactorXML"]["RaceResults"]["Practice1"]["Driver"];

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
            string trackVenue = root.rFactorXML.RaceResults.TrackVenue.ToString();

            RaceResults existingTrack = _raceResultContext.RaceResults.FirstOrDefault(d => d.TrackVenue == trackVenue);
            if (existingTrack == null)
            {
                var newTrack = _raceResultContext.RaceResults.Add(new RaceResults { TrackVenue = trackVenue });
                _raceResultContext.SaveChanges();
            }
            return existingTrack;
        }

        private void UpdateDatabase(Root root, RaceResults existingTrack)
        {
            string trackVenue = root.rFactorXML.RaceResults.TrackVenue.ToString();

            if (root != null && root.rFactorXML.RaceResults.Driver != null)
            {
                foreach (var driver in root.rFactorXML.RaceResults.Driver)
                {
                    Driver? existingDriver = _driverContext.Drivers.FirstOrDefault(d => d.Name == driver.Name);

                    if (existingDriver == null)
                    {
                        _driverContext.Drivers.Add(new Driver { Name = driver.Name });
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
                                    RaceResultsId = existingTrack.Id
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

        /*
        public Root LoadLatestResult()
        {
            string pattern = "*.xml";
            var filePath = "D:\\Racing\\rfactor2-dedicated\\UserData\\Log\\Results";
            var dirInfo = new DirectoryInfo(filePath);
            var file = (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();

            using (StreamReader r = new StreamReader(file.FullName))    
            {
                XmlDocument doc = new XmlDocument();
                string xml = r.ReadToEnd();
                doc.LoadXml(xml);

                var test = JsonConvert.SerializeXmlNode(doc);

                string fileName = file.FullName;

                //File.WriteAllText(fileName, JsonConvert.SerializeXmlNode(doc, Formatting.Indented));

                string jsonText =  JsonConvert.SerializeXmlNode(doc, Formatting.Indented);

                JObject jsonObject = JObject.Parse(jsonText);

                //Console.WriteLine(root.rFactorXML.RaceResults.TrackVenue);
                JToken driverElement = jsonObject["rFactorXML"]["RaceResults"]["Practice1"]["Driver"];

                jsonObject["rFactorXML"]["RaceResults"].Last.Remove();
                jsonObject["rFactorXML"]["RaceResults"]["Driver"] = driverElement;

                // Convert the modified JObject back to a JSON string
                string modifiedJsonString = jsonObject.ToString();

                Root? root = JsonConvert.DeserializeObject<Root>(modifiedJsonString);
                
                string trackVenue = root.rFactorXML.RaceResults.TrackVenue.ToString(); 

                if (root != null && root.rFactorXML.RaceResults.Driver != null)
                {
                    RaceResults existingTrack = _raceResultContext.RaceResults.FirstOrDefault(d => d.TrackVenue == trackVenue);
                    if (existingTrack == null)
                    {
                        _raceResultContext.RaceResults.Add(new RaceResults { TrackVenue = trackVenue });
                        _raceResultContext.SaveChanges();
                    }
                    foreach (var driver in root.rFactorXML.RaceResults.Driver)
                    {
                        // Check if the driver already exists in the database based on some criteria, e.g., Name
                        Driver? existingDriver = _driverContext.Drivers.FirstOrDefault(d => d.Name == driver.Name);

                        if (existingDriver == null)
                        {
                            // Driver name does not exist, add it to the database
                            _driverContext.Drivers.Add(new Driver { Name = driver.Name });
                        }

                        // Now, add the driver's laps to the Laps table

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
                                        RaceResultsId = existingTrack.Id
                                    };

                                    _lapsContext.Laps.Add(lapEntity);
                                }
                            }
                        }
                      
                    }

                    // Save changes to the database
                    _driverContext.SaveChanges();
                    _lapsContext.SaveChanges();
                }

                return root;

            }
        }
        */
        /*
        public Root LoadJson(string jsonText)
        {
            Root? root = JsonConvert.DeserializeObject<Root>(jsonText);

            return root;
        }
        */
    }
}
