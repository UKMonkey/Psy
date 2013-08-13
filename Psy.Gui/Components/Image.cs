namespace Psy.Gui.Components
{
    public class Image : Widget
    {
        private readonly NinePatchHandle _ninePatchHandle;
        public string ImageName { get; set; }
        public bool Border { get; set; }

        internal Image(GuiManager guiManager, Widget parent) 
            : base(guiManager, parent)
        {
            _ninePatchHandle = NinePatchHandle.Create("button");
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            if (Border && guiRenderer.RenderMode == RenderMode.Normal)
            {
                guiRenderer.NinePatch(Size, _ninePatchHandle, alpha: Alpha);
            }

            if (!string.IsNullOrEmpty(ImageName))
            {
                guiRenderer.Image(ImageName, Size, margin: Margin);
            }

            base.Render(guiRenderer);
        }
    }
}