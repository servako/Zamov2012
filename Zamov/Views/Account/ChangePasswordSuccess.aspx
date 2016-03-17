<%@Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>


<asp:Content ID="changePasswordSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%=Html.ResourceString("PasswordChange")%></h2>
    <p>
        <%=Html.ResourceString("PasswordHasBeenChanged")%>
    </p>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="includes" runat="server">
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
</asp:Content>