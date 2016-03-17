<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>DealerCityMappings</title>
    <script type="text/javascript">
        function updateCityMappings() {
            document.getElementById("form1").submit();
        }

        function cityChecked(el) {
            var checked = el.checked;
            var checks = document.getElementsByTagName("input");
            var length = checks.length;
            for (var i = 0; i < length;  i++) {
                checks[i].checked = false;
            }
            el.checked = checked;
        }
    </script>
</head>
<body>
    <%
        bool checkAll = (bool)ViewData["checkAllCity"];
        using (Html.BeginForm("DealerCityMappings", "DealerCabinet", FormMethod.Post, new { id = "form1" }))
        {
            List<SelectListItem> items = (List<SelectListItem>)ViewData["items"];
            foreach (var item in items)
            {
                if (checkAll)
                {
                    Response.Write(Html.CheckBox(item.Value, item.Selected));
                }
                else
                {
                    Response.Write(Html.CheckBox(item.Value, item.Selected, new { onclick = "cityChecked(this)" }));
                }
                Response.Write(item.Text);
                Response.Write("&nbsp;&nbsp;");
            }
        }
    %>
</body>
</html>
