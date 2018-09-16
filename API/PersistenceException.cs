using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace USherbrooke.ServiceModel.Sondage
{
    /// <summary>
    /// Cette exception est lancée lorsqu'une transaction avec le médium de stockage permanent (base de données, par exemple) a échoué.
    /// </summary>
    [Serializable()]
    class PersistenceException : Exception
    {
        public PersistenceException() : base()
        {
        }
        public PersistenceException(string message) : base(message)
        {
        }
        public PersistenceException(string message, Exception inner) : base(message, inner)
        {
        }

        // Needed for serialization.
        protected PersistenceException(SerializationInfo info, StreamingContext context) : base(info, context)
       {
       }
    }
}
