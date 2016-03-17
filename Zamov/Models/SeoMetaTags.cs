using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class SeoMetaTags
    {
        public string Title;
        public string Keywords;
        public string Description;
        public string MetaTags;
        /// <summary>
        ///  Если необходимо, можно сюда прописать дополнительную инфу, 
        ///  котороя будет добавлена после сео сущности(Dealers, Categories, Groups ...) прописанной ручками.
        ///  Например для диллеров можно указать имя группы или подгруппы.
        /// </summary>
        public string AdditionInfo;
    }
}