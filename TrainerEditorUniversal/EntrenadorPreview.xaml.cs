using Gabriel.Cat.Extension;
using PokemonGBAFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrainerEditorUniversal
{
    /// <summary>
    /// Lógica de interacción para EntrenadorPreview.xaml
    /// </summary>
    public partial class EntrenadorPreview : UserControl
    {
        Entrenador entrenador;
        public EntrenadorPreview(int posicion,Entrenador entrenador,IList<ClaseEntrenador> entrenadores)
        {
            this.Entrenador = entrenador;
            InitializeComponent();
            try{
            	imgEntrenador.SetImage(entrenadores[entrenador.SpriteIndex].Sprite);
            }catch{}
            txtIdNombre.Text = posicion + "-" + entrenador.Nombre;
        }

        public Entrenador Entrenador
        {
            get
            {
                return entrenador;
            }

          private  set
            {
                entrenador = value;
            }
        }
    }
}
