<%@ Page Title="" Language="C#" MasterPageFile="~/Views/DealerCabinet/Cabinet.Master"
    Inherits="System.Web.Mvc.ViewPage<List<Zamov.Models.Product>>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        var updates = {};

        $(function() {
            $("#imagePopUp").dialog({ autoOpen: false, width: 440, height: 360, minHeight: 360, resizable: false });
            $("#importProducts")
                .dialog({
                    autoOpen: false,
                    resizable: false,
                    modal: true,
                    buttons: { '<%= Html.ResourceString("Upload") %>': function() { $get("xlsForm").submit(); } }
                });
            $("#descriptionPopUp")
                .dialog({
                    autoOpen: false,
                    width: 700,
                    height: 600,
                    minHeight: 360,
                    resizable: false,
                    buttons: {
                        '<%= Html.ResourceString("Cancel") %>': function() { closeDescriptionDialog(); },
                        '<%= Html.ResourceString("Save") %>': function() { $get("descriptionsFrame").contentWindow.updateDescription(); }
                    }
                });

            $("#productsHeader input[type='checkbox']").click(function() {
                $("." + this.id).attr("checked", this.checked);
            })
        })

        function groupSelected() {
            $("#groupIds option").each(function(i) {
                if (this.selected)
                    location.href = "/DealerCabinet/Products/" + this.value;
            }
            )
        }

        function openImageIframe(productId) {
            $("#updateImageBox").attr("src", "/DealerCabinet/UpdateProductImage/" + productId);
            $("#imagePopUp").dialog('open').css("height", 300);

            $('#imagePopUp').dialog('option', 'height', 360);
            $('#imagePopUp').dialog('option', 'position', 'center');
            $('#imagePopUp').css('height', 'auto');
        }

        function openDescriptionIframe(productId) {
            $("#descriptionPopUp")
                .html('<iframe frameborder="0" name="descriptionsFrame" id="descriptionsFrame" hidefocus="true" style="width:660px; height:500px;" src="/DealerCabinet/UpdateProductDescription/' +
                    productId + 
                    '"></iframe>');

            $("#descriptionPopUp").dialog('open').css("height", 500);
            $('#descriptionPopUp').dialog('option', 'height', 600);
            $('#descriptionPopUp').dialog('option', 'position', 'center');
            //$('#descriptionPopUp').css('height', 'auto');
        }

        function closeImageDialog() {
            $("#imagePopUp").dialog('close');
        }

        function closeDescriptionDialog() {
            $("#descriptionPopUp").dialog('close');
        }
        
        function uploadXls(){
            $("#importProducts").dialog('open');

//            $('#imagePopUp').dialog('option', 'height', 360);
//            $('#imagePopUp').dialog('option', 'position', 'center');
//            $('#imagePopUp').css('height', 'auto');
        }
    </script>
    
    <div title="<%= Html.ResourceString("Image") %>" id="imagePopUp" style="display: block; height: 300px;">
        <iframe id="updateImageBox" frameborder="0" hidefocus="true" style="width: 400px;
            height: 299px; background: transparent;"></iframe>
    </div>
    
    <div title="<%= Html.ResourceString("Description") %>" id="descriptionPopUp" style="display: block; height: 300px;">
    </div>
    
    <div id="importProducts" style="padding:20px; text-align:center;">  
        <%using (Html.BeginForm("UploadXls", "DealerCabinet", FormMethod.Post, new { id="xlsForm", enctype="multipart/form-data" }))
          { %>
            <%= Html.Hidden("groupId", ViewData["groupId"]) %>
            <input type="file" name="xls" id="xls" />
        <%} %>
    </div>
    <h2>
        <%= Html.ResourceString("Products") %></h2>
    <br />
    
    <%= Html.DropDownList("groupIds", (List<SelectListItem>)ViewData["groups"], new { onchange = "groupSelected()" })%>
    <%= Html.ResourceActionLink("ManageGroups", "Groups") %>
    <a href="javascript:uploadXls()">
        <%= Html.ResourceString("ImportProducts") %>
    </a>
    
    <%
        if (Model != null && Model.Count > 0)
        { 
    %>
        <%using (Html.BeginForm("UpdateProducts", "DealerCabinet", FormMethod.Post))
      { %>
      <%= Html.ResourceString("MoveTo") %>
      <%= Html.DropDownList("groups", (List<SelectListItem>)ViewData["moveToGroups"])%> 
      &nbsp;&nbsp;&nbsp;
      <%= Html.ResourceString("SetManufacturer")%>
      <%=Html.DropDownList("manufacturers",(List<SelectListItem>)ViewData["manufacturers"]) %>
      &nbsp;&nbsp;&nbsp;
      <%= Html.ResourceString("Currencies")%>
      <%= Html.DropDownList("currencies", (List<SelectListItem>)ViewData["currencies"])%> 
      &nbsp;&nbsp;&nbsp;
      <input type="submit" value="->" />
      
      
      <br />
      <b>П</b> - <%= Html.ResourceString("Move") %>
      <br />
      <b>ЗП</b> - <%= Html.ResourceString("SetManufacturer")%>
      <br />
      <b>В</b> - <%= Html.ResourceString("Currencies")%>
      <br />
    <table class="adminTable">
        <tr id="productsHeader">
            <th>П
                <input type="checkbox" id="moveToBox" />
            </th>
            <th>ЗП
                <input type="checkbox" id="setManufacturerBox" />
            </th>
             <th>В
                <input type="checkbox" id="setCurrencyBox" />
            </th>
            <th style="width:100px;">
                <%= Html.ResourceString("PartNumber")%>
            </th>
            <th>
                <%= Html.ResourceString("Manufacturer")%>
            </th>
            <th>
                <%= Html.ResourceString("Title")%>
            </th>
            <th>
                <%= Html.ResourceString("Image") %>
                /
                <%= Html.ResourceString("Description") %>
            </th>
            <th style="width:50px;">
                <%= Html.ResourceString("Price")%>, (грн, если не указана валюта).
            </th>
            <th>
                В
            </th>
            <th style="width:50px;">
                <%= Html.ResourceString("Unit")%>
            </th>
            <th>
                <%= Html.ResourceString("New") %>
                <input type="checkbox" id="newBox" />
            </th>
            <th>
                <%= Html.ResourceString("Action") %>
                <input type="checkbox" id="actionBox" />
            </th>
            <th>
                <%= Html.ResourceString("Top") %>
                <input type="checkbox" id="topBox" />
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
                <%= Html.CheckBox("moveTo_" + item.Id, false, new { @class = "moveToBox" })%>
            </td>
            <td>
                <%= Html.CheckBox("setManufacturer_" + item.Id, false, new { @class = "setManufacturerBox" })%>
            </td>
            <td>
                <%= Html.CheckBox("setCurrency_" + item.Id, false, new { @class = "setCurrencyBox" })%>
            </td>
            <td>
                <%= Html.TextBox("partNumber_" + item.Id, item.PartNumber, new { style = "width:100px;" })%>
            </td>
            <td>
                <%if (item.Manufacturer.Count > 0)
                  { %>
                <%=Html.Encode(item.Manufacturer.First().Name)%>
                <%} %>
                
            </td>
            <td>
                <%= Html.TextBox("name_" + item.Id, item.Name)%>
            </td>
            <td align="center">
                <a href="javascript:openImageIframe(<%= item.Id %>)" style="text-decoration:none;">
                    <%= Html.Image("~/Content/img/productImage.jpg", new {style="border:none" })%>
                </a>&nbsp;
                <a href="javascript:openDescriptionIframe(<%= item.Id %>)" class="productDescriptionLink">
                    i
                </a>
            </td>
            <td>
                <% CultureInfo enUs = CultureInfo.GetCultureInfo("en-US"); %>
                <%= Html.TextBox("price_" + item.Id, item.Price.ToString(enUs), new { style = "width:50px;" })%>
            </td>
            <td>
                <%if(item.Currencies!=null){ %>
                <%=Html.Encode(item.Currencies.ShortName) %>
                <%} %>
            </td>
            <td>
                <%= Html.TextBox("unit_" + item.Id, item.Unit, new { style = "width:100%;" })%>
            </td>
            <td>
                <%= Html.CheckBox("new_" + item.Id, item.New, new { @class = "newBox" })%>
            </td>
            <td>
                <%= Html.CheckBox("action_" + item.Id, item.Action, new { @class = "actionBox" })%>
            </td>
            <td>
                <%= Html.CheckBox("topProduct_" + item.Id, item.TopProduct, new { @class = "topBox" })%>
            </td>
            <td align="center">
                <%= Html.CheckBox("active_" + item.Id, item.Enabled, new { onblur = "tableChanged(updates, this)", @class = "activeBox" })%>
            </td>
            <td>
                <%=Html.ActionLink("x", "DeleteProduct", new { productId = item.Id, groupId = ViewData["groupId"] }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
            </td>
        </tr>
        <%	} %>
    </table>
    <%= Html.Hidden("groupId") %>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%} %>
    <%} %>
    <%
        if (Convert.ToInt32(ViewData["groupId"]) >= 0)
        {        
    %>
    <div>
        <% using (Html.BeginForm("AddProduct", "DealerCabinet"))
           { %>
        <%= Html.Hidden("groupId") %>
        <table class="adminTable">
            <tr>
                <th>
                    <%= Html.ResourceString("PartNumber")%>
                </th>
                <th>
                    <%= Html.ResourceString("Title")%>
                </th>
                <%--                <th>
                    <%= Html.ResourceString("Description") %>
                    /
                    <%= Html.ResourceString("Image") %>
                </th>--%>
                <th>
                    <%= Html.ResourceString("Price")%>, грн.
                </th>
                <th>
                    <%= Html.ResourceString("Unit")%>
                </th>
                <th>
                    <%= Html.ResourceString("ActiveM")%>
                </th>
            </tr>
            <tr>
                <td>
                    <%= Html.TextBox("partNumber")%>
                </td>
                <td>
                    <%= Html.TextBox("name")%>
                </td>
                <td>
                    <%= Html.TextBox("price")%>
                </td>
                <td>
                    <%= Html.TextBox("unit")%>
                </td>
                <td align="center">
                    <%= Html.CheckBox("active")%>
                </td>
            </tr>
        </table>
        <input type="submit" value="<%= Html.ResourceString("Add") %>" />
        <%} %>
    </div>
    <%} %>
</asp:Content>
