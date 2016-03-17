<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    
    
    <%= Html.RegisterJS("fckeditorapi.js") %>
    <%= Html.RegisterJS("fckeditor.js") %>
    <%= Html.RegisterJS("fcktools.js") %>
    <%= Html.RegisterJS("jquery.FCKeditor.js") %>
    
    <script type="text/javascript">
        $(function() {
            $.fck.config = { path: '<%= VirtualPathUtility.ToAbsolute("~/Controls/fckeditor/") %>', height: 300, config: { SkinPath: "skins/office2003/", DefaultLanguage: "RU", AutoDetectLanguage: false, HtmlEncodeOutput: true} };
        });
    </script>
    
    
    <script type="text/javascript">
        var enables = {};
        var update = {};
        function updateEnables(check, id) {
            if (check.checked) {
                enables[id] = true;
            }
            else {
                enables[id] = false;
            }
        }

        function collectCategoryChanges() {
            collectChanges(update, 'updateCategory');
            var enablities = $get("enablities");
            enablities.value = Sys.Serialization.JavaScriptSerializer.serialize(enables);
            return true;
        }
    
        function insertCategory(link, parentId) {
            var pos = $(link).offset();
            if (parentId >= 0) {
                $("#parentId").val(parentId);
                pos.left = pos.left - $("#insertCategory").width();
            }
            else {
                pos.left = pos.left + $(link).width();
            }
            $("#insertCategory").css("top", pos.top).css("left", pos.left).slideDown("fast");
            window.setTimeout(bindBodyClick, 50);
        }

        function bindBodyClick() {
            $("body").click(bodyClick);
        }

        function bodyClick() {
            $("#insertCategory").slideUp("fast");
            $("body").unbind("click", bodyClick);
        }

        $(function() {
            $("#insertCategory").click(function(e) { e.stopPropagation(); if (window.event) { window.event.cancelBubbling = true; } });
            $("#imagePopUp").dialog({ autoOpen: false, width: 440, height: 360, minHeight: 360, resizable: false });
        }
        );

        function openImageIframe(categoryId) {
            $("#updateImageBox").attr("src", "/Admin/UpdateCategoryImage/" + categoryId);
            $("#imagePopUp").dialog('open').css("height", 300);

            $('#imagePopUp').dialog('option', 'height', 360);
            $('#imagePopUp').dialog('option', 'position', 'center');
            $('#imagePopUp').css('height', 'auto');
        }

        function closeImageDialog() {
            $("#imagePopUp").dialog('close');
        }

//        $(function() {
//            $(".cDescription").fancybox(
//            {
//                hideOnContentClick: false,
//                callbackOnShow: function() {
//                $('textarea#uaDescription, textarea#ruDescription').fck({ toolbar: "Common", height: 300 });
//                }
//            });
//        })
    </script>
    <div title="<%= Html.ResourceString("Image") %>" id="imagePopUp" style="display: block; height: 300px;">
        <iframe id="updateImageBox" frameborder="0" hidefocus="true" style="width: 400px;
            height: 299px; background: transparent;"></iframe>
    </div>
    <h2>
        <%= Html.ResourceString("Categories") %></h2>
    <% using (Html.BeginForm("UpdateCategories", "Admin", FormMethod.Post)){ %>
    <% Html.RenderAction<Zamov.Controllers.AdminController>(a => a.CategoriesList(null, 0)); %>
    <%= Html.Hidden("enablities") %>
    <%= Html.Hidden("updateCategory") %>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" onclick="return collectCategoryChanges();" />
    <%} %>
    
    <a style="text-decoration:underline; cursor:pointer" onclick="insertCategory(this, <%= int.MinValue %>)">
        <%= Html.ResourceString("AddCategory") %>
    </a>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="bodyTop">
    <div id="insertCategory" class="greyBorderBox popUpBox">
        <% using (Html.BeginForm("InsertCategory", "Admin")){ %>
        <table class="adminTable">
            <tr>
                <th>
                    <%= Html.ResourceString("Ukr") %>
                </th>
                <th>
                    <%= Html.ResourceString("Rus") %>
                </th>
                <th>
                    <%= Html.ResourceString("Show") %>
                </th>
                <th>
                    <%= Html.ResourceString("IndexNumberOutput")%>
                </th>
            </tr>
            <tr>
                <td>
                    <%= Html.TextBox("categoryUkrName")%>
                </td>
                <td>
                    <%= Html.TextBox("categoryRusName")%>
                </td>
                <td align="center">
                    <%= Html.CheckBox("categoryEnabled", true) %>
                </td>
                <td>
                    <%= Html.TextBox("categoryIndex", 1, new { style = "width:50px; font-size:10px; text-align:center", onkeyup = "validateQuantity(this)" })%>                              
                </td>
            </tr>
        </table>
        <%= Html.Hidden("parentId", int.MinValue) %>
        <input type="submit" value="<%= Html.ResourceString("Add") %>" />
        <%} %>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="includes">
</asp:Content>