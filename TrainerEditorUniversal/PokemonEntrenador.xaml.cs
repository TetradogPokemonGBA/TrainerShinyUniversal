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
using System.Drawing;

namespace TrainerEditorUniversal
{
	/// <summary>
	/// Lógica de interacción para PokemonEntrenador.xaml
	/// </summary>
	public partial class PokemonEntrenador : UserControl
	{
		private Entrenador entrenador;
		private bool isShiny;
		SpritesPokemon sprites;
		public event EventHandler ShinyChanged;

		public PokemonEntrenador(RomData rom, PokemonGBAFrameWork.PokemonEntrenador pokemon)
		{

			InitializeComponent();
			sprites = rom.Pokedex[pokemon.Especie].Sprites;
			txtNombre.Text += rom.Pokedex[pokemon.Especie].Nombre + " " + pokemon.Nivel;
			IsShiny=false;
			if (!(rom.Edicion.AbreviacionRom == AbreviacionCanon.AXV || rom.Edicion.AbreviacionRom == AbreviacionCanon.AXP))//NO TIENEN IMAGEN
			{
				if (pokemon.Item > 0)
					imgObjeto.SetImage(rom.Objetos[pokemon.Item].Sprite);
				else
					imgObjeto.SetImage(new Bitmap(1, 1));
			}


		}

		public PokemonEntrenador(RomData rom, PokemonGBAFrameWork.PokemonEntrenador pokemon, Entrenador entrenador) : this(rom, pokemon)
		{
			this.Entrenador = entrenador;
		}

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

		public bool IsShiny {
			get { return isShiny; }
			set
			{
				isShiny = value;

				if (ShinyChanged != null)
					ShinyChanged(this, new EventArgs());

				if(isShiny)
				{
					imgPokemon.SetImage(sprites.SpritesFrontales[0][1]);
				}
				else
				{
					imgPokemon.SetImage(sprites.SpritesFrontales[0][0]);
				}
			}
		}

		private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			IsShiny = !IsShiny;
		}
	}
}
