using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DressApp.ViewModel.ButtonItems.TopMenuButtons
{
    public class ScreenshotButtonViewModel : TopMenuButtonViewModel
    {
        #region Contructor
        //inicializacion de una nueva instancia
        public ScreenshotButtonViewModel(Bitmap image)
            : base(image)
        { }
        #endregion
        #region Metodos
        //esconde todo los botones
        public override void ClickExecuted()
        {
            PlaySound();
            ClearMenu();
        }
        #endregion
    }
}
