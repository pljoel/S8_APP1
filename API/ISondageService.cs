using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace USherbrooke.ServiceModel.Sondage
{
    /// <summary>
    /// Interface décrivant le service de sondage.
    /// </summary>
    [ServiceContract(Namespace = "http://USherbrooke.ServiceModel.Sondage")]
    public interface ISondageService
    {
        /// <summary>
        /// Effectue une connexion au système de sondage, puis retourne l'identifiant de l'utilisateur qui devra être utilisé pour répondre aux questions.
        /// </summary>
        /// <returns>L'identifiant de l'utilisateur.</returns>
        [OperationContract]
        int Connect(String username, String password);

        /// <summary>
        /// Liste les sondages disponibles.
        /// </summary>
        /// <param name="userId">L'identifiant unique de l'utilisateur. Il est exigé ici pour s'assurer que l'utilisateur s'est bien connecté avant de faire une opération.</param>
        /// <returns>Les sondages disponibles.</returns>
        [OperationContract]
        IList<Poll> GetAvailablePolls(int userId);

        /// <summary>
        /// En fonction de la réponse à la question courante, retourne la prochaine question pour le sondage en cours. Si la réponse est invalide, la même question est retournée.
        /// Pour aller chercher la première réponse du sondage, questionId de answer doit être null.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur courant.</param>
        /// <param name="answer">La réponse à la question courante, ou null pour obtenir la première question.</param>
        /// <returns>La prochaine question, ou la même question si la réponse est invalide.</returns>
        [OperationContract]
        PollQuestion GetNext(int userId, PollQuestion answer);

    }
}
