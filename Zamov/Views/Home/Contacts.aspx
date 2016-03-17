<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%=Html.ResourceString("Contacts")%></h2>
<br />    
    <% 
        Response.Write(ApplicationData.ContactsHeader);
    %>

    <% using (Html.BeginForm())
       { %>
       
    <%= Html.ValidationSummary() %>
    <table>
    <tr>
        <td><%=Html.ResourceString("YourName") %>:</td>
        <td><%=Html.TextBox("userName")%></td>
    </tr>
    <tr>
        <td><%=Html.ResourceString("MessageSubj") %>:</td>
        <td><%=Html.TextBox("messageSubj")%></td>
    </tr>
    <tr>
        <td><%=Html.ResourceString("MessageBody")%>:</td>
        <td><%=Html.TextArea("messageBody", new { cols = "17", rows = "5" })%>
        <%= Html.ValidationMessage("messageBody", "*", new { @class = "validationError" })%>
        </td>
    </tr>
    <tr>
        <td>Email:</td>
        <td>
        <%=Html.TextBox("email")%>
        <%= Html.ValidationMessage("email", "*", new { @class="validationError" })%>
        </td>
    </tr>
    <tr>
        <td><%=Html.ResourceString("Phone")%>:</td>
        <td><%=Html.TextBox("phone")%></td>
    </tr>
    </table>
    <input type="submit" value="<%=Html.ResourceString("Send")%>" />
<%} %>

<table></table>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
</asp:Content>
