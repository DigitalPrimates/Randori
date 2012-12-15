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
using guice;
using guice.resolver;
using randori.behaviors;
using randori.content;

namespace randori.dom {
    public class DomExtensionFactory {
        readonly ContentLoader contentLoader;
        private ClassResolver classResolver;
        private ExternalBehaviorFactory externalBehaviorFactory;

        public AbstractBehavior buildBehavior(InjectionClassBuilder classBuilder, HtmlElement element, JsString behaviorClassName ) {
            AbstractBehavior behavior = null;

            var resolution = classResolver.resolveClassName(behaviorClassName);
            
            if ( resolution.builtIn ) {
                /** If we have a type which was not created via Randori, we send it out to get created. In this way
                 * we dont worry about injection data and we allow for any crazy creation mechanism the client can
                 * consider **/
                behavior = externalBehaviorFactory.createExternalBehavior(element, behaviorClassName, resolution.type);
            } else {
                behavior = (AbstractBehavior)classBuilder.buildClass(behaviorClassName);
                behavior.provideDecoratedElement(element);
            }

            return behavior;
        }

        public void buildNewContent(HtmlElement element, JsString fragmentURL) {
            jQueryContext.J(element).append(contentLoader.synchronousFragmentLoad(fragmentURL));
        }

        public InjectionClassBuilder buildChildClassBuilder(InjectionClassBuilder classBuilder, HtmlElement element, JsString contextClassName) {
            var module = (GuiceModule)classBuilder.buildClass(contextClassName);
            var injector = (ChildInjector)classBuilder.buildClass("guice.ChildInjector");
            var guiceJs = new GuiceJs();
            guiceJs.configureInjector(injector, module);

            //Setup a new InjectionClassBuilder
            return (InjectionClassBuilder)injector.getInstance(typeof(InjectionClassBuilder));
        }

        public DomExtensionFactory( ContentLoader contentLoader, ClassResolver classResolver, ExternalBehaviorFactory externalBehaviorFactory ) {
            this.contentLoader = contentLoader;
            this.classResolver = classResolver;
            this.externalBehaviorFactory = externalBehaviorFactory;
        }
    }
}
