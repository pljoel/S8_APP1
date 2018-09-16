using System;
using System.Collections.Generic;
using System.Text;
using USherbrooke.ServiceModel.Sondage;

namespace Server.Services
{
    public class SondageService : ISondageService
    {
        private readonly SondageDAO _sondageDAO;

        public SondageService()
        {
            _sondageDAO = new SondageDAO();
        }

        public int Connect()
        {
            throw new NotImplementedException();
        }

        public IList<Poll> GetAvailablePolls(int userId)
        {
            // TODO: use the userID
            return _sondageDAO.GetAvailablePolls();
        }

        public PollQuestion GetNext(int userId, PollQuestion answer)
        {
            int pollid = 1;
            int currentQuestionID = 11;
            return _sondageDAO.GetNextQuestion(pollid, currentQuestionID);
        }
    }
}
