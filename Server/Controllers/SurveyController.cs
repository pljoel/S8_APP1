using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public ActionResult<int> Connect([FromBody]Credentials credentials)
        {
            if (credentials!=null && !String.IsNullOrWhiteSpace(credentials.username) && !String.IsNullOrWhiteSpace(credentials.password))
            {
                int userId = _sondageServices.Connect(credentials.username, credentials.password);
                if (userId == -1)
                    return Unauthorized();
                else
                    return userId;
            }
            return BadRequest("Provided credentials are invalid to parse.");
        }

        [HttpGet]
        [Route("GetAvailablePolls")]
        public ActionResult<IList<Poll>> GetAvailablePolls([FromQuery]int userId)
        {
            if (userId <= 0)
                return BadRequest("Provided User ID is invalid.");

            IList<Poll> polls = _sondageServices.GetAvailablePolls(userId);
            if (polls != null)
            {
                if(polls.Count <= 0)
                {
                    return Ok("There's no available polls, right now.");
                }
                else
                    return polls.ToList();
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("GetNext")]
        public ActionResult<PollQuestion> GetNext([FromQuery]int userId, [FromBody]PollQuestion answer)
        {
            if (userId <= 0)
                return BadRequest("Provided User ID is invalid.");

            if (answer != null && answer.PollId >= 0 && answer.QuestionId >= 0 && !String.IsNullOrWhiteSpace(answer.Text))
            {
                var nextQuestion = _sondageServices.GetNext(userId, answer);
                if (nextQuestion != null)
                    return nextQuestion;
            }

            return BadRequest("Invalid answer or not yet connected.");
        }
    }
}