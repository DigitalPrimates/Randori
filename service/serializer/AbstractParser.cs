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

namespace randori.service.serializer {
    public abstract class AbstractParser<T> {
        public ParserToken<T> createToken(AbstractToken<object> serviceToken) {
            var parserToken = new ParserToken<T>();

            //Parse the result of the service before notifying the listener
            //eventually we need to handle an error during parsing becoming a fault
            serviceToken.result = delegate(object result) {
                parserToken.serviceResult(parseResult(result));
            };

            //just pass the error straight through
            serviceToken.fault = parserToken.serviceFault;
            return parserToken;
        }

        protected abstract T parseResult( object result );
    }
}
