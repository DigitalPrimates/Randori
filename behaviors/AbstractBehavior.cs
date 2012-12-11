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

        protected JsObject<object> viewElementIDMap;
        protected JsObject<object> viableInjectionPoints;
        protected HtmlElement decoratedElement;

        //This highlights the need for some type of decorator more than an extension methodology for something becoming a behavior
        public void hide() {
            jQueryContext.J(decoratedElement).hide();
            //rootElement.hide();
        }

        public void show() {
            //rootElement.show();
            jQueryContext.J(decoratedElement).show();
        }

        protected object getViewElementByID( JsString id ) {
            return viewElementIDMap[id].As<jQuery>();
        }

        //Its possible we want to do some things before our children are parsed..
        //This helps facilitate us being a black box
        protected virtual void onPreRegister() {
            //setup our injection point requirements
            if (this.viableInjectionPoints == null) {
                this.viableInjectionPoints = getBehaviorInjectionPoints();
            }
        }

        protected abstract void onRegister();

        public void injectPotentialNode( JsString id, object node) {
            //if we ever want to throw an error because of a duplicate id injection, this is the place to do it
            //right now first one in wins
			if ((viableInjectionPoints != null ) && (viableInjectionPoints[id] != null))
			{
                JsContext.delete( viableInjectionPoints[ id ] );
                dynamic instance = this;
                instance[ id ] = node;
            }
        }

        public void provideDecoratedElement( HtmlElement element ) {
            this.decoratedElement = element;
            onPreRegister();
        }

        public void verifyAndRegister() {
            foreach (var id in viableInjectionPoints) {
                if (viableInjectionPoints[id].As<JsString>() == "req") {
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

        public AbstractBehavior() {
        }
    }
}