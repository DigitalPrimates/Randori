using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using randori.attributes;
using randori.dom;
using randori.template;

namespace randori.behaviors {

    [JsType(JsMode.Prototype, Export = false)]
    public delegate jQuery RenderFunction(int index, JsObject values);

    [JsType(JsMode.Prototype, NativeOverloads = false, OmitCasts = true)]
    public class SimpleList : AbstractBehavior {

        protected jQuery rootElement;

        [View(required = false)]
        public jQuery template;

        [Inject]
        public TemplateBuilder templateBuilder;

        protected RenderFunction _renderFunction;
        public RenderFunction renderFunction {
            get { return _renderFunction; }
            set { _renderFunction = value; }
        }

        protected JsArray _data = new JsArray();
	    DomWalker domWalker;

	    public JsArray data {
            get { return _data; }
            set {
                _data = value;
                renderList();
            }
        }

        public virtual void renderList() {
            jQuery row;
            jQuery div = jQueryContext.J("<div></div>");
            if ((data == null) || (data.length == 0)) {
                showNoResults();
                return;
            }

            if ((!templateBuilder.validTemplate) && renderFunction == null) return;

            if (templateBuilder.validTemplate) {
                for (int i = 0; i < data.length; i++) {
                    row = templateBuilder.renderTemplateClone(data[i].As<JsObject>()).children();
					domWalker.walkDomFragment(row[0], this);
					row.addClass("randoriListItem");
                    div.append(row);
                }
            } else if (renderFunction != null) {
                for (int i = 0; i < data.length; i++) {
                    row = renderFunction(i, data[i].As<JsObject>());
					domWalker.walkDomFragment(row[0], this);
					row.addClass("randoriListItem");
                    div.append(row);
                }
            }

            rootElement.empty();
            rootElement.append(div.children());
        }

		protected override void onPreRegister()
		{
			base.onPreRegister();

			this.rootElement = jQueryContext.J(decoratedElement);
			templateBuilder.captureAndEmptyTemplateContents(rootElement);
		}
		
		protected override void onRegister()
		{
            //adds a listener to the root element
            //fires click whenever a .listItem is clicked
            renderList();
        }

        public void showLoading() {
            JsString output = "<div style=\"height:100%; width:100%;\"><div style=\"text-align:center;width:100%;top:60%;position:absolute\">Loading...</div></div>";

            rootElement.html(output);
        }

        private void showNoResults(bool visible = true) {
            JsString output = "<div style=\"height:100%; width:100%;\"><div style=\"text-align:center;width:100%;top:60%;position:absolute\">No Items Found</div></div>";

            rootElement.html(output);
        }

		public SimpleList(DomWalker domWalker)
            : base() {
				this.domWalker = domWalker;
		}
    }
}