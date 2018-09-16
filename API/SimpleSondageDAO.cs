using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USherbrooke.ServiceModel.Sondage
{
    /// <summary>
    /// Implémentation très simple de l'interface de communication, utilisant la mémoire pour stocker les réponses.
    /// Naturellement, il s'agit ici d'une preuve de concept. Une implémentation beaucoup plus robuste, utilisant
    /// un médium de stockage permanent, devrait être utilisée dans une solution réelle.
    /// </summary>
    public class SimpleSondageDAO : ISondageDAO
    {
        // Lorsque l'on ne possède pas d'identifiant.
        private const int NO_AVAILABLE_ID = -1;

        // Contient les descriptions des sondages disponibles.
        private readonly IList<Poll> pollDescriptions = new List<Poll>();

        // Contient les questions pour chacun des sondages.
        private readonly IDictionary<int, IList<PollQuestion>> availablePolls = new Dictionary<int, IList<PollQuestion>>();

        // Stocke les réponses fournies aux différentes questions.
        private readonly IDictionary<int, IDictionary<int, IList<PollQuestion>>> answeredPolls = new Dictionary<int, IDictionary<int, IList<PollQuestion>>>();

        /// <summary>
        /// Constructeur pour le SimpleSondageDAO.
        /// </summary>
        public SimpleSondageDAO()
        {
            // définition des sondages disponibles
            pollDescriptions.Add(new Poll {Id = 1, Description = "Lecture à la maison"});
            pollDescriptions.Add(new Poll {Id = 2, Description = "Consommation de café et d'alcool"} );

            // définition des questions des différents sondages
            IList<PollQuestion> poll1 = new List<PollQuestion>();
            poll1.Add(new PollQuestion { PollId = 1, QuestionId = 11, Text = "À quelle tranche d'âge appartenez-vous? a:0-25 ans, b:25-50 ans, c:50-75 ans, d:75 ans et plus"});
            poll1.Add(new PollQuestion { PollId = 1, QuestionId = 12, Text = "Êtes-vous une femme ou un homme? a:Femme, b:Homme, c:Je ne veux pas répondre" });
            poll1.Add(new PollQuestion { PollId = 1, QuestionId = 13, Text = "Quel journal lisez-vous à la maison? a: La Presse, b:Le Journal de Montréal, c:The Gazette, d:Le Devoir" });
            poll1.Add(new PollQuestion { PollId = 1, QuestionId = 14, Text = "Combien de temps accordez-vous à la lecture de votre journal quotidiennement? a:Moins de 10 minutes; b:Entre 10 et 30 minutes, c:Entre 30 et 60 minutes, d:60 minutes ou plus" });

            IList<PollQuestion> poll2 = new List<PollQuestion>();
            poll2.Add(new PollQuestion { PollId = 2, QuestionId = 21, Text = "À quelle tranche d'âge appartenez-vous? a:0-25 ans, b:25-50 ans, c:50-75 ans, d:75 ans et +"});
            poll2.Add(new PollQuestion { PollId = 2, QuestionId = 22, Text = "Êtes-vous une femme ou un homme? a:Femme, b:Homme, c:Je ne veux pas répondre"});
            poll2.Add(new PollQuestion { PollId = 2, QuestionId = 23, Text = "Combien de tasses de café buvez-vous chaque jour? a:Je ne bois pas de café, b:Entre 1 et 5 tasses, c:Entre 6 et 10 tasses, d:10 tasses ou plus"});
            poll2.Add(new PollQuestion { PollId = 2, QuestionId = 24, Text = "Combien de consommations alcoolisées buvez-vous chaque jour? a:0, b:1, c:2 ou 3, d:3 ou plus"});

            availablePolls.Add(1, poll1);
            availablePolls.Add(2, poll2);
        }

        /// <summary>
        /// Liste les sondages disponibles.
        /// </summary>
        /// <returns>Les sondages disponibles, où la clé est l'identifiant du sondage et la valeur est la description du sondage.</returns>
        /// <exception cref="PersistenceException">Cette exception sera lancée si le DAO est incapable de communiquer avec le médium de stockage pour lire les sondages disponibles</exception>
        public IList<Poll> GetAvailablePolls()
        {
            return pollDescriptions;
        }

        /// <summary>
        /// En fonction de la question courante, retourne la question suivante, ou null si le sondage est terminé.
        /// </summary>
        /// <param name="pollId">L'identifiant unique du sondage.</param>
        /// <param name="currentQuestionId">L'identifiant unique de la question courante, ou -1 si on veut la première question du sondage.</param>
        /// <returns>La prochaine question, ou null si le sondage est terminé.</returns>
        /// <exception cref="InvalidIdException">Cette exception sera lancée si l'un des identifiants fournis au DAO est invalide</exception>
        /// <exception cref="PersistenceException">Cette exception sera lancée si le DAO est incapable de communiquer avec le médium de stockage pour lire les questions</exception>
        public PollQuestion GetNextQuestion(int pollId, int currentQuestionId)
        {
            IList<PollQuestion> questions = new List<PollQuestion>();
            if (availablePolls.TryGetValue(pollId, out questions) && questions.Count > 0)
            {
                // if there is no previous question id, returning the first question
                if (currentQuestionId == NO_AVAILABLE_ID)
                {
                    return questions[0];
                }

                // looking for the current question, so that we can return the next one
                int count = questions.Count;
                for (int i = 0; i < count; i++)
                {
                    if (questions[i].QuestionId == currentQuestionId && count > i + 1)
                    {
                        return questions[i + 1];
                    }
                }

            }
            return null;
        }

        /// <summary>
        /// Sauvegarde la réponse fournie dans le médium de stockage.
        /// </summary>
        /// <param name="userId">L'identifiant unique de l'utilisateur.</param>
        /// <param name="question">La question, incluant sa réponse.</param>
        /// <exception cref="InvalidIdException">Cette exception sera lancée si l'un des identifiants fournis au DAO est invalide</exception>
        /// <exception cref="PersistenceException">Cette exception sera lancée si le DAO est incapable de communiquer avec le médium de stockage pour sauvegarder la réponse</exception>
        public void SaveAnswer(int userId, PollQuestion question)
        {
            // Vérification d'un poll ID valide
            IList<PollQuestion> questions;
            if (!availablePolls.TryGetValue(question.PollId, out questions)) {

                throw new InvalidIdException(question.PollId.ToString(), "Invalid poll ID!");
            }

            // S'il existe déjà des réponses pour ce sondage, on vérifie si l'utilisateur courant y a déjà répondu
            IDictionary<int, IList<PollQuestion>> pollAnswers;
            if (answeredPolls.TryGetValue(question.PollId, out pollAnswers))
            {
                // S'il existe déjà des réponses pour cet usager, on vérifie s'il a déjà répondu à cette question en particulier
                IList<PollQuestion> answersByUser;
                if (pollAnswers.TryGetValue(userId, out answersByUser))
                {
                    int deleteIndex = -1;
                    for (int i = 0; i < answersByUser.Count; i++)
                    {
                        if (answersByUser[i].QuestionId == question.QuestionId)
                        {
                            deleteIndex = i;
                        }
                    }

                    // Si l'usager avait déjà répondu à cette question, on supprime sa réponse précédente
                    if (deleteIndex > -1)
                    {
                        answersByUser.RemoveAt(deleteIndex);
                    }
                } 
                // S'il n'existe pas de réponses pour cet usager, on crée une nouvelle liste de réponses
                else {
                    answersByUser = new List<PollQuestion>();
                    pollAnswers.Add(userId, answersByUser);
                }

                // Ensuite on ajoute sa nouvelle réponse
                answersByUser.Add(question);   
            }

            // Si aucune question n'a jamais été répondue pour le sondage courant, on crée une nouvelle entrée
            else
            {
                pollAnswers = new Dictionary<int, IList<PollQuestion>>();
                IList<PollQuestion> answersByUser = new List<PollQuestion>();
                answersByUser.Add(question);
                pollAnswers.Add(userId, answersByUser);
                answeredPolls.Add(question.PollId, pollAnswers);
            }
        }
    }
}
