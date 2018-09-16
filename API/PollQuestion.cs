using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace USherbrooke.ServiceModel.Sondage
{   
    /// <summary>
    /// Définit une question dans un sondage. La question contient toujours l'identifiant
    /// unique du sondage auquel elle appartient, ainsi que son identifiant unique propre.
    /// Le texte contenu dans la question est soit le texte de la question, soit la réponse
    /// (selon le besoin).
    /// </summary>
    [DataContract]
    public class PollQuestion
    {
        /// <summary>
        /// Identifiant du sondage auquel appartient cette question.
        /// </summary>
        [DataMember]
        public int PollId { get; set; }
        
        /// <summary>
        /// Identifiant unique de la question.
        /// </summary>
        [DataMember]
        public int QuestionId { get; set; }

        /// <summary>
        /// Si l'usager a répondu à la question, contient la réponse. Sinon, contient le
        /// texte de la question (le libellé).
        /// </summary>
        [DataMember]
        public String Text { get; set; }
    }
}
