﻿/***
 * Copyright 2012 LTN Consulting, Inc. /dba Digital Primates®
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * @author Michael Labriola <labriola@digitalprimates.net>
 */

using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using randori.behaviors.viewStack;
using randori.content;
using randori.dom;

namespace randori.behaviors {
    [JsType(JsMode.Prototype, OmitCasts = true, NativeOverloads = false)]
    public class ViewStack : AbstractBehavior {
        readonly ContentLoader contentLoader;
        readonly ContentParser contentParser;
        readonly DomWalker domWalker;
        readonly ViewChangeAnimator viewChangeAnimator;

        private JsString _currentView;
        private JsObject<jQuery> views;
		private JsObject<AbstractMediator> mediators;

        private jQuery rootElement;

        public bool hasView(JsString url) {
            return (views[url] != null);
        }

	    public void addView(JsString url) {
            if (hasView(url)) {
                selectView( url );
            } else {
                addView(url, null);
            }
	    }

	    public void addView(JsString url, object viewData) {
            if (!hasView(url)) {
                //this is one point that I conced we should consider making async
                var content = contentParser.parse( contentLoader.synchronousFragmentLoad( url ) );
                var div = new HtmlDivElement();
                var fragment = jQueryContext.J( div );
                fragment.hide();
                fragment.html( content );
                fragment.css( "width", "100%" );
                fragment.css( "height", "100%" );
                fragment.css( "position", "absolute" );
                fragment.css( "top", "0" );
                fragment.css( "left", "0" );

                var mediatorCapturer = new MediatorCapturer();
                domWalker.walkDomFragment( div, mediatorCapturer );

                rootElement.append( div );
                views[ url ] = fragment;
                mediators[ url ] = mediatorCapturer.mediator;
            }

	        selectView( url, viewData );
        }

        public void removeView(JsString url) {

            if (url == _currentView) {
                //we must choose some view to be active now... catch a tiger by its toe...
                //this is not recommended
            }

            var fragment = views[url];
            fragment.remove();

            JsContext.delete( views[url] );
        }

        public JsString currentView {
            get {
                return _currentView;
            }
        }

	    public void selectView(JsString url) {
			selectView(url, null);
	    }
		
	    public void selectView(JsString url, object viewData) {

            if ( _currentView != url ) {
                var oldFragment = views[_currentView];
                var fragment = views[url];

                if (fragment == null ) {
                    throw new JsError("Unknown View");
                }

                if (oldFragment!= null) {
                    oldFragment.hide();
                }

                _currentView = url;

				var mediator = mediators[url];
				if (mediator != null) {
					mediator.setViewData(viewData);
				}

                fragment.show();
            } 
        }

        private jQuery transitionViews(jQuery arrivingView, jQuery departingView, object data = null) {
            return null;
        }

        protected override void onRegister() {
            views = new JsObject<jQuery>();
			mediators = new JsObject<AbstractMediator>();

            //We may eventually want to look for existing elements and hold onto them... not today
            rootElement = jQueryContext.J(decoratedElement);
            rootElement.empty();
        }

        public ViewStack(ContentLoader contentLoader, ContentParser contentParser, DomWalker domWalker, ViewChangeAnimator viewChangeAnimator) {
            this.contentLoader = contentLoader;
            this.contentParser = contentParser;
            this.viewChangeAnimator = viewChangeAnimator;
            this.domWalker = domWalker;
        }
    }
}
