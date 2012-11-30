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
using guice;
using randori.behaviors;

namespace randori.dom {
    public class DomExtensionFactory {

        public AbstractBehavior buildBehavior(InjectionClassBuilder classBuilder, HtmlElement element, JsString behaviorClassName ) {
            var behavior = (AbstractBehavior)classBuilder.buildClass(behaviorClassName, element);
            return behavior;
        }

        public void buildNewContent(HtmlElement element, JsString contentURL) {

        }

        public InjectionClassBuilder buildChildClassBuilder(InjectionClassBuilder classBuilder, HtmlElement element, JsString contextClassName) {
            var module = (GuiceModule)classBuilder.buildClass(contextClassName);
            var injector = (ChildInjector)classBuilder.buildClass("guice.ChildInjector");
            var guiceJs = new GuiceJs();
            guiceJs.configureInjector(injector, module);

            //Setup a new InjectionClassBuilder
            return (InjectionClassBuilder)injector.getInstance(typeof(InjectionClassBuilder));
        }

        public DomExtensionFactory() {
        }
    }
}
