using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zamov.Controllers;

namespace Zamov.Models
{
    public enum Statuses
    {
        New = 0,
        Accepted = 1,
        Complited = 2,
        Canceled = 3
    }

    public class Status
    {/*
        public static string GetStatus(int status)
        {
            return SystemSettings.CurrentLanguage == "uk-UA" ? status_UA[status] : status_RU[status];
        }

        private static readonly string[] status_RU ={"новый","принятый","выполненный","отклонённый"};
        private static readonly string[] status_UA = {"новий", "прийнятий", "виконаний", "відхилений"};*/
    }
}
