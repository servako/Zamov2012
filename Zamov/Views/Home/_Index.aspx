<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Controllers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%= Html.ResourceString("PageTitle") %>
    </title>
    <link rel="shortcut icon" type="image/ico" href="/favicon.ico" />
     <meta name="KeyWords" content="Замов, заказ пиццы, заказ суши, заказ обеда, доставка пиццы, доставка еды, доставка суши, заказ подарков, подарок заказать, заказ онлайн, онлайн заказ" />

     <meta name="Description" content="Zamov - Первый в Украине супермаркет онлайн заказа пиццы, суши, подарков, електроники, автотоваров." />

    <link href="../../Content/Start.css" rel="stylesheet" type="text/css" />
    <%= Html.RegisterJS("MicrosoftAjax.js")%>
    <%= Html.RegisterJS("MicrosoftMvcAjax.js")%>
    <%= Html.RegisterJS("jquery.js")%>
    <%= Html.RegisterJS("common.js")%>
    <%= Html.RegisterJS("jquery.corner.js")%>

    <script type="text/javascript">
        $(function() {
            $("#header").corner("top");
            $("#mainMenu .outer .inner").corner("bottom").parent().css('padding', '1px').corner("bottom");
        }
        )
    </script>

</head>
<body>
    <map id="logoLink">
        <area shape="rect" coords="0,40,233, 90" href="/" />
    </map>
    <div class="shader">
    </div>
    <div id="header">
        <div id="headerRight">
            <div id="languageSelector">
                <% Html.RenderPartial("LanguageSelector"); %>
            </div>
        </div>
        <div id="masterLogin">
            <% Html.RenderPartial("LogonStatus"); %>
            <%if (Roles.IsUserInRole("Administrators")) Html.RenderPartial("AdminOrdersInfo"); %>
            <%if (Roles.IsUserInRole("Dealers")) Html.RenderPartial("DealerOrdersInfo"); %>
        </div>
        <center>
            <div id="logo">
                <%= Html.Image("~/Content/img/logo" + SystemSettings.CurrentDomain + ".jpg", "Zamov.net", new { usemap = "#logoLink", style = "border:none" })%>
            </div>
        </center>
    </div>
    <div id="mainMenu">
        <div class="outer greyBg">
            <div class="inner whiteBg">
                <table cellspacing="0" cellspacing="0">
                    <tr>
                        <td valign="top">
                            <%=Html.ResourceString("Search")%>:
                        </td>
                        <td valign="top">
                            <%using (Html.BeginForm("SearchProduct", "Search", FormMethod.Post))
                              {%><%=Html.TextBox("searchContext", null, new { style="height:13px; font-size:10px;"})%>
                            <%
                                } %>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="outer greyBg">
            <div class="inner whiteBg">
                <% Html.RenderAction<PagePartsController>(ac => ac.MainMenu()); %>
            </div>
        </div>
    </div>
    <div id="content">
        <div id="headerSelectors">
            <center>
                <% Html.RenderAction<PagePartsController>(ppc => ppc.HeaderSelectors()); %>
            </center>
        </div>
    </div>
    <div class="startText">
        <%= ApplicationData.StartText %>
    </div>

    <script type="text/javascript">
        var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
        document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
    </script>

    <script type="text/javascript">
        try {
            var pageTracker = _gat._getTracker("UA-10840182-1");
            pageTracker._trackPageview();
        } catch (err) { }
    </script>

</body>
</html>
