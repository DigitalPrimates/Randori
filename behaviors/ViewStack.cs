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
using randori.async;
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

        private jQuery _currentView;
        private JsArray<jQuery> viewFragmentStack;
		private JsObject<AbstractMediator> mediators;

        public bool hasView(JsString url) {
            jQuery fragment = null;
            var allFragments = decoratedNode.children();

            if ( allFragments.length > 0 ) {
                fragment = allFragments.find("[data-url='" + url + "']");    
            }
            
            return ((fragment != null) && fragment.length>0);
        }

        public Promise<AbstractMediator> pushView(JsString url ) {
            Promise<AbstractMediator> promise;

            var stack = this;
            var div = new HtmlDivElement();
            var fragment = jQueryContext.J(div);
            fragment.hide();
            fragment.css("width", "100%");
            fragment.css("height", "100%");
            fragment.css("position", "absolute");
            fragment.css("top", "0");
            fragment.css("left", "0");
            fragment.data( "url", url ) ;

            promise = contentLoader.asynchronousLoad(url).thenR<AbstractMediator>(delegate(string result) {
                var content = contentParser.parse(result);

                fragment.html(content);
                decoratedNode.append(div);

                var mediatorCapturer = new MediatorCapturer();
                domWalker.walkDomFragment(div, mediatorCapturer);

                viewFragmentStack.push(fragment);
                var mediator = mediatorCapturer.mediator;
                mediators[ url ] = mediator;

                showView(_currentView, fragment);

                return mediator;
            } );

            return promise;
        }

        public void popView() {

            var oldView = viewFragmentStack.pop();
            if (oldView != null ) {
                oldView.remove();
            }

            if ( viewFragmentStack.length > 0 ) {
                _currentView = viewFragmentStack[ viewFragmentStack.length - 1 ];
                if ( _currentView != null ) {
                    _currentView.show();
                }
            } else {
                _currentView = null;
            }
        }

        public JsString currentViewUrl {
            get {
                return ( (_currentView!=null)?_currentView.data("url").As<JsString>():null );
            }
        }

	    public void selectView(JsString url) {

            if (currentViewUrl != url) {

                var fragment = decoratedNode.children().filter("[data-url=" + url + "]");

                if (fragment == null) {
                    throw new JsError("Unknown View");
                }

                fragment.detach();
                decoratedNode.append( fragment );

                showView( _currentView, fragment );

                _currentView = fragment;
            } 
        }

        private void showView(jQuery oldFragment, jQuery newFragment ) {
            if (oldFragment != null) {
                oldFragment.hide();
            }

            if (newFragment != null) {
                newFragment.show();
            }            
        }

        private jQuery transitionViews(jQuery arrivingView, jQuery departingView, object data = null) {
            return null;
        }

        protected override void onRegister() {
			mediators = new JsObject<AbstractMediator>();

            //We may eventually want to look for existing elements and hold onto them... not today
            decoratedNode.empty();
        }

        protected override void onDeregister() {
            decoratedNode.empty();
        }

        public ViewStack(ContentLoader contentLoader, ContentParser contentParser, DomWalker domWalker, ViewChangeAnimator viewChangeAnimator) {
            this.contentLoader = contentLoader;
            this.contentParser = contentParser;
            this.viewChangeAnimator = viewChangeAnimator;
            this.domWalker = domWalker;

            viewFragmentStack = new JsArray<jQuery>();
        }
    }
}
