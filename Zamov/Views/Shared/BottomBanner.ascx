<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Zamov.Models.Advert>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>



<div id="bannerBottomOuter">

<table style="width:100%;">
    <tr>
        <td align="left">
            <div id="bannerBottomLeft">
                <%
                    var leftBanner = Model.Select(b => b).Where(b => b.Position == (int) BannerPosition.BottomLeft).First();
                    if (leftBanner != null&&leftBanner.IsActive)
                    {
                        %>
                        <%=Html.RegisterFlashScript(leftBanner.ImageSource, "b_" + leftBanner.Id, (BannerPosition)leftBanner.Position)%>
                        
                        <%
                    }
                        
                %>
            </div>
        </td>
        <td align="center">
            <div id="bannerBottomCenter">
                <%
                    var centerBanner = Model.Select(b => b).Where(b => b.Position == (int) BannerPosition.BottomCenter).First();
                    if (centerBanner != null && centerBanner.IsActive)
                    {%>
                    <%=Html.RegisterFlashScript(centerBanner.ImageSource, "b_" + centerBanner.Id, (BannerPosition)centerBanner.Position)%>
                    <%
                    }
%>
            </div>
        </td>
        <td align="right">
            <div id="bannerBottomRight">
                <%
                    var rightBanner = Model.Select(b => b).Where(b => b.Position == (int) BannerPosition.BottomRight).First();
                    if (rightBanner != null&&rightBanner.IsActive)
                    {%>
                       <%=Html.RegisterFlashScript(rightBanner.ImageSource, "b_" + rightBanner.Id, (BannerPosition)rightBanner.Position)%>
                        <%
                    }
                %>
            </div>
        </td>
    </tr>
</table>
</div>
