<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Order>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="AjaxControlToolkitMvc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.ValidationSummary() %>
    <%
        string pickerLocale = (SystemSettings.CurrentLanguage == "uk-UA") ? "uk" : "ru";
        string coookie = "true";
        if (Response.Cookies["makeOrder"] != null)
            if (Response.Cookies["makeOrder"].Value != null)
                coookie = Response.Cookies["makeOrder"].Value.ToString();
    %>

    <script type="text/javascript">

        $(function() {
            $.datepicker.setDefaults($.extend({ showMonthAfterYear: false }, $.datepicker.regional['']));
            applyDropShadows(".orderDetails .logo img", "shadow3");
            $("#deliveryDate").datepicker($.datepicker.regional["<%= pickerLocale %>"]);
            //$("#deliveryDate").datepicker("setDate", new Date());
            $("#deliveryDate").datepicker('option', 'minDate', new Date());
            $("#agreementLink").fancybox({ frameWidth: 700, hideOnContentClick: false });
            checkCookie();
        })

        function setOrderTime() {
            var date = $("#deliveryDate").datepicker("getDate");
            $get("deliveryDateTime").value = $get("deliveryDate").value + " " + $get("deliveryTime").value;
        }

        function checkCookie() {
            //var cookieValue = $.cookie("makeOrder");
            var cookieValue = "<%=coookie %>";
            //alert(cookieValue);
            if (!cookieValue || cookieValue != "true")
                location.href = "/Cart/Expired";
        }

        function checkComments(e, len) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var code = e.keyCode ? e.keyCode : (e.which ? e.which : e.charCode)

            switch (code) {
                case 13:
                case 8:
                case 9:
                case 46:
                case 37:
                case 38:
                case 39:
                case 40:
                    return true;
            }
            return target.value.length <= len;
        }
    </script>

    <% foreach (var order in Model)
       {
           int dealerId = (int)order.DealerReference.EntityKey.EntityKeyValues[0].Value;
           DealerPresentation presentation = Dealer.GetPresentation(dealerId);
    %>
    <div class="orderDetails">
        <center>
        <table>
            <tr>
                <td>
                    <div class="logo">
                        <%= Html.Image("~/Image/ShowLogo/" + order.DealerReference.EntityKey.EntityKeyValues[0].Value) %>
                    </div>
                </td>
                <td align="center">
                    <div class="paymentTypesHeader">
                        <%= Html.ResourceString("SelectPaymentType") %>
                    </div>
                    <div class="paymentTypes">
                        <table>
                            <% 
                                int currentPaymentType = (int)PaymentTypes.Encash;
                                if (ViewData["paymentType_" + order.HashCode] != null)
                                {
                                    currentPaymentType = Convert.ToInt32(ViewData["paymentType_" + order.HashCode]);
                                }
                                List<SelectListItem> paymentTypes = new List<SelectListItem>();
                                if (presentation.Cash)
                                    paymentTypes.Add(new SelectListItem { Text = Html.ResourceString("Encash"), Value = ((int)PaymentTypes.Encash).ToString(), Selected = currentPaymentType == (int)PaymentTypes.Encash });
                                if (presentation.Card)
                                    paymentTypes.Add(new SelectListItem { Text = Html.ResourceString("Card"), Value = ((int)PaymentTypes.Card).ToString(), Selected = currentPaymentType == (int)PaymentTypes.Card });
                                if (presentation.Noncash)
                                    paymentTypes.Add(new SelectListItem { Text = Html.ResourceString("Noncash"), Value = ((int)PaymentTypes.Noncash).ToString(), Selected = currentPaymentType == (int)PaymentTypes.Noncash });
                                MvcHtmlString[] options = Html.RadioButtonList("paymentType_" + order.HashCode, paymentTypes);
                                for (int i = 0; i < options.Length; i++)
                                {%>
                            <tr>
                                <td>
                                    <%= paymentTypes[i].Text %>
                                </td>
                                <td>
                                    <%= options[i] %>
                                </td>
                            </tr>
                            <%}
                            %>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        </center>
        <%if (presentation.HasDiscounts)
          { %>
        <div class="discount">
            <%= Html.ResourceString("InputVoucher")%>
            <%= Html.TextBox("voucherCode_" + order.HashCode)%>
            <div class="respectDiscount">
                <%= Html.ResourceString("DiscountWillBeRespected")%>
            </div>
        </div>
        <%} %>
    </div>
    <%} %>
    <div style="text-align: center; margin-top: 30px;">
        <%= Html.CheckBox("agreedPresentation", false, new { onclick = "$get('agreed').checked = this.checked;"})%>
        <span id="agreedSpan">
            <%= Html.ResourceActionLink("Agreed", "Agreement", "Home", null, new {id="agreementLink"}) %>
        </span>
