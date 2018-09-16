using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using USherbrooke.ServiceModel.Sondage;

namespace Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISondageService _services;
        public SurveyController(ISondageService services)
        {
            _services = services;
        }

        [HttpGet]
        [Route("GetAvailablePolls")]
        public IList<Poll> GetAvailablePolls(int userId)
        {
            var availablePolls = _services.GetAvailablePolls(1);

            if(availablePolls.Count <= 0)
            {
                throw new Exception();
            }
            return availablePolls;
        }

        
        public PollQuestion GetNext(int userId, PollQuestion answer)
        {
            var nextQuestion = _services.GetNext(1, new PollQuestion());

            if(nextQuestion == null)
            {
                //Cest la premiere question selon ISondageService...
            }
            return nextQuestion;
        }
    }
}