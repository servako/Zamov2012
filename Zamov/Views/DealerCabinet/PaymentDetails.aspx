<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PaymentDetails</title>
    <script type="text/javascript">
        function updatePaymentDetails() {
            document.getElementById("paymentForm").submit();
        }
    </script>
</head>
<body>
    <form method="post" id="paymentForm">
        <%= Html.CheckBox("cash")%>
        <%= Html.ResourceString("Encash")%>
        <br />
        <%= Html.CheckBox("noncash")%>
        <%= Html.ResourceString("Noncash")%>
        <br />
        <%= Html.CheckBox("card")%>
        <%= Html.ResourceString("Card")%>
        <br />  
        <%= Html.CheckBox("hasDiscounts")%>
        <%= Html.ResourceString("HasDiscounts")%>
    </form>
</body>
</html>
