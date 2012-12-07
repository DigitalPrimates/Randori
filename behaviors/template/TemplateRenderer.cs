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
using randori.attributes;
using randori.dom;
using randori.template;

namespace randori.behaviors.template {
    public class TemplateRenderer : AbstractBehavior {

        jQuery rootNode;
        readonly DomWalker domWalker;
        readonly InjectionClassBuilder classBuilder;
        readonly TemplateBuilder templateBuilder;

        protected JsObject _data;

        public JsObject data {
            get { return _data; }
            set {
                if (value == _data) return;
                _data = value;
                renderMessage();
            }
        }

        protected override void onPreRegister() {
            base.onPreRegister();

            this.rootNode = jQueryContext.J(decoratedElement);
            templateBuilder.captureAndEmptyTemplateContents(rootNode);
        }

        protected void renderMessage() {
            //If the first part of the newNode is text and not an actual node, jQuery loses it during an append
            //So this is the only method I have been able to figure out that actually keeps those first text nodes
            //which is really important during templating
            var newNode = templateBuilder.renderTemplateClone(data);
            rootNode.html(newNode.html());
            domWalker.walkDomChildren(decoratedElement, this);
        }

        protected override void onRegister() {
        }

        public TemplateRenderer(InjectionClassBuilder classBuilder, DomWalker domWalker, TemplateBuilder templateBuilder) {
            this.domWalker = domWalker;
            this.classBuilder = classBuilder;
            this.templateBuilder = templateBuilder;
        }
    }
}
