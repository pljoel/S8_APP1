using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace USherbrooke.ServiceModel.Sondage
{
    /// <summary>
    /// Cette classe définit un sondage.
    /// </summary>
    [DataContract]
    public class Poll
    {
        /// <summary>
        /// L'identifiant du sondage.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// La description du sondage.
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}
