<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Seo/Seo.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Seo>>" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using(Html.BeginForm()){ %>
    <fieldset>
        <p>
            <%= Html.Hidden("lang", ViewData["lng"]) %>
        </p>
        <p>
            Url:<br />
            <%= Html.TextBox("url") %>
        </p>
        <p>
            Title:<br />
            <%= Html.TextArea("title")%>
        </p>
        <p>
            Keywords:<br />
            <%= Html.TextBox("keywords") %>
        </p>
        <p>
            Description:<br />
            <%= Html.TextArea("description")%>
        </p>
        <input type="submit" value="<%= Html.ResourceString("Add") %>" />
    </fieldset>
    
    <%} %>
    <% foreach (var item in Model) { %>
        <% using(Html.BeginForm()){ %>
        <fieldset>
            <p>
                <%= Html.Hidden("id", item.Id) %>
            </p>
            <p>
                <%= Html.Hidden("lang", item.Language) %>
            </p>
            <p>
                Url:<br />
                <%= Html.TextBox("url", item.Url) %>
            </p>
            <p>
                Title:<br />
                <%= Html.TextArea("title", item.Title)%>
            </p>
            <p>
                Keywords:<br />
                <%= Html.TextBox("keywords", item.Keywords) %>
            </p>
            <p>
                Description:<br />
                <%= Html.TextArea("description", item.Description) %>
            </p>
            <input type="submit" value="<%= Html.ResourceString("Save") %>" />
            <%= Html.ResourceActionLink("Delete", "DeleteSeo", new {id= item.Id}) %>
        </fieldset>
        <%} %>
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <style type="text/css">
        p input{width:99%;}
        p textarea{width:99%;}
    </style>
</asp:Content>