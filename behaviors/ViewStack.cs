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

using SharpKit.JavaScript;
using SharpKit.jQuery;
using randori.behaviors.viewStack;
using randori.content;

namespace randori.behaviors {
    [JsType(JsMode.Prototype, OmitCasts = true, NativeOverloads = false)]
    public class ViewStack : AbstractBehavior {
        readonly ViewChangeAnimator viewChangeAnimator;
        readonly ContentLoader contentLoader;

        private JsString _currentView;

        public bool hasView(JsString url) {
            return false;
        }

        public void addView(JsString url) {
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
            _currentView = url;
            //contentLoader
        }


        private jQuery transitionViews(jQuery arrivingView, jQuery departingView, object data = null) {
            return null;
        }

        protected override void onRegister() {
            
        }

        public ViewStack(ContentLoader contentLoader, ViewChangeAnimator viewChangeAnimator) {
            this.contentLoader = contentLoader;
            this.viewChangeAnimator = viewChangeAnimator;
        }
    }
}
