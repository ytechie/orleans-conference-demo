using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using GrainInterfaces;

namespace OrleansWeb.Controllers
{
    public class OrleansController : ApiController
    {
        public OrleansController()
        {
            Orleans.OrleansClient.Initialize(System.AppDomain.CurrentDomain.BaseDirectory + "DevTestClientConfiguration.xml");
        }

        [HttpGet]
        public string Hello()
        {
            return "hello!";
        }

        [HttpGet]
        public async Task<string> GetAttendeeLocation(int id)
        {
            var attendee = AttendeeFactory.GetGrain(id);
            var location = await attendee.GetCurrentLocation();
            var locName = await location.GetName();

            return locName;
        }
    }
}
