<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<Dictionary<string, SeoMetaTags>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        int seoEntityType = (int)ViewData["seoEntityType"];
        int id = 0;
        string name = "";
        string logoUrl = VirtualPathUtility.ToAbsolute("~/Content/img/noLogo");
        
        if (seoEntityType == (int)SeoEntityType.Dealers)
        {
            var dealer = (Dealer) ViewData["dealer"];
            name = dealer.GetName(SystemSettings.CurrentLanguage);
            id = dealer.Id;
            if (dealer.LogoImage.Length > 0)
                logoUrl = Url.Action("ShowLogo", "Image", new {id = dealer.Id});
        }
        if (seoEntityType == (int)SeoEntityType.Categories)
        {
            var category = (Category) ViewData["category"];
            name = category.Name;
            if (category.Images.Count > 0)
                logoUrl = Url.Action("CategoryImageByCategoryId", "Image", new { id = category.Id });
        }
        if (seoEntityType == (int)SeoEntityType.Groups)
        {
            var group = (GroupsAdditional)ViewData["group"];
            name = group.gr_Name;
        }
%>
    <h2>
            <%=name%>
            <br />
            <%=Html.Image(logoUrl)%><br />
    </h2>
    <% using (Html.BeginForm())
       { %>
    <%= Html.Hidden("id", id)%>
    <%= Html.Hidden("seoEntityType", seoEntityType)%>
    <table class="seoTable">
        <tr>
            <th>
                Тип
            </th>
            <th>
                <%=Html.ResourceString("Ukr")%>
            </th>
            <th>
                <%=Html.ResourceString("Rus")%>
            </th>
        </tr>
        <tr>
            <td>
                <span>Title</span><br/>
                <span>Keywords</span><br/>
                <span>Description</span><br/>
                <span>MetaTags</span>
            </td>
            <td>
                <% String local = "uk-UA"; 
                   Boolean existLanguage = Model.ContainsKey(local); %>
                <%=Html.TextBox("Title_"+ local, existLanguage ? Model[local].Title : "")%><br/>
                <%=Html.TextBox("Keywords_" + local, existLanguage ? Model[local].Keywords : "")%><br/>
                <%=Html.TextBox("Description_"+ local, existLanguage ? Model[local].Description : "")%><br/>
                <%=Html.TextArea("MetaTags_" + local, existLanguage ? Model[local].MetaTags : "")%>
            </td>
            <td>
                <% local = "ru-RU"; 
                   existLanguage = Model.ContainsKey(local); %>
                <%=Html.TextBox("Title_"+ local, existLanguage ? Model[local].Title : "")%><br/>
                <%=Html.TextBox("Keywords_" + local, existLanguage ? Model[local].Keywords : "")%><br/>
                <%=Html.TextBox("Description_"+ local, existLanguage ? Model[local].Description : "")%><br/>
                <%=Html.TextArea("MetaTags_" + local, existLanguage ? Model[local].MetaTags : "")%>
            </td>
        </tr>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%} %>   
    <div>
        <%=Html.ActionLink("Назад", seoEntityType == (int)SeoEntityType.Dealers ? "Dealers" :
                    seoEntityType == (int)SeoEntityType.Categories ? "Categories" : "GroupsAddit")%>
    </div>
</asp:Content>
