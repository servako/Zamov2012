using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zamov.Models;
using System.Web.Caching;
using Zamov.Controllers;
using System.Data.Common;
using System.Data.EntityClient;

public static class ApplicationData
{
    public static int PageSize
    {
        get { return 5; }
    }

    public static string ContactsHeader
    {
        get { return GetApplicationData("ContactsHeader", SystemSettings.CurrentLanguage); }
    }

    public static string Agreement
    {
        get { return GetAgreement(SystemSettings.CurrentLanguage); }
    }

    public static string StartText
    {
        get { return GetApplicationData("StartText", SystemSettings.CurrentLanguage); }
    }

    public static string FeedbackEmail
    {
        get { return GetApplicationData("FeedbackEmail", "All"); }
        set
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("All", value);
            UpdateApplicationData("FeedbackEmail", values);
        }
    }

    public static string ZamovSmtpHost
    {
        get { return GetApplicationData("smtp", "All"); }
        set
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("All", value);
            UpdateApplicationData("smtp", values);
        }
    }

    public static string SubCategoryText
    {
        get { return GetApplicationData("SubCategoryText", SystemSettings.CurrentLanguage); }
    }

    public static void UpdateAgreement(Dictionary<string, string> values)
    {
        UpdateApplicationData("Agreement", values);
    }

    public static void UpdateContactsHeader(Dictionary<string, string> values)
    {
        UpdateApplicationData("ContactsHeader", values);
    }

    public static void UpdateStartText(Dictionary<string, string> values)
    {
        UpdateApplicationData("StartText", values);
    }

    public static void UpdateAboutUsALittle(Dictionary<string, string> values)
    {
        UpdateApplicationData("AboutUsALittle", values);
    }

    public static void UpdateSubCategoryText(Dictionary<string, string> values)
    {
        UpdateApplicationData("SubCategoryText", values);
    }

    public static string GetStartText(string language)
    {
        return GetApplicationData("StartText", language);
    }

    public static string GetAboutUsALittle(string language)
    {
        return GetApplicationData("AboutUsALittle", language);
    }

    public static string GetAgreement(string language)
    {
        return GetApplicationData("Agreement", language);
    }

    public static string GetContactsHeader(string language)
    {
        return GetApplicationData("ContactsHeader", language);
    }

    public static string GetSubCategoryText(string language)
    {
        return GetApplicationData("SubCategoryText", language);
    }

    private static string GetApplicationData(string name, string language)
    {
        if (HttpRuntime.Cache["ApplicationData_" + name + "_" + language] == null)
        {
            using (SettingsStorage context = new SettingsStorage())
            {
                string result = (from data in context.ApplicationSettings
                                 where data.Name == name
                                 && data.Language == language
                                 select data.Value).First();
                HttpRuntime.Cache.Insert("ApplicationData_" + name + "_" + language, result, null, DateTime.Now.AddHours(3), Cache.NoSlidingExpiration);
            }
        }
        return HttpRuntime.Cache["ApplicationData_" + name + "_" + language].ToString();
    }

    private static void UpdateApplicationData(string name, Dictionary<string, string> values)
    {
        HttpRuntime.Cache.Remove("ApplicationData_" + name + "_uk-UA");
        HttpRuntime.Cache.Remove("ApplicationData_" + name + "_ru-RU");
        using (SettingsStorage context = new SettingsStorage())
        {
            var data = (from appData in context.ApplicationSettings
                        where appData.Name == name
                        select appData);
            foreach (string key in values.Keys)
                foreach (var item in data)
                    if (item.Language == key)
                        item.Value = values[key];
            context.SaveChanges();
        }
    }
}
