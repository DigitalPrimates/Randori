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
using randori.behaviors;
using randori.data;
using randori.i18n;
using randori.styles;

namespace randori.dom {

    public class DomWalker {

        readonly StyleExtensionManager styleExtensionManager;
        readonly LocalizationProvider localizationProvider;
        readonly DomExtensionFactory domExtensionFactory;
        readonly ElementDescriptorFactory elementDescriptorFactory;
        readonly InjectionClassBuilder classBuilder;

        HashMap<StyleExtensionMapEntry> extensionsToBeApplied;
        //An entry element is the first real element we foind in this particular DomWalker instance
        HtmlElement entryElement;

        private void investigateLinkElement(HtmlLinkElement element) {
            if (styleExtensionManager.parsingNeeded( element )) {
                styleExtensionManager.parseAndReleaseLinkElement(element);
                //we must rebuild our stylemanager cache if we find a style sheet in this walk. Do so from our entryElement
                extensionsToBeApplied = styleExtensionManager.getExtensionsForFragment(entryElement);
            }
        }

        private void investigateDomElement(HtmlElement element, AbstractBehavior parentBehavior) {
            var currentBehavior = parentBehavior;
            var domWalker = this;

            var id = element.getAttribute("id");

            if (id != null) {
                //we have a reference to the element now, so remove the id so we dont have to deal with conflicts and clashes
                element.removeAttribute("id");
            }

            var elementDescriptor = elementDescriptorFactory.describeElement(element, extensionsToBeApplied);

            if (elementDescriptor.context != null) {
                //change the class builder for everything under this point in the DOM
                var newClassBuilder = domExtensionFactory.buildChildClassBuilder(classBuilder, element, elementDescriptor.context);
                //change the domWalker for everything under this point in the DOM
                domWalker = (DomWalker)newClassBuilder.buildClass("randori.dom.DomWalker");
            }

            if (elementDescriptor.behavior != null) {
                //build a context for this behavior IF it turns out that this particular element defines one
                currentBehavior = domExtensionFactory.buildBehavior( classBuilder, element, elementDescriptor.behavior );

                //we have a new behavior, this effectively causes us to use a new context for the nodes below it
                //Make sure we add ourselves to our parent though
                if (id != null && parentBehavior != null) {
                    parentBehavior.injectPotentialNode(id, currentBehavior);
                }
            } else {
                if (id != null && currentBehavior != null) {
                    currentBehavior.injectPotentialNode(id, jQueryContext.J(element));
                }                
            }

            if (elementDescriptor.fragment != null) {
                //build a context for this behavior IF it turns out that this particular element defines one
                domExtensionFactory.buildNewContent(element, elementDescriptor.fragment);
            }

            domWalker.walkChildren(element, currentBehavior);

            //Now that we have figured out all of the items under this dom element, setup the behavior
            if (currentBehavior != null && currentBehavior != parentBehavior) {
                currentBehavior.verifyAndRegister();
            }
        }

        private void investigateNode(Node node, AbstractBehavior parentBehavior) {

            if (node.nodeType == Node.ELEMENT_NODE) {

                if (extensionsToBeApplied == null) {
                    //We build our extension cache from the first element we find
                    entryElement = node.As<HtmlElement>();
                    extensionsToBeApplied = styleExtensionManager.getExtensionsForFragment(entryElement);
                }

                //Just an optimization, need to create constants for all of these things
                if (node.nodeName == "SCRIPT" || node.nodeName == "META") {
                    return;
                }

                if ( node.nodeName == "LINK" ) {
                    investigateLinkElement(node.As<HtmlLinkElement>());
                } else {
                    investigateDomElement(node.As<HtmlElement>(), parentBehavior);
                }

            } else if (node.nodeType == Node.TEXT_NODE) {
                //This is a text node, check to see if it needs internationalization
                localizationProvider.investigateTextNode(node);
            } else {
                walkChildren(node, parentBehavior);
            }
        }

        public void walkChildren(Node parentNode, AbstractBehavior parentBehavior = null) {
            var node = parentNode.firstChild;

            //The fact that we have two entry point into here walkChildren and walkDomFragment continues to screw us
            if (extensionsToBeApplied == null && (parentNode.nodeType == Node.ELEMENT_NODE)) {
                //We build our extension cache from the first element we find
                entryElement = parentNode.As<HtmlElement>();
                extensionsToBeApplied = styleExtensionManager.getExtensionsForFragment(entryElement);
            }

            while (node != null) {
                investigateNode(node, parentBehavior);
                node = node.nextSibling;
            }
        }

        public void walkDomFragment(Node node, AbstractBehavior parentBehavior = null) {

            investigateNode(node, parentBehavior);
        }

        public DomWalker(DomExtensionFactory domExtensionFactory, InjectionClassBuilder classBuilder, ElementDescriptorFactory elementDescriptorFactory, StyleExtensionManager styleExtensionManager, LocalizationProvider localizationProvider) {
            this.domExtensionFactory = domExtensionFactory;
            this.elementDescriptorFactory = elementDescriptorFactory;
            this.styleExtensionManager = styleExtensionManager;
            this.localizationProvider = localizationProvider;
            this.classBuilder = classBuilder;
        }
    }
}