<%--        <%= Html.ValidationMessage("agreed", "<script type='text/javascript'>$('#agreedSpan a').css('color', 'red');</script>").ToHtmlString() %>--%>
        <%= Html.ValidationMessage("agreed", "*") %>
    </div>
    <div style="color: Red; font-weight: bold; margin-top: 20px; margin-bottom: 30px; height:100px;" class="orderValidation">
        <label for="captcha">
        </label>
        <%=Html.ResourceString("EnterCode")%>
        <span class="validationError">*</span>
        <%= Html.ValidationMessage("captchaInvalid", " ")%>
        <br />
        <div style="font-weight:normal; color:Black;">
            <%= Html.CaptchaImage(50, 160)%><br />
            <%= Html.TextBox("captcha", "")%>
        </div>
    </div>
    <div class="orderDone">
        <center>
            <table>
                <tr>
                    <td valign="middle">
                        <%= Html.ResourceString("Done") %>!
                    </td>
                    <td>
                        <%= Html.SubmitImage("", "~/Content/img/tickMark.jpg")%>
                    </td>
                </tr>
            </table>
        </center>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterCss("~/Content/Cart.css") %>
    <%= Html.RegisterCss("~/Content/shadows.css")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterJS("dropshadow.js")%>
    <%= Html.RegisterJS("ui.datepicker-ru.js")%>
    <%= Html.RegisterJS("ui.datepicker-uk.js")%>
    <%= Html.RegisterCss("~/Content/redmond/jquery.ui.css")%>
    <%= Html.RegisterJS("jquery.ui.js")%>
    <%= Html.RegisterJS("jquery.cookie.js")%>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentTop" runat="server">
    <form method="post" action="/Cart/MakeOrder">
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentBottom" runat="server">
    </form>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
    <div class="yourDetails">
        <div class="menuHeader centered">
            <%= Html.ResourceString("YourDetails") %>
        </div>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td class="parameterName">
                    <%= Html.ResourceString("FirstName") %>
                    <span class="validationError">*</span>
                    <%= Html.ValidationMessage("firstName", " ")%>
                </td>
                <td class="parameterValue">
                    <%= Html.TextBox("firstName") %>
                </td>
            </tr>
            <tr>
                <td class="parameterName">
                    <%= Html.ResourceString("LastName")%>
                </td>
                <td class="parameterValue">
                    <%= Html.TextBox("lastName") %>
                </td>
            </tr>
            <tr>
                <td class="parameterName">
                    <%= Html.ResourceString("City") %>
                    <span class="validationError">*</span>
                    <%= Html.ValidationMessage("city", " ")%>
                </td>
                <td class="parameterValue">
                    <%= Html.TextBox("city") %>
                </td>
            </tr>
            <tr>
                <td class="parameterName">
                    <%= Html.ResourceString("DeliveryAddress")%>
                    <span class="validationError">*</span>
                    <%= Html.ValidationMessage("deliveryAddress", " ")%>
                </td>
                <td class="parameterValue">
                    <%= Html.TextArea("deliveryAddress")%>
                </td>
            </tr>
            <tr>
                <td class="parameterName">
                    <%= Html.ResourceString("ContactPhone") %>
                    <span class="validationError">*</span>
                    <%= Html.ValidationMessage("contactPhone", " ")%>
                </td>
                <td class="parameterValue">
                    <%= Html.TextBox("contactPhone")%>
                </td>
            </tr>
            <tr>
                <td class="parameterName">
                    <nowrap>E-mail</nowrap>
                    <%= Html.ValidationMessage("email", "*", new { @class="validationError"})%>
                </td>
                <td class="parameterValue">
                    <%= Html.TextBox("email")%>
                </td>
            </tr>
            <tr>
                <td class="parameterName">
                    <%= Html.ResourceString("Comments") %>
                </td>
                <td class="parameterValue">
                    <%= Html.TextArea("comments", new { onkeypress = "return checkComments(event,250)" })%>
                </td>
            </tr>
        </table>
        <div class="menuFooter">
        </div>
    </div>
    <%= Html.Hidden("deliveryDateTime", DateTime.Now)%>
    <div class="deliveryDate">
        <div class="menuHeader centered">
            <%= Html.ResourceString("DeliveryDateTime") %>
        </div>
        <div class="datePicker">
            <%= Html.TextBox("deliveryDate") %><br />
            <%--<%= HttpUtility.HtmlDecode(Html.ValidationMessage("deliveryDate", "<script type='text/javascript'>$('#deliveryDate').css('color', 'red').css('border', '1px solid red');</script>").ToString())%>--%>
            <%= Html.ValidationMessage("deliveryDate", "*")%>
            <%= Html.TextBox("deliveryTime", DateTime.Now.AddHours(1).ToString("HH:mm")) %>
            <%--<%= HttpUtility.HtmlDecode(Html.ValidationMessage("deliveryTime", "<script type='text/javascript'>$('#deliveryTime').css('color', 'red').css('border', '1px solid red');</script>").ToString())%>--%>
            <%= Html.ValidationMessage("deliveryTime", "*") %>
            <%= Html.Hidden("deliveryTimeClientState") %>
            <%= Ajax.MaskEdit("deliveryTimeClientState", MaskTypes.Time, "99:99", false, false, "deliveryTime")%>
            <%= Html.CheckBox("agreed", false, new { style="position:relative; left:-4000px;"})%>
        </div>
        <div class="menuFooter">
        </div>
    </div>
</asp:Content>
