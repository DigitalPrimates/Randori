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

namespace randori.behaviors {

    public abstract class AbstractBehavior {

        protected jQuery rootElement;
        protected JsObject<object> viewElementIDMap;

        //This highlights the need for some type of decorator more than an extension methodolofy for something becoming a behavior
        public void hide() {
            rootElement.hide();
        }

        public void show() {
            rootElement.show();
        }

        protected object getViewElementByID( JsString id ) {
            return viewElementIDMap[id].As<jQuery>();
        }

        public abstract void onRegister();

        public AbstractBehavior(jQuery rootElement) {
            this.rootElement = rootElement;
        }
    }
}