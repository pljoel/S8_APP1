using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using USherbrooke.ServiceModel.Sondage;

namespace Server.Controllers
{
    public class Credentials
    {
        public String username;
        public String password;
          
    }
    [Route("api")]
    [Authorize]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISondageService _sondageServices;
        public SurveyController(ISondageService services)
        {
            _sondageServices = services;
        }

        [HttpPost]
        [Route("Connect")]
        public int Connect([FromBody]Credentials credentials)
        {
            return _sondageServices.Connect(credentials.username, credentials.password);
        }

        [HttpGet]
        [Route("GetAvailablePolls")]
        public IList<Poll> GetAvailablePolls([FromQuery]int userId)
        {
            return _sondageServices.GetAvailablePolls(userId);
        }

        [HttpPost]
        [Route("GetNext")]
        public PollQuestion GetNext([FromQuery]int userId, [FromBody]PollQuestion answer)
        {
            return _sondageServices.GetNext(userId, answer);
        }
    }
}