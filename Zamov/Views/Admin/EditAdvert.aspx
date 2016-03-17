<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Zamov.Models.Advert>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>������� - <%=Html.ResourceString("Edit")%></h2>


    <% using (Html.BeginForm("EditAdvert", "Admin", FormMethod.Post, new { enctype = "multipart/form-data" }))
       {%>

        <fieldset>
            <legend>������</legend>
            
                
                <%= Html.Hidden("Id", Model.Id) %>
            
            <p>
                <%=Html.ResourceString("ActiveM")%>:
                <%= Html.CheckBox("IsActive", Model.IsActive) %>
            </p>
            <p>
                <%if (!string.IsNullOrEmpty(Model.ImageSource))
                {%>
                <%=Html.RegisterFlashScript(Model.ImageSource, "b_" + Model.Id, (BannerPosition)Model.Position)%>
                <%
                }%>
                <br />
                ����:<input type="file" name="banner" />
            </p>
            <p>
                <input type="submit" value="���������" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ResourceActionLink("BackToList", "Advert") %>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="includes" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>

