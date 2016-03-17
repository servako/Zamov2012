<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.DealerPresentation>>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>RemoveDealers</title>
</head>
<body>
    <table>
        <tr>
            <th></th>

            <th>
                Name
            </th>
            <th>
                Включен
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.ActionLink("Удалить", "RemoveDealer", new { id = item.Id }, new { onclick="return confirm('Точно удалить дилера \""+item.Name+"\"?')"})%> 
            </td>
            <td>
                <%= Html.Encode(item.Name) %>
            </td>
            <td>
                <%= Html.CheckBox("enabled", item.Enabled, new { disabled = "disabled"})%>
            </td>
        </tr>
    
    <% } %>

    </table>

</body>
</html>

