﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DressApp.ViewModel.ButtonItems.TopMenuButtons
{
    class MakeWiderButtonViewModel : TopMenuButtonViewModel
    {
        #region Constantes
        private const double PlusFactor = 0.05;
        #endregion
        #region Constructor
        //inicializacion de instancia
        public MakeWiderButtonViewModel(Bitmap image)
            : base(image)
        { }
        #endregion
        #region Metodos
        //hace mas ancho
        public override void ClickExecuted()
        {
            PlaySound();
            if (ClothingManager.Instance.ChosenClothesModels.Count != 0)
                ClothingManager.Instance.ScaleImageWidth(PlusFactor);
        }
        #endregion
    }
}
