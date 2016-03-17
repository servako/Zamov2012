using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace AjaxControlToolkitMvc
{
    public static class ModalPopUpExtensions
    {
        public static string ModalPopUp(
            this AjaxHelper helper,
            string targetControlId,
            string backgroungCssClass,
            string cancelControlId,
            string onCancelScript,
            bool dropShadow,
            string okControlId,
            string onOkScript,
            string popupControlId,
            string popupDragHandleControlId,
            int repositionMode)
        {
            var sb = new StringBuilder();

            sb.AppendLine(helper.ToolkitInclude
            (
                "AjaxControlToolkit.ExtenderBase.BaseScripts.js",
                "AjaxControlToolkit.Common.Common.js",
                "AjaxControlToolkit.Animation.Animations.js",
                "AjaxControlToolkit.PopupExtender.PopupBehavior.js",
                "AjaxControlToolkit.Common.Threading.js",
                "AjaxControlToolkit.Compat.Timer.Timer.js",
                "AjaxControlToolkit.DragPanel.FloatingBehavior.js",
                "AjaxControlToolkit.ModalPopup.ModalPopupBehavior.js"
            ));

            var props = new
            {
                BackgroungCssClass = backgroungCssClass,
                CancelControlID = cancelControlId,
                OnCancelScript = onCancelScript,
                DropShadow = dropShadow,
                OkControlID = okControlId,
                OnOkScript = onOkScript,
                PopupCntrolID = popupControlId,
                PopupDragHandleControlID = popupControlId,
                RepositionMode = repositionMode
            };

            sb.AppendLine(helper.Create("AjaxControlToolkit.ModalPopupBehavior", props, targetControlId));
            return sb.ToString();
        }
    }
}
