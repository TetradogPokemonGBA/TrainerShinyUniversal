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
using PokemonGBAFrameWork;
using Gabriel.Cat.Extension;
namespace TrainerEditorUniversal
{
    /// <summary>
    /// Lógica de interacción para PokemonEntrenador.xaml
    /// </summary>
    public partial class PokemonEntrenador : UserControl
    {
        public PokemonEntrenador(RomData rom,Entrenador.Equipo.Pokemon pokemon)
        {
            InitializeComponent();
            try
            {
                    imgPokemon.SetImage(rom.Pokedex[pokemon.PokemonIndex].Sprites.GetImagenFrontal());
                    txtNombre.Text = rom.Pokedex[pokemon.PokemonIndex].Nombre;
                    imgObjeto.SetImage(rom.Objetos[pokemon.Item].ImagenObjeto);
                
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
    }
}
