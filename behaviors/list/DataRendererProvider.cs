using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpKit.JavaScript;

namespace randori.behaviors.list {
	public class DataRendererProvider : AbstractBehavior {
		object data;

		protected override void onRegister() {
		}

		public override void injectPotentialNode(JsString id, object node) {
			//Ugly hack for the moment, figure out a better way to handle by checking the identity... somehow... of this class
			dynamic behavior = node;
			if (behavior.setData != null) {
				behavior.setData(data);
			}
		}
		
		public DataRendererProvider(object data) {
			this.data = data;
		}
	}
}
