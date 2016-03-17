<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>

<%
    bool isNew = (bool)ViewData["isNew"];
    string checkClass = (isNew) ? "newCheck" : "modifiedCheck";
    string divId = (isNew) ? "newProductDescription_" + ViewData["id"] : "updatedProductDescription_" + ViewData["id"];
    string fckUkId = (isNew) ? "newUkDescription_" + ViewData["id"] : "updatedUkDescription_" + ViewData["id"];
    string fckRuId = (isNew) ? "newRuDescription_" + ViewData["id"] : "updatedRuDescription_" + ViewData["id"];
%>

<tr>
    <td class="check">
        <%= Html.CheckBox("check_" + ViewData["id"], false, new { @class = checkClass })%>
    </td>
    <td>
        <%= ViewData["partNumber"] %>
    </td>
    <td>
        <%= ViewData["name"] %>
    </td>
    <td>
        <%= ViewData["price"] %>
    </td>
    <td>
        <%= ViewData["unit"] %>
    </td>
    <td align="center">
        <a href="javascript:openDescriptionDialog('<%= divId %>')" class="productDescriptionLink">
            i
        </a>
        <div class="productDescriptionContainer" title="<%= Html.ResourceString("Description") %>" id="<%= divId %>">
            <table>
                <tr class="adminTable">
                    <td align="center"><%= Html.ResourceString("Ukr")%></td>
                    <td align="center"><%= Html.ResourceString("Rus")%></td>
                </tr>
                <tr>
                    <td style="width:330px;">
                        <%= ViewData["ukDescription"] %>
                    </td>
                    <td style="width:330px;">
                        <%= ViewData["ruDescription"] %>
                    </td>
                </tr>
            </table>
        </div>
    </td>
    <td>
        <%if (!string.IsNullOrEmpty((string)ViewData["imageUrl"])){ %>
            <a class="fancy" href="<%= ViewData["imageUrl"] %>"><%= Html.ResourceString("Image") %></a>
        <%} %>
    </td>
</tr>