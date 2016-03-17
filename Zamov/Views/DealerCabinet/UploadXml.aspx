<%@ Page Title="" Language="C#" MasterPageFile="~/Views/DealerCabinet/Cabinet.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% if (TempData["message"] != null)
       {%>
    <div class="statusMessage"><%=Html.Encode(TempData["message"])%></div>
    <%
       }%>
    <h2>
        Upload Xml</h2>        
    <div id="importProducts" style="padding: 20px; text-align: center;">
        <%using (Html.BeginForm("UploadXml", "DealerCabinet", FormMethod.Post, new { id="xmlForm", enctype = "multipart/form-data" }))
          { %>
        <input type="file" name="xml" id="xml" />
        <input type="submit"  value="Загрузить"/>
        <%} %>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>
