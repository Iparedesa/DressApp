﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DressApp.ViewModel.ButtonItems.TopMenuButtons
{
    class SoundsButtonViewModel : TopMenuButtonViewModel
    {
        #region Constructor
        //inicializacion de instancia
        public SoundsButtonViewModel(Bitmap image)
            : base(image)
        { }
        #endregion
        #region Methodos
        // on / off del sonido
        public override void ClickExecuted()
        {
            TopMenuManager.Instance.SoundsOn = !TopMenuManager.Instance.SoundsOn;
            PlaySound();
            ClearMenu();
        }
        #endregion
    }
}
