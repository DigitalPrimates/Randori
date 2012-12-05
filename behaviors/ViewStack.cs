/***
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
        readonly ViewChangeAnimator viewChangeAnimator;
        readonly ContentLoader contentLoader;
        readonly DomWalker domWalker;

        private JsString _currentView;
        private JsObject<jQuery> views;

        private jQuery rootElement;

        public bool hasView(JsString url) {
            return false;
        }

        public void addView(JsString url) {
            //this is one point that I conced we should consider making async
            var content = contentLoader.synchronousLoad(url);
            var div = new HtmlDivElement();
            var fragment = jQueryContext.J(div);
            fragment.hide();
            fragment.html(content);
            fragment.css("width", "100%");
            fragment.css("height", "100%");
            fragment.css("position", "absolute");
            fragment.css("top", "0");
            fragment.css("left", "0");

            domWalker.walkDomFragment(div);

            rootElement.append(div);
            views[url] = fragment;

            currentView = url;
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
            set {
                selectView(value);
            }
        }

        private void selectView(JsString url) {
            if ( _currentView != url ) {
                var oldFragment = views[_currentView];
                var fragment = views[url];

                if (oldFragment!= null) {
                    oldFragment.hide();
                }

                _currentView = url;
                fragment.show();
            }
        }

        private jQuery transitionViews(jQuery arrivingView, jQuery departingView, object data = null) {
            return null;
        }

        protected override void onRegister() {
            views = new JsObject<jQuery>();

            //We may eventually want to look for existing elements and hold onto them... not today
            rootElement = jQueryContext.J(decoratedElement);
            rootElement.empty();
        }

        public ViewStack(ContentLoader contentLoader, DomWalker domWalker, ViewChangeAnimator viewChangeAnimator) {
            this.contentLoader = contentLoader;
            this.viewChangeAnimator = viewChangeAnimator;
            this.domWalker = domWalker;
        }
    }
}
