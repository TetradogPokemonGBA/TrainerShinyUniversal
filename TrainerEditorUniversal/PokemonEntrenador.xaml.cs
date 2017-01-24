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
using Gabriel.Cat;

namespace TrainerEditorUniversal
{
    /// <summary>
    /// Lógica de interacción para PokemonEntrenador.xaml
    /// </summary>
    public partial class PokemonEntrenador : UserControl
    {
        public PokemonEntrenador(RomData rom, Entrenador.Equipo.Pokemon pokemon)
        {
            int indexPokemon;
            InitializeComponent();
            indexPokemon =pokemon.PokemonIndex;
            try
            {
                if (pokemon.EsDeLaTerceraGeneracion)
                {
                    imgPokemon.SetImage(rom.pokedexHoenn[202-indexPokemon ].Sprites.GetImagenFrontal());
                    txtNombre.Text += "\n" + rom.pokedexHoenn[202 - indexPokemon].Nombre + " " + ((Hex)(int)pokemon.PokemonIndex).ToString() + "-" + ((int)pokemon.PokemonIndex).ToString() + "  " + pokemon.Nivel + " ES NACIONAL";
                }
                else
                {
                    txtNombre.Text += rom.Pokedex[indexPokemon].Nombre + " " + ((Hex)(int)pokemon.PokemonIndex).ToString() + "-" + ((int)pokemon.PokemonIndex).ToString() + "  " + pokemon.Nivel;

                    imgPokemon.SetImage(rom.Pokedex[indexPokemon].Sprites.GetImagenFrontal());
                }

                if (pokemon.Item > 0)
                    imgObjeto.SetImage(rom.Objetos[pokemon.Item].ImagenObjeto);

            }catch { }

        }
    }
}
