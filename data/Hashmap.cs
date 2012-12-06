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

using SharpKit.JavaScript;

namespace randori.data {

    [JsType(JsMode.Prototype, OmitCasts = true, Export = false, Name = "Object")]
    public class Entry<T> {
        public object key;
        public T value;
    }

    //HashMap that allows object keys with only mostly terrible performance
    //Intend on making this more peformant, but optimize 2nd
    public class HashMap<T> where T : class {
        readonly JsObject entries;

        private Entry<T> getEntry(object key) {
            var keyAsString = key.As<JsString>();
            dynamic entry = entries[keyAsString];
            Entry<T> returnEntry = null;

            if (entry != JsContext.undefined) {
                if (entry is JsArray) {
                    for ( var i = 0; i < entry.length; i++ ) {
                        if ( entry[ i ].key == key ) {
                            returnEntry = entry[i];
                            break;
                        }
                    }
                } else if (entry.key == key) {
                    returnEntry = entry;
                } 
            }

            return returnEntry;
        }

        public T get(object key) {
            var entry = getEntry( key );

            return entry != null ? entry.value : null;
        }

        public void put( object key, T value ) {
            var keyAsString = key.As<JsString>();
            dynamic entryLocation = entries[keyAsString];

            if (entryLocation == JsContext.undefined) {
                //Doesnt exist, add it
                entries[key.As<JsString>()] = new Entry<T> { key = key, value = value };
            } else {
                //there is already an entry location.... so
                dynamic entry = getEntry(key);

                //Do we have a matching entry.. if so, update the value
                if (entry != JsContext.undefined) {
                    entry.value = value;
                } else if ( entryLocation is JsArray ) {
                    //Add this one to the location
                    entryLocation.push( new Entry<T> {key = key, value = value} );
                } else {
                    //Convert this location to an array
                    var ar = new JsArray();
                    ar[0] = entryLocation;
                    ar[1] = new Entry<T> { key = key, value = value };
                    entries[keyAsString] = ar;
                }
            }
        }

        public HashMap() {
            entries = new JsObject();
        }
    }
}
