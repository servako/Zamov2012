<%@ Page Title="" Language="C#" MasterPageFile="~/Views/DealerCabinet/Cabinet.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var newItems = {};
        var updatedItems = {};

        $(function() {
            $("#tabs").tabs();
            $(".productDescriptionContainer")
                .dialog({
                    autoOpen: false,
                    width: 700,
                    height: 530,
                    minHeight: 360,
                    resizable: false,
                    modal: true,
                    buttons: { "Ok": closeDescriptionDialog }
                });

        })
        
        function closeDescriptionDialog() {
            $(".productDescriptionContainer").dialog('close');
        }

        function openDescriptionDialog(id) {
            $("#" + id).dialog('open');
            $("#" + id).dialog('open').css("height", 425);
            $("#" + id).dialog('option', 'height', 530);
            $("#" + id).dialog('option', 'position', 'center');
        }

        function checkAll(className, el) {
            if (el.checked)
                $("." + className).attr("checked", true);
            else {
                $("." + className).attr("checked", false); 
            }
        }

        function verifyGroupSaving() {
            if ($get("groupItems").value) {
                return true;
            }
            else {
                $get("groupItems").style.color = "red";
                return false;
            }
                
        }
    </script>
<%
    Dictionary<string, Dictionary<string, string>> updatedItems = (Dictionary<string, Dictionary<string, string>>)ViewData["updatedItems"];
    Dictionary<string, Dictionary<string, string>> newItems = (Dictionary<string, Dictionary<string, string>>)ViewData["newItems"];
%>
    
    <h2><%= Html.ResourceString("ImportedProducts") %></h2>
  <%--  <input style="padding:10px; margin-top:10px;" type="button" class="ui-state-default ui-corner-all" value="<%= Html.ResourceString("Save") %>" onclick="saveImports()" />--%>
    <div id="tabs">
        <ul>
            <li>
                <a href="#tabs-1"><%= Html.ResourceString("NewProducts")%></a>
            </li>
            <li>
                <a href="#tabs-2"><%= Html.ResourceString("UpdatedProducts")%></a>
            </li>
        </ul>
        <div id="tabs-1">
            <%using(Html.BeginForm("SaveSelectedTo", "DealerCabinet")){ %>
            <div>
                <%= Html.ResourceString("CheckedSaveTo")%>
                <%= Html.DropDownList("groupItems")%>
                <input type="submit" value=">" onclick="return verifyGroupSaving()" style="padding:0 5px;" />
            </div>
            <table class="importedTable">
                <tr>
                    <td class="check">
                        <input type="checkbox" onclick="checkAll('newCheck', this)" />
                    </td>
                    <td align="center"><%= Html.ResourceString("PartNumber")%></td>
                    <td align="center"><%= Html.ResourceString("Name")%></td>
                    <td align="center"><%= Html.ResourceString("Price")%></td>
                    <td align="center"><%= Html.ResourceString("Unit")%></td>
                    <td align="center"><%= Html.ResourceString("Description")%></td>
                </tr>
                <%
                foreach (var item in newItems)
                    Html.RenderAction<Zamov.Controllers.DealerCabinetController>(ac => ac.ImportedProduct(item.Value, true));
                %>
            </table>
            <%} %>
        </div>
        <div id="tabs-2">
            <% using (Html.BeginForm("MoveToNew", "DealerCabinet"))
               {%>
            <input type="submit" value="<%= Html.ResourceString("MoveCheckedToNew") %>" />
            <table class="importedTable">
                <tr>
                    <td align="center" class="check">
                        <input type="checkbox" onclick="checkAll('modifiedCheck', this)" />
                    </td>
                    <td align="center"><%= Html.ResourceString("PartNumber")%></td>
                    <td align="center"><%= Html.ResourceString("Name")%></td>
                    <td align="center"><%= Html.ResourceString("Price")%></td>
                    <td align="center"><%= Html.ResourceString("Unit")%></td>
                    <td align="center"><%= Html.ResourceString("Description")%></td>
                    <td align="center"><%= Html.ResourceString("Image")%></td>
                </tr>
                <%
                foreach (var item in updatedItems)
                    Html.RenderAction<Zamov.Controllers.DealerCabinetController>(ac => ac.ImportedProduct(item.Value, false));
                %>
            </table>
            <%} %>
            <% using(Html.BeginForm("SaveUpdated", "DealerCabinet")){ %>
                <%= Html.Hidden("groupId", ViewData["groupId"]) %>
                <input type="submit" value="<%= Html.ResourceString("Save") %>" />
            <%} %>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("fckeditorapi.js") %>
    <%= Html.RegisterJS("fckeditor.js") %>
    <%= Html.RegisterJS("fcktools.js") %>
    <%= Html.RegisterJS("jquery.FCKeditor.js") %>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <style type="text/css">
        .ui-tabs-nav { height:2.4em; }
    </style>
    
    <script type="text/javascript">
        $(function() {
            $(".fancy").fancybox();
        })
    </script>
</asp:Content>
