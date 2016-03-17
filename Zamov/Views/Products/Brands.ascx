<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Zamov.Models.Brands>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<script type="text/javascript">
    $(function () {
        $('#checkall').click(function () {
            $(this).parents('#brandcheckbox').find(':checkbox').attr('checked', this.checked);
        });
    });
</script>
<div class="menu">
    <div class="menuHead">
        <div>
            <%=Html.ResourceString("Brands")%>
        </div>
    </div>
    <div class="menuBody">
        <%
            if (Model.Count > 0)
            {
        %>
        <%=Html.Hidden("groupId", (int?)ViewData["groupId"])%>
        <div id="brandcheckbox">
            <div style="display: block; margin-bottom: 7px; margin-top: 7px;">
                <input type="checkbox" id="checkall" />
                <span style="padding-left:10px"><%=Html.ResourceString("ChoiceAll")%></span>
            </div>
            <%
                for (int i = 0; i < Model.Count(); i++)
                {
            %>
            <%=Html.CheckBox(string.Format("brands[{0}].IsCheck", i), Model[i].IsCheck)%>
            <%=Html.Hidden(string.Format("brands[{0}].brandID", i), Model[i].brandID)%>
            <span style="padding-left:10px">
            <%=Html.Hidden(string.Format("brands[{0}].brandName", i), Model[i].brandName)%>
            </span>
            <%=Html.Label(Model[i].brandName)%>
                        (<%=Html.Label(Model[i].BrandCount.ToString())%>)
            <br />
            <% if (i == 9 && Model.Count() == 10)
               {
            %>
            <%= Ajax.ActionLink(Html.ResourceString("ShowAllBrands"), "Brands", "Products",
					                new AjaxOptions
					                	{
											UpdateTargetId = "nextBrands",
					                		OnBegin = "begin",
					                		OnSuccess = "success",
					                		OnFailure = "failure"
										}, 
										new { @id = "showAllBrands" })%>
            <script type="text/javascript">
                function begin(args) {
                    $('#nextBrands').html('Loading!')
                }

                function success(data) {
                    $('#showAllBrands').remove();
                }

                function failure() {
                    alert("Could not retrieve brands.");
                }
            </script>
            <%
               }%>
            <%
            }
            %>
            <div id="nextBrands">
            </div>
        </div>
        <%
            }
        %>
    </div>
</div>
