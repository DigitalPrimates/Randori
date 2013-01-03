using SharpKit.JavaScript;
using SharpKit.jQuery;
using randori.attributes;
using randori.dom;
using randori.template;

namespace randori.behaviors {
    [JsType(JsMode.Prototype, Export=false)]
    public delegate void ListChangeDelegate(int index, JsObject data);

    [JsType(JsMode.Prototype, NativeOverloads = false, OmitCasts = true)]
    public class List : SimpleList {
        public ListChangeDelegate listChanged;

        public object selectedItem {
            get { return _data[_selectedIndex]; }
            set {
                if (_data == null) {
                    return;
                }

                for (int i = 0; i < _data.length; i++) {
                    if (value == _data[i]) {
                        selectedIndex = i;
                        break;
                    }
                }
            }
        }

        protected int _selectedIndex;
        public int selectedIndex {
            get { return _selectedIndex; }
            set {
                _selectedIndex = value;
                decoratedNode.children().removeClass("selected");
                if (value > -1 && value < decoratedNode.children().length) {
                    decoratedNode.children().eq(value).addClass("selected");
                    if (listChanged != null) {
                        listChanged(value, data[value].As<JsObject>());
                    }
                }
            }
        }

        protected override void onRegister() {
            base.onRegister();

            //adds a listener to the root element
            //fires click whenever a .listItem is clicked
            decoratedNode.@delegate(".randoriListItem", "click", onItemClick);
        }

        public override void renderList() {
            base.renderList();
            selectedIndex = 0;
        }

        protected void onItemClick(Event e) {
            var targetJq = jQueryContext.J(e.currentTarget);
            int index = targetJq.index();
            selectedIndex = index;
        }

		public List(DomWalker domWalker)
			: base(domWalker)
		{
        }
    }
}