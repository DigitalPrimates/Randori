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

namespace randori.timer {
    
    [JsType(Export = false)]
    public delegate void TimerTick(Timer timer, int currentCount);

    [JsType(Export = false)]
    public delegate void TimerComplete(Timer timer);

    public class Timer {
        int _delay;
        int _repeatCount;
        int _currentCount;

        int intervalID;

        public TimerTick timerTick;
        public TimerComplete timerComplete;

        public int delay {
            get { return _delay; }
        }

        public int repeatCount {
            get { return _repeatCount; }
        }

        public int currentCount {
            get { return _currentCount; }
        }

        protected void onTimerTick() {
            _currentCount++;

            if (timerTick != null) {
                timerTick(this, _currentCount);
            }

            if (_currentCount == _repeatCount) {
                if (timerComplete != null) {
                    timerComplete( this );
                }
            }

            stop();
        }

        public void start() {
            intervalID = HtmlContext.window.setInterval(onTimerTick, delay);
        }

        public void stop() {
            HtmlContext.window.clearInterval( intervalID );
        }

        public void reset() {
            _currentCount = 0;
            stop();
        }

        public Timer(int delay, int repeatCount=0) {
            _delay = delay;
            _repeatCount = repeatCount;
            _currentCount = 0;
        }
    }
}
