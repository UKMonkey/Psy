using System;

namespace Psy.Core
{
    public delegate void TextureAreaHolderCallback(TextureAreaHolder item);

    public class TextureAreaHolder : IDisposable
    {
        private TextureArea _areaCurrent;
        private TextureArea _areaNext;

        public event TextureAreaHolderCallback OnChange;
             
        public TextureArea TextureArea
        {
            get { return _areaCurrent; }
            set { _areaNext = value; }
        }

        public TextureAreaHolder(TextureArea area)
        {
            _areaCurrent = area;
            _areaNext = null;
        }

        public void Update()
        {
            if (_areaNext == null) 
                return;

            _areaCurrent = _areaNext;
            _areaNext = null;

            if (OnChange != null)
                OnChange(this);
        }

        public void Dispose()
        {
            _areaCurrent.Dispose();
            if (_areaNext != null)
                _areaNext.Dispose();
        }
    }
}
