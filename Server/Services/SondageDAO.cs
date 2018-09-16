using System;
using System.Collections.Generic;
using System.Text;
using USherbrooke.ServiceModel.Sondage;

namespace Server.Services
{
    public class SondageDAO : ISondageDAO
    {
        private readonly SimpleSondageDAO _simpleSondageDAO;

        public SondageDAO()
        {
            _simpleSondageDAO = new SimpleSondageDAO();
        }

        public IList<Poll> GetAvailablePolls()
        {
            return _simpleSondageDAO.GetAvailablePolls();
        }

        public PollQuestion GetNextQuestion(int pollId, int currentQuestionId)
        {
            return _simpleSondageDAO.GetNextQuestion(pollId, currentQuestionId);
        }

        public void SaveAnswer(int userId, PollQuestion question)
        {
            _simpleSondageDAO.SaveAnswer(userId, question);
        }
    }
}
