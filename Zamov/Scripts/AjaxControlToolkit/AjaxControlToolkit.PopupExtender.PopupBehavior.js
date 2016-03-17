﻿Type.registerNamespace("AjaxControlToolkit");AjaxControlToolkit.PopupBehavior=function(c){var b=null,a=this;AjaxControlToolkit.PopupBehavior.initializeBase(a,[c]);a._x=0;a._y=0;a._positioningMode=AjaxControlToolkit.PositioningMode.Absolute;a._parentElement=b;a._parentElementID=b;a._moveHandler=b;a._firstPopup=true;a._originalParent=b;a._visible=false;a._onShow=b;a._onShowEndedHandler=b;a._onHide=b;a._onHideEndedHandler=b};AjaxControlToolkit.PopupBehavior.prototype={initialize:function(){var a=this;AjaxControlToolkit.PopupBehavior.callBaseMethod(a,"initialize");a._hidePopup();a.get_element().style.position="absolute";a._onShowEndedHandler=Function.createDelegate(a,a._onShowEnded);a._onHideEndedHandler=Function.createDelegate(a,a._onHideEnded)},dispose:function(){var b=null,a=this,c=a.get_element();if(c){if(a._visible)a.hide();if(a._originalParent){c.parentNode.removeChild(c);a._originalParent.appendChild(c);a._originalParent=b}c._hideWindowedElementsIFrame=b}a._parentElement=b;if(a._onShow&&a._onShow.get_animation()&&a._onShowEndedHandler)a._onShow.get_animation().remove_ended(a._onShowEndedHandler);a._onShowEndedHandler=b;a._onShow=b;if(a._onHide&&a._onHide.get_animation()&&a._onHideEndedHandler)a._onHide.get_animation().remove_ended(a._onHideEndedHandler);a._onHideEndedHandler=b;a._onHide=b;AjaxControlToolkit.PopupBehavior.callBaseMethod(a,"dispose")},show:function(){var a=this;if(a._visible)return;var b=new Sys.CancelEventArgs;a.raiseShowing(b);if(b.get_cancel())return;a._visible=true;var c=a.get_element();$common.setVisible(c,true);a.setupPopup();if(a._onShow){$common.setVisible(c,false);a.onShow()}else a.raiseShown(Sys.EventArgs.Empty)},hide:function(){var a=this;if(!a._visible)return;var b=new Sys.CancelEventArgs;a.raiseHiding(b);if(b.get_cancel())return;a._visible=false;if(a._onHide)a.onHide();else{a._hidePopup();a._hideCleanup()}},getBounds:function(){var d=this,c=d.get_element(),h=c.offsetParent||document.documentElement,f,b;if(d._parentElement){b=$common.getBounds(d._parentElement);var g=$common.getLocation(h);f={x:b.x-g.x,y:b.y-g.y}}else{b=$common.getBounds(h);f={x:0,y:0}}var e=c.offsetWidth-(c.clientLeft?c.clientLeft*2:0),i=c.offsetHeight-(c.clientTop?c.clientTop*2:0);if(d._firstpopup){c.style.width=e+"px";d._firstpopup=false}var a;switch(d._positioningMode){case AjaxControlToolkit.PositioningMode.Center:a={x:Math.round(b.width/2-e/2),y:Math.round(b.height/2-i/2)};break;case AjaxControlToolkit.PositioningMode.BottomLeft:a={x:0,y:b.height};break;case AjaxControlToolkit.PositioningMode.BottomRight:a={x:b.width-e,y:b.height};break;case AjaxControlToolkit.PositioningMode.TopLeft:a={x:0,y:-c.offsetHeight};break;case AjaxControlToolkit.PositioningMode.TopRight:a={x:b.width-e,y:-c.offsetHeight};break;case AjaxControlToolkit.PositioningMode.Right:a={x:b.width,y:0};break;case AjaxControlToolkit.PositioningMode.Left:a={x:-c.offsetWidth,y:0};break;default:a={x:0,y:0}}a.x+=d._x+f.x;a.y+=d._y+f.y;return new Sys.UI.Bounds(a.x,a.y,e,i)},adjustPopupPosition:function(a){var d=this.get_element();if(!a)a=this.getBounds();var b=$common.getBounds(d),c=false;if(b.x<0){a.x-=b.x;c=true}if(b.y<0){a.y-=b.y;c=true}if(c)$common.setLocation(d,a)},addBackgroundIFrame:function(){var c=this,b=c.get_element();if(Sys.Browser.agent===Sys.Browser.InternetExplorer&&Sys.Browser.version<7){var a=b._hideWindowedElementsIFrame;if(!a){a=document.createElement("iframe");a.src="javascript:'<html></html>';";a.style.position="absolute";a.style.display="none";a.scrolling="no";a.frameBorder="0";a.tabIndex="-1";a.style.filter="progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)";b.parentNode.insertBefore(a,b);b._hideWindowedElementsIFrame=a;c._moveHandler=Function.createDelegate(c,c._onMove);Sys.UI.DomEvent.addHandler(b,"move",c._moveHandler)}$common.setBounds(a,$common.getBounds(b));a.style.left=b.style.left;a.style.top=b.style.top;a.style.display=b.style.display;if(b.currentStyle&&b.currentStyle.zIndex)a.style.zIndex=b.currentStyle.zIndex;else if(b.style.zIndex)a.style.zIndex=b.style.zIndex}},setupPopup:function(){var a=this,b=a.get_element(),c=a.getBounds();$common.setLocation(b,c);a.adjustPopupPosition(c);b.zIndex=1e3;a.addBackgroundIFrame()},_hidePopup:function(){var a=this.get_element();$common.setVisible(a,false);if(a.originalWidth){a.style.width=a.originalWidth+"px";a.originalWidth=null}},_hideCleanup:function(){var a=this,c=a.get_element();if(a._moveHandler){Sys.UI.DomEvent.removeHandler(c,"move",a._moveHandler);a._moveHandler=null}if(Sys.Browser.agent===Sys.Browser.InternetExplorer){var b=c._hideWindowedElementsIFrame;if(b)b.style.display="none"}a.raiseHidden(Sys.EventArgs.Empty)},_onMove:function(){var a=this.get_element();if(a._hideWindowedElementsIFrame){a.parentNode.insertBefore(a._hideWindowedElementsIFrame,a);a._hideWindowedElementsIFrame.style.top=a.style.top;a._hideWindowedElementsIFrame.style.left=a.style.left}},get_onShow:function(){return this._onShow?this._onShow.get_json():null},set_onShow:function(c){var a=this;if(!a._onShow){a._onShow=new AjaxControlToolkit.Animation.GenericAnimationBehavior(a.get_element());a._onShow.initialize()}a._onShow.set_json(c);var b=a._onShow.get_animation();if(b)b.add_ended(a._onShowEndedHandler);a.raisePropertyChanged("onShow")},get_onShowBehavior:function(){return this._onShow},onShow:function(){var a=this;if(a._onShow){if(a._onHide)a._onHide.quit();a._onShow.play()}},_onShowEnded:function(){this.adjustPopupPosition();this.addBackgroundIFrame();this.raiseShown(Sys.EventArgs.Empty)},get_onHide:function(){return this._onHide?this._onHide.get_json():null},set_onHide:function(c){var a=this;if(!a._onHide){a._onHide=new AjaxControlToolkit.Animation.GenericAnimationBehavior(a.get_element());a._onHide.initialize()}a._onHide.set_json(c);var b=a._onHide.get_animation();if(b)b.add_ended(a._onHideEndedHandler);a.raisePropertyChanged("onHide")},get_onHideBehavior:function(){return this._onHide},onHide:function(){var a=this;if(a._onHide){if(a._onShow)a._onShow.quit();a._onHide.play()}},_onHideEnded:function(){this._hideCleanup()},get_parentElement:function(){var a=this;if(!a._parentElement&&a._parentElementID){a.set_parentElement($get(a._parentElementID));Sys.Debug.assert(a._parentElement!=null,String.format(AjaxControlToolkit.Resources.PopupExtender_NoParentElement,a._parentElementID))}return a._parentElement},set_parentElement:function(a){this._parentElement=a;this.raisePropertyChanged("parentElement")},get_parentElementID:function(){if(this._parentElement)return this._parentElement.id;return this._parentElementID},set_parentElementID:function(a){this._parentElementID=a;if(this.get_isInitialized())this.set_parentElement($get(a))},get_positioningMode:function(){return this._positioningMode},set_positioningMode:function(a){this._positioningMode=a;this.raisePropertyChanged("positioningMode")},get_x:function(){return this._x},set_x:function(b){var a=this;if(b!=a._x){a._x=b;if(a._visible)a.setupPopup();a.raisePropertyChanged("x")}},get_y:function(){return this._y},set_y:function(b){var a=this;if(b!=a._y){a._y=b;if(a._visible)a.setupPopup();a.raisePropertyChanged("y")}},get_visible:function(){return this._visible},add_showing:function(a){this.get_events().addHandler("showing",a)},remove_showing:function(a){this.get_events().removeHandler("showing",a)},raiseShowing:function(b){var a=this.get_events().getHandler("showing");if(a)a(this,b)},add_shown:function(a){this.get_events().addHandler("shown",a)},remove_shown:function(a){this.get_events().removeHandler("shown",a)},raiseShown:function(b){var a=this.get_events().getHandler("shown");if(a)a(this,b)},add_hiding:function(a){this.get_events().addHandler("hiding",a)},remove_hiding:function(a){this.get_events().removeHandler("hiding",a)},raiseHiding:function(b){var a=this.get_events().getHandler("hiding");if(a)a(this,b)},add_hidden:function(a){this.get_events().addHandler("hidden",a)},remove_hidden:function(a){this.get_events().removeHandler("hidden",a)},raiseHidden:function(b){var a=this.get_events().getHandler("hidden");if(a)a(this,b)}};AjaxControlToolkit.PopupBehavior.registerClass("AjaxControlToolkit.PopupBehavior",AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.PositioningMode=function(){throw Error.invalidOperation()};AjaxControlToolkit.PositioningMode.prototype={Absolute:0,Center:1,BottomLeft:2,BottomRight:3,TopLeft:4,TopRight:5,Right:6,Left:7};AjaxControlToolkit.PositioningMode.registerEnum("AjaxControlToolkit.PositioningMode");
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
