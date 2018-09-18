using System;
using System.Collections.Generic;
using System.Text;
using USherbrooke.ServiceModel.Sondage;

namespace Server.Services
{
    // Classe utilisateur de sondages
    public class User
    {
        private String username;
        private String password;
        private int userId;
        private DateTime expire;

        public User(String u, String p, int uid)
        {
            username = u;
            password = p;
            userId = uid;
            expire = new DateTime();
        }

        public String getUsername()
        {
            return username;
        }

        public int getUserId()
        {
            return userId;
        }

        public String getPassword()
        {
            return password;
        }

        public bool getActive()
        {
            return expire > DateTime.Now;
        }

        // Active le user pour 30 min
        public void setActive()
        {
            expire = DateTime.Now.AddMinutes(30);
        }
    }// Utilisateur

    public class SondageService : ISondageService
    {
        private readonly SimpleSondageDAO _simpleSondageDAO;
        private readonly Dictionary<String, User> _users;
        private Dictionary<int, String> _usersValidation;
        

        // Constructeur
        public SondageService()
        {
            _simpleSondageDAO = new SimpleSondageDAO();
            _users = new Dictionary<String, User>();
            _usersValidation = new Dictionary<int, String>();

            // Creation des users
            User user1 = new User("admin", "admin", 999);
            _users.Add(user1.getUsername(), user1);
            // Link le userId et son username
            _usersValidation.Add(user1.getUserId(), user1.getUsername());
        }

        public int Connect(String username, String password)
        {
            if (_users.ContainsKey(username))
            {
                if (_users[username].getUsername() == username && _users[username].getPassword() == password)
                {
                    // Met le user actif pour 30 min
                    _users[username].setActive();
                    return _users[username].getUserId();
                }
                return -1; //User/Password invalide...
            }
            return -1; // Le username n'existe pas...
        }

        public IList<Poll> GetAvailablePolls(int userId)
        {
            try {
                // Regarde si le userId est valide et connecter...
                if (validateUserActive(userId))
                {
                    return _simpleSondageDAO.GetAvailablePolls();
                }
                return null; // User id invalide ou non-connecter
            }
            catch (Exception e) when (e is PersistenceException)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }

        public PollQuestion GetNext(int userId, PollQuestion answer)
        {
            try
            {
                if (validateUserActive(userId))
                {
                    if (answer != null)
                    {
                        if (validatePollId(answer.PollId) && validateQuestionId(answer.PollId, answer.QuestionId))
                        {
                            // On veut la premiere question du sondage, donc -1
                            if (answer.QuestionId == -1)
                            {
                                return _simpleSondageDAO.GetNextQuestion(answer.PollId, -1);
                            }

                            if (answer.Text != null)
                            {
                                // Sanitise la reponse avant d'etre process
                                if (answer.Text.Equals("a") || answer.Text.Equals("b") || answer.Text.Equals("c") || answer.Text.Equals("d"))
                                {
                                    _simpleSondageDAO.SaveAnswer(userId, answer);
                                    return _simpleSondageDAO.GetNextQuestion(answer.PollId, answer.QuestionId);
                                }
                                else
                                {
                                    //Mauvais data, on recommence la question.
                                    return _simpleSondageDAO.GetNextQuestion(answer.PollId, answer.QuestionId - 1);
                                }
                            }
                            return null; // Lance exception car answer.Text ne contient pas une reponse attendue
                        }
                    }
                    return null; // Lance exception car answer nest pas valide
                }
                return null; // Lance Exception que le user n'est pas valide
            }
            catch (Exception e) when (e is PersistenceException)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
            catch(Exception e) when (e is InvalidIdException)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
            
        }
        

        //
        // Notre propre implementation, Non dans l'interface //
        //
        private bool validateUserActive(int userId)
        {
            // Valide que le userId existe
            if (_usersValidation.ContainsKey(userId))
            {
                // Retourne sil est actif ou non
                return _users[_usersValidation[userId]].getActive();
            }
            return false;
        }

        private bool validatePollId(int pid)
        {
            try
            {
                foreach (Poll poll in _simpleSondageDAO.GetAvailablePolls())
                {
                    if (poll.Id.Equals(pid))
                        return true;
                }
                return false; // pollId n'est pas valide
            }
            catch (Exception e) when (e is PersistenceException)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return false;
            }
        }

        private bool validateQuestionId(int pid, int qid)
        {
            try
            {
                if (validatePollId(pid))
                {
                    if (_simpleSondageDAO.GetAvailableQuestions(pid).Contains(qid) || qid == -1)
                        return true;
                }
                return false;
            }
            catch (Exception e) when (e is PersistenceException)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
