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
using guice.reflection;

namespace randori.behaviors {

    public abstract class AbstractBehavior {

        protected jQuery rootElement;
        protected JsObject<object> viewElementIDMap;
        protected JsObject<object> viableInjectionPoints;

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

        protected abstract void onRegister();

        public void injectPotentialNode( JsString id, object node) {

            //setup our injection point requirements
            if ( this.viableInjectionPoints == null ) {
                this.viableInjectionPoints = getBehaviorInjectionPoints();                
            }

            //if we ever want to throw an error because of a duplicate id injection, this is the place to do it
            //right now first one in wins
            if (viableInjectionPoints[id] != null) {
                JsContext.delete( viableInjectionPoints[ id ] );
                dynamic instance = this;
                instance[ id ] = node;
            }
        }

        public void verifyAndRegister() {
            foreach (var id in viableInjectionPoints) {
                if (viableInjectionPoints[id] == "req") {
                    dynamic instance = this;
                    var typeDefinition = new TypeDefinition(instance.constructor);

                    HtmlContext.alert(typeDefinition.getClassName() + " requires a [View] element with the id of " + id + " but it could not be found");
                    return;                    
                }
                JsContext.delete(viableInjectionPoints[id]);
            }

            this.viableInjectionPoints = null;
            onRegister();
        }

        private JsObject<object> getBehaviorInjectionPoints() {
            dynamic jsInstance = this;

            var map = new JsObject<object>();
            var typeDefinition = new TypeDefinition(jsInstance.constructor);

            JsArray<InjectionPoint> viewPoints = typeDefinition.getViewFields();

            for (int i = 0; i < viewPoints.length; i++) {
                if (viewPoints[i].r == 0) {
                    map[viewPoints[i].n] = "opt";
                } else {
                    map[viewPoints[i].n] = "req";
                }
            }

            return map;
        }

        public AbstractBehavior(jQuery rootElement) {
            this.rootElement = rootElement;
        }
    }
}