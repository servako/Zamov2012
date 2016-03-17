<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    $(function() {
        $.cookie("makeOrder", "false");
    })
</script>
    <center>
    
    <h2><%=Html.ResourceString("ThankYou")%></h2>
    
    <%
        if (!Request.IsAuthenticated)
        {%>
     <table>
     <tr>
        <td colspan="2">
            <%=Html.ResourceString("WouldRegister")%>   
        </td>
     </tr>
     <tr>
        <td align="right">
        <%using(Html.BeginForm("Register", "Account", FormMethod.Get))
        {%>
            <input style="width:50px;" type="submit" value="<%=Html.ResourceString("Yes")%>" />
        <%}%>
     
        </td>
        <td align="left">
        <%using (Html.BeginForm("EmptyCart", "Cart", FormMethod.Get))
          {%>
            <input style="width:50px;" type="submit" value="<%=Html.ResourceString("No")%>" />
        <%}%>
        </td>
     </tr>
     </table>
     <%
    
        }
        else
        {%>

        
        <%using(Html.BeginForm("Index", "Home", FormMethod.Get))
        {%>
            <input type="submit" value="<%=Html.ResourceString("ToMainPage")%>" />
        <%}
        }
        
        %>
    </center>
    

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
    <%= Html.RegisterJS("jquery.cookie.js") %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
</asp:Content>
