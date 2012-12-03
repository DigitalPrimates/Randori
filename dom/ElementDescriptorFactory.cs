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
using randori.styles;

namespace randori.dom {

    [JsType(JsMode.Prototype, OmitCasts = true, Export = false, Name = "Object")]
    public class ElementDescriptor {
        public JsString context;
        public JsString behavior;
        public JsString fragment;
        public JsString formatter;
        public JsString validator;
    }

    public class ElementDescriptorFactory {
        readonly StyleExtensionManager styleExtensionManager;

        public ElementDescriptor describeElement(HtmlElement element ) {
            //This is purely an efficiency gain. By making a merged map for this one element, we stop everyone from cycling through 
            //every class on an element to pull out their own piece of data
            var entry = styleExtensionManager.getMergedEntryForElement(element);
            var descriptor = new ElementDescriptor {
                context = element.getAttribute("data-context"),
                behavior = element.hasAttribute("data-mediator") ? element.getAttribute("data-mediator") : element.getAttribute("data-behavior"),
                fragment = element.getAttribute("data-fragment"),
                formatter = element.getAttribute( "data-formatter" ),
                validator = element.getAttribute( "data-validator" )
            };

            if ( entry != null ) {
                if (descriptor.context == null) {
                    descriptor.context = entry.getExtensionClass("module");
                } 

                if (descriptor.behavior == null) {
                    //mediator and behavior are really the same thing and hence mutually exclusive
                    descriptor.behavior = entry.hasExtensionType("mediator")?entry.getExtensionClass("mediator"):entry.getExtensionClass("behavior");
                } 

                if (descriptor.fragment == null) {
                    descriptor.fragment = entry.getExtensionClass("fragment");
                } 

                if (descriptor.formatter == null) {
                    descriptor.formatter = entry.getExtensionClass("formatter");
                } 

                if (descriptor.validator == null) {
                    descriptor.validator = entry.getExtensionClass("validator");
                } 
            }

            return descriptor;
        }

        public ElementDescriptorFactory(StyleExtensionManager styleExtensionManager) {
            this.styleExtensionManager = styleExtensionManager;
        }
    }
}
