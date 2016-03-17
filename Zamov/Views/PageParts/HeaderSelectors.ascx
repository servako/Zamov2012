<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<script type="text/javascript">


    $(function() {
        $("#currentCategory").click(function() { $(this).parent().css({ border: "none", padding: "2px" }); });
    })
    function cityChanged() {
        var currentCity = document.getElementById("currentCity");
        var cityId = currentCity[currentCity.selectedIndex].value;

        var webRequest = Sys.Net.WebServiceProxy.invoke(
            "/Services/Tools.asmx",
            "GetCityCategories",
            false,
            { id: cityId },
            function(response) {
                var currentCategory = $get("currentCategory");
                currentCategory.options.length = 0;
                var firsOption = document.createElement("option");
                firsOption.value = -1;
                firsOption.text = '--<%= Html.ResourceString("SelectCategory") %>--';
                try {
                    currentCategory.add(firsOption, null);
                }
                catch (ex) {
                    currentCategory.add(firsOption);
                }
                for (var i in response) {
                    var option = document.createElement("option");
                    option.text = response[i].Text;
                    option.value = response[i].Value;
                    option.selected = response[i].Selected;
                    try {
                        currentCategory.add(option, null);
                    }
                    catch (ex) {
                        currentCategory.add(option);
                    }
                }
            },
            failureCallback);
        }

        function checkSelectedCategory() {
            debugger;
//            if ($get("currentCategory").value && $get("currentCategory").value > 0) {
                //                window.location.href = "/Dealers/" + $get("currentCategory").value + "/?cityId=" + $get("currentCity").value;
            if ($get("currentCategory").value > "") {
                window.location.href = $get("currentCategory").value + "/?cityId=" + $get("currentCity").value;
            }
            else {
                $("#currentCategory").parent().css("border", "1px solid red").css("padding", "1px;");
                return false;
            }
        }

        function keydownSelectCategory(form) {
            var key = window.event.keyCode;
            if (key == 13) {
                form.submit();
            }
        }
</script>
<table>
    <tr>
        <td>
            <span><%= Html.ResourceString("City") %></span><br />
            <%= Html.DropDownList("currentCity", (List<SelectListItem>)ViewData["citiesList"], new { onchange = "cityChanged()", @class = "currentCity" })%>
        </td>
        <td>
            <span><%= Html.ResourceString("Category") %></span><br />
            <div style="padding:2px;">
                <%= Html.DropDownList("currentCategory", (List<SelectListItem>)ViewData["categoriesList"], new { onkeydown = "keydownSelectCategory(this.form)", @class = "currentCategory" })%>
            </div>
        </td>
        <td>
            <input type="button" onclick="return checkSelectedCategory()" value=">" class="headerSelect" style="margin-top:14px;" />
        </td>
    </tr>
</table>
