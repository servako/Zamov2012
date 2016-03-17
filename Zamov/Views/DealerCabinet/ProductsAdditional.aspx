<%@ Page Title="" Language="C#" MasterPageFile="~/Views/DealerCabinet/Cabinet.Master" 
    Inherits="System.Web.Mvc.ViewPage<List<Zamov.Models.ProductPricePresentation>>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $("#importProductsFromXml")
                .dialog({
                        autoOpen: false,
                        width: 400,
                        resizable: false,
                        modal: true,
                        buttons: { '<%= Html.ResourceString("Upload") %>': function() { $get("xmlForm").submit(); } }
                    });
            $("#importProductsFromXls")
                .dialog({
                        autoOpen: false,
                        width: 400,
                        resizable: false,
                        modal: true,
                        buttons: { '<%= Html.ResourceString("Upload") %>': function () { $get("xlsForm").submit(); } }
                    });
            $("#productsHeader input[type='checkbox']").click(function() {
                $("." + this.id).attr("checked", this.checked);
            });
        });

        function groupSelected() {
        	$("#groupId option").each(function (i) {
        		if (this.selected)
        			location.href = "/DealerCabinet/ProductsAdditional/" + this.value;
        	}
            );
        }

        function uploadXml() {
            $("#importProductsFromXml").dialog('open');
        }

        function uploadXls() {
            $("#importProductsFromXls").dialog('open');
        }
    </script>
    
    <% if (TempData["error"] != null)
        {%>
    <div class="error"><%=Html.Encode(TempData["error"])%></div>
    <%
        }%>
    <% if (TempData["success"] != null)
        {%>
    <div class="success"><%=Html.Encode(TempData["success"])%></div>
    <%
        }%>

    <div id="importProductsFromXml" style="padding: 20px; text-align: center;">
        <%using (Html.BeginForm("UploadXml", "DealerCabinet", FormMethod.Post, new { id="xmlForm", enctype = "multipart/form-data" }))
          { %>
        <input type="file" name="xml" id="xml" accept="text/xml, application/xml"/>
        <%} %>
    </div>
    <div id="importProductsFromXls" style="padding: 20px; text-align: center;">
        <%using (Html.BeginForm("UploadXlsForProductsAdditional", "DealerCabinet", FormMethod.Post, new { id = "xlsForm", enctype = "multipart/form-data" }))
          { %>
        <input type="file" name="xls" id="xls" accept="application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" />
        <%} %>
    </div>
    <h2>
        <%= Html.ResourceString("Products") %></h2>
    <br />
    <% int idGroup = (int)ViewData["groupId"];%>
	<% var items = new List<SelectListItem>
                      {
                          new SelectListItem {Text = ResourcesHelper.GetResourceString("SelectGroup"), Value = "-2"},
                          new SelectListItem {Text = ResourcesHelper.GetResourceString("AllProducts"), Value = "-1"}
                      }; %>
    <%= Html.HierarchicalDropDown("groupId", (List<GroupResentation>)ViewData["groups"], g => g.Children, g => g.Name, g => g.Id.ToString(), g => g.Id == idGroup, new { onchange = "groupSelected()" }, items)%>
    &nbsp;&nbsp;
    <a href="javascript:uploadXml()">
        <%= Html.ResourceString("LoadProductsFromXml")%>
    </a>
    &nbsp;&nbsp;
    <a href="javascript:uploadXls()">
        <%= Html.ResourceString("LoadProductsFromXls")%>
    </a>
    
    <%
        if (Model != null && Model.Count > 0)
        { 
    %>
        <%using (Html.BeginForm("UpdateProductsAdditional", "DealerCabinet", System.Web.Mvc.FormMethod.Post))
      { %>
        <%= Html.Hidden("groupId") %>
    <table class="adminTable">
        <tr id="productsHeader">
			<th>
               ID
            </th>
            <th>
                <%= Html.ResourceString("Name")%>
            </th>
			<th style="width:50px;">
                <%= Html.ResourceString("Unit")%>
            </th>
			<th style="width:50px;">
                <%= Html.ResourceString("Quarantee")%>
            </th>
            <th>
                <%= Html.ResourceString("UrlPhotoProduct")%>
			</th>
            <th>
				<%= Html.ResourceString("Description")%>
            </th>
            <th style="width:50px;">
                <%= Html.ResourceString("Price")%>
            </th>
            <th>
                <%= Html.ResourceString("Currencies")%>
            </th>
			<th>
                <%= Html.ResourceString("Availability")%>
            </th>
            <th>
                <%= Html.ResourceString("ActiveM")%>
                <input type="checkbox" id="activeBox" />
            </th>
            <th>
                
            </th>
        </tr>
        <%
              foreach (var item in Model)
          {%>
        <tr>
            <td>
                <%=Html.Encode(item.IdDealerProd)%>
                <%=Html.Hidden("idDP_" + item.IdDealerProd, item.IdDealerProd)%>				
                <%=Html.Hidden("idPByD_" + item.IdDealerProd, item.IdProductByDealer)%>                
            </td>
            <td>
				<%=Html.Encode(item.dpName)%>
<%--                <%=Html.TextBox("name_" + item.IdDealerProd, item.dpName, new { style = "width:100px;" })%>--%>
            </td>
            <td>
                <%=Html.Encode(item.dpUnit)%>
<%--                <%=Html.TextBox("unit_" + item.IdDealerProd, item.dpUnit, new { style = "width:100%;" })%>--%>
            </td>
            <td>
                <%=Html.TextBox("guarantee_" + item.IdDealerProd, item.Quarantee, new { style = "width:30px;" })%>
            </td>
            <td>
				<%=Html.TextBox("photoUrl_" + item.IdDealerProd, item.dpUrl, new { style = "width:100px;" })%>
            </td>
			<td>
				<%=Html.TextBox("descr_" + item.IdDealerProd, item.dpDescription, new { style = "width:200px;" })%>
            </td>
            <td>
                <% CultureInfo enUs = CultureInfo.GetCultureInfo("en-US"); %>
                <%= Html.TextBox("price_" + item.IdDealerProd, item.Price.ToString(enUs), new { style = "width:50px;" })%>
            </td>
            <td>
                <%= Html.DropDownList("currency_" + item.IdDealerProd, new SelectList((List<Currencies>)ViewData["currencies"], "Id", "Name", item.CurId))%>                 
            </td>
            <td>
                <%= Html.TextBox("state_" + item.IdDealerProd, item.State, new { style = "width:50px;" })%>
            </td>
            <td align="center">
                <%= Html.CheckBox("enable_" + item.IdDealerProd, item.dpEnable, new { @class = "activeBox" })%>
            </td>
            <td>
                <%=Html.ActionLink("x", "DeleteProductAdditional", new { productId = item.IdDealerProd, groupId = ViewData["groupId"] }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
            </td>
        </tr>
        <%	} %>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%} %>
    <%} %>
</asp:Content>

