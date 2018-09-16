using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace USherbrooke.ServiceModel.Sondage
{
    /// <summary>
    /// Exception lancée lorsque l'identifiant fourni à la couche de persistence n'est pas valide.
    /// </summary>
    [Serializable()]
    class InvalidIdException : PersistenceException
    {
        public InvalidIdException() 
            : base()
        {
        }
        public InvalidIdException(string message) 
            : base(message)
        {
        }
        public InvalidIdException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        public InvalidIdException(string id, string message)
            : base(message)
        {
            Id = id;
        }
        public InvalidIdException(string id, string message, Exception inner)
            : base(message, inner)
        {
            Id = id;
        }


        // Needed for serialization.
        protected InvalidIdException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// L'identifiant fautif.
        /// </summary>
        public string Id { get; private set; }
    }
}
