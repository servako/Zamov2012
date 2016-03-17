<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<List<CategoryPresentation>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>DealerCategoryMappings</title>
    <%= Html.RegisterJS("jquery.js") %>
    <%= Html.RegisterJS("jquery.treeview.js") %>
    <%= Html.RegisterCss("~/Content/jquery.treeview.css") %>
    <script type="text/javascript">
        $(function() {
            $("#categories input[type='checkbox']").click(nodeClick);
        });

        function nodeClick() {
            if (this.parentNode.className.indexOf("collapsable") > -1 && this.checked) {
                $("ul li input[type='checkbox']", this.parentNode).each(function() { this.checked = true });
            }
            else if (!this.checked) {
                var liElement = $(this).parents().filter(".collapsable").eq(0)[0];
                var liChildren = liElement.childNodes
                var len = liChildren.length;
                for (var i = 0; i < len; i++) {
                    if (liChildren[i].tagName && liChildren[i].tagName.toLowerCase() == "input" && liChildren[i].type.toLowerCase() == "checkbox") {
                        liChildren[i].checked = false;
                    }
                }
            }
        }
                    
        function updateCategoryMappings() {
            document.getElementById("form1").submit();
        }
    </script>
</head>
<body>
    <%
        using (Html.BeginForm("DealerCategoryMappings", "DealerCabinet", FormMethod.Post, new { id = "form1" }))
        {%>
            <%= Html.TreeView("categories", Model, c=>c.Children, c=>Html.CheckBox(c.Id.ToString(), c.Selected) + c.Name, true) %>
      <%}
    %>
</body>
</html>
