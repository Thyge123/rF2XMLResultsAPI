using rF2XMLTestAPI.DBContext;
using rF2XMLTestAPI.Model;

namespace rF2XMLTestAPI.Manager
{
    public class DriversManager
    {
        private readonly DriverContext _context;

        public DriversManager(DriverContext context)
        {
            _context = context;
        }

        public List<Driver> GetDrivers()
        {
            return _context.Drivers.ToList();
        }

        public Driver GetDriver(int id)
        {
            return _context.Drivers.Find(id);
        }

        public void AddDriver(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("First name and last name are required.");
            }

            var driver = new Driver { FirstName = firstName, LastName = lastName };
            _context.Drivers.Add(driver);
            _context.SaveChanges();
        }

        public void UpdateDriver(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("First name and last name are required.");
            }
            var driver = new Driver { FirstName = firstName, LastName = lastName };
            _context.Drivers.Update(driver);
            _context.SaveChanges();
        }

        public void DeleteDriver(int id)
        {
            var driver = _context.Drivers.Find(id);
            _context.Drivers.Remove(driver);
            _context.SaveChanges();
        }
    }
}
