using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace AjaxControlToolkitMvc
{
    public enum MaskTypes
    { 
        None = 0,
        Number = 1,
        Date = 2,
        Time = 3,
        DateTime = 4
    }

    public static class MaskEditExtensions
    {
        public static string MaskEdit(this AjaxHelper helper, string clientStateFieldID, MaskTypes maskType, string mask, bool acceptAMPM, bool clearMaskOnLostFocus, string elementId)
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
                "AjaxControlToolkit.MaskedEdit.MaskedEditBehavior.js",
                "AjaxControlToolkit.MaskedEdit.MaskedEditValidator.js"
            ));

            var props = new
            {
                MaskType = (int)maskType,
                Mask = mask,
                AcceptAMPM = acceptAMPM,
                ClearMaskOnLostFocus = clearMaskOnLostFocus,
                ClientStateFieldID = clientStateFieldID,
                CultureAMPMPlaceholder = "AM;PM",
                CultureCurrencySymbolPlaceholder="грн.",
                CultureDateFormat="DMY",
                CultureDatePlaceholder=".",
                CultureDecimalPlaceholder=",",
                CultureName="en-US",
                CultureThousandsPlaceholder="",
                CultureTimePlaceholder=":"
            };

            sb.AppendLine(helper.Create("AjaxControlToolkit.MaskedEditBehavior", props, elementId));   
            return sb.ToString();
        }
    }
}
