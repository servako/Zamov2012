<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%using (Html.BeginForm())
      {%>
      <%= Html.DropDownList("dealerId") %><br />
      <%= Html.DropDownList("cityId") %><br />
      <input type="submit" value="Копировать" />
    <%} %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
