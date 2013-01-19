using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpKit.JavaScript;

namespace randori.behaviors.viewStack {
	public class MediatorCapturer : AbstractBehavior {
		AbstractMediator _mediator;

		public AbstractMediator mediator {
			get { return _mediator; }
		}

		protected override void onRegister() {
		}

        protected override void onDeregister() {
            _mediator = null;
        }

		public override void injectPotentialNode(JsString id, object node) {
			//Ugly hack for the moment, figure out a better way to handle by checking the identity... somehow... of this class
			dynamic behavior = node;
			if (_mediator == null && behavior.setViewData != null) {
				_mediator = behavior;
			}
		}

		public MediatorCapturer() {
		}
	}
}
