using Gabriel.Cat.S.Extension;
using PokemonGBAFramework.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Lógica de interacción para PokemonEntrenador.xaml
    /// </summary>
    public partial class PokemonEntrenador : UserControl
    {
		private Entrenador entrenador;
		private bool isShiny;

		public event EventHandler ShinyChanged;

		public PokemonEntrenador(Pokemon pokemon, PokemonGBAFramework.Core.PokemonEntrenador pokemonEntrenador, Objeto[] objetos=default)
		{

			InitializeComponent();
			Pokemon=pokemon;
			txtNombre.Text += $"{Pokemon.Nombre} {pokemonEntrenador.Nivel}";
			IsShiny = false;
			if (!Equals(objetos,default) &&!ReferenceEquals(objetos.First().Sprite,default))//NO TIENEN IMAGEN
			{
				if (pokemonEntrenador.Item > 0)
					imgObjeto.SetImage(objetos[pokemonEntrenador.Item].Sprite);
				else
					imgObjeto.SetImage(new Bitmap(1, 1));
			}


		}

		public PokemonEntrenador(Pokemon pokemon, PokemonGBAFramework.Core.PokemonEntrenador pokemonEntrenador, Entrenador entrenador, Objeto[] objetos = default) : this(pokemon,pokemonEntrenador,objetos)
		{
			Entrenador = entrenador;
		}
		public Pokemon Pokemon { get; set; }
		public Entrenador Entrenador
		{
			get
			{
				return entrenador;
			}

			set
			{
				entrenador = value;
			}
		}

		public bool IsShiny
		{
			get { return isShiny; }
			set
			{
				isShiny = value;

				if (ShinyChanged != null)
					ShinyChanged(this, new EventArgs());

				if (isShiny)
				{
					imgPokemon.SetImage(Pokemon.Sprites.Frontales.Sprites[0]+ Pokemon.Sprites.PaletaShiny);
				}
				else
				{
					imgPokemon.SetImage(Pokemon.Sprites.Frontales.Sprites[0] + Pokemon.Sprites.PaletaNomal);
				}
			}
		}

		private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			IsShiny = !IsShiny;
		}
	}
}
