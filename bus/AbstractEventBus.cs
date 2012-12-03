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
namespace randori.bus {
    /** Contexts will often provide an EventBus use by children within the Context. This is really just an Abstract placeholder
     * for that concept. Developers are expected to extend this AbstractEventBus and then add the delegates that they see fit for 
     * that particular context. For example:**/

    /*
    [JsType(JsMode.Prototype,Export = false)]
    public delegate void SomeRelevantNotifcation(JsString importantDate, JsArray<Things> evenMoreData );
     * 
     * Then define a public property of the type of that delegate:
     * 
     * public SomeRelevantNotifcation someRelevantNotifcation;
     * 
     * And you now have an EventBus with one event. In this system, we favor a very typed EventBus model
    */
    public abstract class AbstractEventBus {
    }
}
