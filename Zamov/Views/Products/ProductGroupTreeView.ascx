<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<GroupResentation>>" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Controllers" %>
<div>
    <%
        int groupId = (ViewData["groupId"] != null) ? Convert.ToInt32(ViewData["groupId"]) : int.MinValue; %>
    <div class="menu">
        <div class="menuHead">
            <div>
                <%= Html.ResourceString("Groups")%>
            </div>
        </div>
        <div class="menuBody">
        <%= Html.TreeView("groups", Model, sg => sg.Children,
                                                    sg => Html.ActionLink(sg.Name, "ShowProducts", "Products", new { idCity = SystemSettings.CityId, id = sg.Id }, new
                                                    {
                                                        @class = sg.Id == groupId ? "active" : string.Empty
                                                    }).ToHtmlString())%>
        </div>
    </div>
    <script type="text/javascript">
        $(function() {
            window.setTimeout(expandCurrent, 1000);

        });

        function expandCurrent() {
            $("a.active").parent("div").prev("div.hitarea").click();
            $("a.active").parents("li").find("div.hitarea").click();
            //$("a.active").parents("ul").prevAll("div.hitarea").click();
        }
    </script>
</div>
