<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Zamov.Models.Brands>>" %>
<%
	if (Model.Count > 0)
	{
		for (int i = 0; i < Model.Count(); i++)
		{
%>
<%=Html.CheckBox(string.Format("brands[{0}].IsCheck", i+10), Model[i].IsCheck)%>
<%=Html.Hidden(string.Format("brands[{0}].brandID", i+10), Model[i].brandID)%>
<span style="padding-left:10px">
<%=Html.Hidden(string.Format("brands[{0}].brandName", i+10), Model[i].brandName)%>
</span>
<%=Html.Label(Model[i].brandName)%>
<br />
<%
		}
	}
%>