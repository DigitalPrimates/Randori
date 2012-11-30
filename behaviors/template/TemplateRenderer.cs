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
using randori.attributes;
using randori.template;

namespace randori.behaviors.template {
    public class TemplateRenderer : AbstractBehavior {
        [Inject]
        public TemplateBuilder templateBuilder;

        protected JsObject _data;
        public JsObject data {
            get { return _data; }
            set {
                if (value == _data) return;
                _data = value;
                renderMessage();
            }
        }

        protected void renderMessage() {
            rootElement = templateBuilder.replaceTemplate(rootElement, data, this);
        }

        protected override void onRegister() {
            rootElement = templateBuilder.parseAndReplaceTemplate(rootElement);
        }

        public TemplateRenderer(HtmlElement rootElement)
            : base(rootElement) {
        }
    }
}
