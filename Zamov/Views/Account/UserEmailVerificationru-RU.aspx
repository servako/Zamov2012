<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Поздравляем!</h2>
    Регистрация практически завершена.<br />
    Пожалуйста, проверьте Ваш почтовый ящик <strong><%= ViewData["email"] %></strong> и пройдите по ссылке, которая указана в письме. <br />
    В большинстве случаев письма приходят в течение одной минуты, но иногда для этого требуется до 10 минут.
    
</asp:Content>
<asp:Content ContentPlaceHolderID="includes" runat="server">
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
</asp:Content>