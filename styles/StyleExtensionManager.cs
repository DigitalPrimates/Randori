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

namespace randori.styles {

    /* Not able to make this work as I want just yet. Feature request filed
    [JsType(JsMode.Json)]
    public class StyleExtensionType {
        public static readonly string Module = "module";
        public static readonly string Mediator = "mediator";
        public static readonly string Behavior = "behavior";
        public static readonly string Content = "content";
        public static readonly string Formatter = "formatter";
        public static readonly string Validator = "validator";
    } */

    [JsType(Export = false)]
    public delegate void DataRetrievedDelegate( object o );

    public class StyleExtensionManager  {

        readonly StyleBehaviorMap map;

        //TODO consider moving this somewhere more appropriate
        public StyleExtensionMapEntry getMergedEntryForElement(HtmlElement element) {
            JsString cssClassList = element.getAttribute("class");
            StyleExtensionMapEntry mergedEntry = null;

            if (cssClassList != null) {
                var cssClassArray = cssClassList.split(" ");
                for (int i = 0; i < cssClassArray.length; i++) {
                    var cssClass = cssClassArray[i].As<JsString>();
                    var extensionMapEntry = map.getBehaviorEntry(cssClass);

                    if (extensionMapEntry != null) {
                        if (mergedEntry == null) {
                            //Waiting as long as we can to instantiate
                            mergedEntry = new StyleExtensionMapEntry();
                        }

                        extensionMapEntry.mergeTo(mergedEntry);
                    }
                }
            }

            return mergedEntry;
        }

        public bool parsingNeeded(HtmlLinkElement link) {
            return ( link.rel == "stylesheet/randori" );
        }

        JsString resetLinkAndReturnURL(HtmlLinkElement link) {

            //reset it and let the browser handle it now, we have the url we need
            //we will grab it next. So long as we are caching files, it will be retrieved synchronoulsy from the cache
            link.rel = "stylesheet";

            return link.href;
        }

        /*
         * 
         * We dont need this right now but keeping the code cause we might need it again someday soon
        JsString correctPaths(JsString sheet, JsString urlPrefix) {
            //First, fix the path of any objects in the system. This fines url references and preprends paths
            //Right now this just works for relative urls, I am going to need to fix it for absolute
            //Now using this one instead for optional quotes
            //(url\s?\(\s?['"]?)([\w\W]*?)(['"]?\s?\))

            var pathReplace = new JsRegExp("(url\\s?\\(\\s?[\'\"]?)([\\w\\W]*?)([\'\"]?\\s?\\))", "g");
            JsString replacementString = "$1" + urlPrefix + "/$2$3";
            sheet = sheet.replace(pathReplace, replacementString);
            return sheet;
        }*/

        void resolveSheet(JsString url) {
            var sheetRequest = new XMLHttpRequest();
            JsString behaviorSheet = "";
            JsString prefix;

            sheetRequest.open("GET", url, false);
            sheetRequest.send("");

            if (sheetRequest.status == 404) {
                throw new JsError("Cannot Find StyleSheet " + url);
            }

            int lastSlash = url.lastIndexOf("/");
            prefix = url.substring(0, lastSlash);

            parseAndPersistBehaviors(sheetRequest.responseText);
        }

        void parseAndPersistBehaviors(JsString sheet) {

            JsString classSelector;
            JsRegExpResult dpVendorItemsResult;
            JsRegExpResult dpVendorItemInfoResult;
            JsRegExpResult CSSClassSelectorNameResult;
            /*
             * This regular expression then grabs all of the class selectors
             * \.[\w\W]*?\}
             * 
             * This expression finds an -randori vendor prefix styles in the current selector and returns 2 groups, the first
             * is the type, the second is the value
             * 
             * \s?-randori-([\w\W]+?)\s?:\s?["']?([\w\W]+?)["']?;
             * 
             */

            var allClassSelectors = new JsRegExp("\\.[\\w\\W]*?\\}", "g");

            //These two are the same save for the global flag. The global flag seems to disable all capturing groups immediately
            var dpVendorItems = new JsRegExp(@"\s?-randori-([\w\W]+?)\s?:\s?[""']?([\w\W]+?)[""']?;", "g");

            //This is the same as the one in findRelevant classes save for the the global flag... which is really important
            //The global flag seems to disable all capturing groups immediately
            var dpVendorItemsDetail = new JsRegExp(@"\s?-randori-([\w\W]+?)\s?:\s?[""']?([\w\W]+?)[""']?;");

            var classSelectorName = new JsRegExp(@"\.([\W\w]+?)\s+?{");
            JsString CSSClassSelectorName;
            JsString dpVendorItemStr;

            var selectors = sheet.match(allClassSelectors);

            if (selectors != null ) {
                for (int i = 0; i < selectors.length; i++) {
                    classSelector = selectors[i];

                    dpVendorItemsResult = classSelector.match(dpVendorItems);
                    if (dpVendorItemsResult != null) {

                        CSSClassSelectorNameResult = classSelector.match(classSelectorName);
                        CSSClassSelectorName = CSSClassSelectorNameResult[1];

                        for (int j = 0; j < dpVendorItemsResult.length; j++) {
                            dpVendorItemStr = dpVendorItemsResult[j];
                            dpVendorItemInfoResult = dpVendorItemStr.match(dpVendorItemsDetail);
                            map.addBehaviorEntry(CSSClassSelectorName, dpVendorItemInfoResult[1], dpVendorItemInfoResult[2]);
                            if (HtmlContext.console != null) {
                                HtmlContext.console.log(CSSClassSelectorName + " specifies a " + dpVendorItemInfoResult[1] + " implemented by class " + dpVendorItemInfoResult[2]);
                            }
                        }
                    }
                }
            }
        }

        public void parseAndReleaseLinkElement(HtmlLinkElement element) {
            resolveSheet(resetLinkAndReturnURL(element));
        }

        StyleExtensionManager(StyleBehaviorMap map) {
            this.map = map;
        }
    }
}