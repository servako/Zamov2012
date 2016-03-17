using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects.DataClasses;

namespace Zamov.Models
{
    public class EntityTranslationPair<T> where T : EntityObject
    {
        public T Entity { get; set; }
        public Translation Translation { get; set; }
        public string Language { get; set; }
    }
}
