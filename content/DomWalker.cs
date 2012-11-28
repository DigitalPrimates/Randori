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
using randori.behaviors;
using randori.i18n;
using randori.styles;

namespace randori.content {

    public class DomWalker {

        readonly JsRegExp internationalKey = new JsRegExp(@"\[(labels|messages|reference)\.\w+\]", "g");
        readonly BehaviorResolver behaviorResolver;
        readonly ContentResolver contentResolver;
        readonly StyleBehaviorManager styleBehaviorManager;
        readonly LocalizationProvider localizationProvider;

        private void investigateTextNode(Node node) {
            var textContent = node.nodeValue;
            var i18nResult = textContent.match(internationalKey);

            if (i18nResult != null) {
                HtmlContext.console.log(textContent + " contains " + i18nResult);
                localizationProvider.translateNode(node, i18nResult);
            }
        }

        private void investigateLinkElement(HtmlLinkElement element) {
            if (styleBehaviorManager.parsingNeeded( element )) {
                styleBehaviorManager.parseAndReleaseLinkElement(element);
            }
        }

        private void investigateDomElement(HtmlElement element, BehaviorContext parentContext) {
            var resolvedNewBehavior = false;

            var nodeName = element.nodeName;

            HtmlContext.console.log( element.nodeName );

            //build a context for this behavior IF it turns out that this particular element defines one
            var behaviorContext = behaviorResolver.resolveBehavior(element);
            var id = element.getAttribute("id");

            if (id != null) {
                //remove the id so we dont have to deal with conflicts and clashes
                element.removeAttribute("id");
            }

            contentResolver.resolveContent( element );

            if (behaviorContext != null) {
                //we have a new behavior, this effectively causes us to use a new context for the nodes below it
                //Make sure we add ourselves to our parent though
                if (id != null && parentContext != null) {
                    parentContext.addBehavior(id, behaviorContext.resolvedBehavior);
                }

                resolvedNewBehavior = true;
            } else {
                //Keep the same context and pass it along as we didn't create a new behavior
                behaviorContext = parentContext;

                if (id != null && parentContext != null) {
                    parentContext.addNode(id, jQueryContext.J(element));
                }
            }

            walkChildren(element, behaviorContext);

            //Now that we have figured out all of the items under this dom element, setup the behavior
            if (resolvedNewBehavior) {
                behaviorContext.setupBehavior();
            }

            //after a recursive loop, reset our localContext back to the parent context
            behaviorContext = parentContext;
        }

        private void investigateNode( Node node, BehaviorContext parentContext=null ) {

            if (node.nodeType == Node.ELEMENT_NODE) {

                //Just an optimization, need to create constants for all of these things
                if (node.nodeName == "SCRIPT" || node.nodeName == "META") {
                    return;
                }

                if ( node.nodeName == "LINK" ) {
                    investigateLinkElement(node.As<HtmlLinkElement>());
                } else {
                    investigateDomElement(node.As<HtmlElement>(), parentContext);
                }

            } else if (node.nodeType == Node.TEXT_NODE) {
                //This is a text node, check to see if it needs internationalization
                investigateTextNode(node);
            } else if (node.nodeType == Node.DOCUMENT_NODE) {
                walkChildren( node, parentContext );
            }
        }

        private void walkChildren(Node parentNode, BehaviorContext parentContext = null) {
            var node = parentNode.firstChild;

            while (node != null) {
                investigateNode(node, parentContext);
                node = node.nextSibling;
            }
        }

        public void walkDomFragment(Node node, BehaviorContext parentContext = null) {
            investigateNode(node, parentContext);
        }

        public DomWalker(BehaviorResolver behaviorResolver, ContentResolver contentResolver, StyleBehaviorManager styleBehaviorManager, LocalizationProvider localizationProvider) {
            this.behaviorResolver = behaviorResolver;
            this.contentResolver = contentResolver;
            this.styleBehaviorManager = styleBehaviorManager;
            this.localizationProvider = localizationProvider;
        }
    }
}
