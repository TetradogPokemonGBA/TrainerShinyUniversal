using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.Win32;
using PokemonGBAFramework.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static string VersionApp = "2.0";
		Script scriptActual;
		IList<KeyValuePair<int, Entrenador>> entrenadorYSusbatallas;
		public MainWindow()
		{
			ContextMenu menu = new ContextMenu();
			MenuItem item;
			InitializeComponent();


			item = new MenuItem();
			item.Header = "Sobre";
			item.Click += (s, e) =>
			{
				if (MessageBox.Show("Este programa esta bajo licencia GNU, se ha hecho para facilitar la creación de scripts con el shinytzer créditos a Hackmew por la rutina, ¿Quieres ver el código fuente?", "Sobre la app", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
					new Uri("https://github.com/TetradogPokemonGBA/TrainerShinyUniversal").Abrir();

			};
			menu.Items.Add(item);

			item = new MenuItem();
			item.Header = "Cambiar Rom";
			item.Click += (s, e) => PideRom();
			menu.Items.Add(item);

			ContextMenu = menu;
			menu = new ContextMenu();
			item = new MenuItem();
			item.Header = "Copiar Offset";
			item.Click += (s, m) =>
			{
				Clipboard.SetText(txtOffsetScript.Text);
			};
			menu.Items.Add(item);
			txtOffsetScript.ContextMenu = menu;

			menu = new ContextMenu();
			item = new MenuItem();
			item.Header = "Copiar Script XSE";
			item.Click += (s, m) =>
			{
				Clipboard.SetText(txtScript.Text);
			};
			menu.Items.Add(item);
			txtScript.ContextMenu = menu;

			menu = new ContextMenu();
			item = new MenuItem();
			item.Header = "Copiar Bin Hex";
			item.Click += (s, m) =>
			{
				Clipboard.SetText(txtBinScript.Text);
			};
			menu.Items.Add(item);
			txtBinScript.ContextMenu = menu;
			PideRom();
			if (ReferenceEquals(Rom, default))
				Close();
		}
		public RomGba Rom { get; set; }
		public ClaseEntrenador[] ClasesEntrenador { get; set; }
		public Entrenador[] Entrenadores { get; set; }
		public Objeto[] Objetos { get; set; }
		public Pokemon[] Pokedex { get; set; }
		public string LastRom { get; set; }


		private void PideRom()
		{
			OpenFileDialog opn = new OpenFileDialog();


			opn.Filter = "GBA|*.gba";
			bool cargadoCorrecto = false;
			while (!cargadoCorrecto)
				if (opn.ShowDialog().GetValueOrDefault())
				{

					try
					{
						LastRom = opn.FileName;
						cargadoCorrecto = Load();
					}
					catch
					{
						cargadoCorrecto = MessageBox.Show("No se ha podido cargar la rom, puede que tengas abierto otro programa que lo use, cierralo y vuelve a probar", "Atención error al cargar la rom", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No;

					}

				}
				else cargadoCorrecto = true;
		}

		private bool Load()
		{
			bool cargadoCorrecto;
			IList<EntrenadorPreview> entrenadores;
			Rom = new RomGba(LastRom);

			try
			{

				InicializaCampos();
				Entrenadores = Entrenador.Get(Rom).ToArray();
				ClasesEntrenador = ClaseEntrenador.Get(Rom).ToArray();
				Objetos = Objeto.Get(Rom).ToArray();
				Pokedex = Pokemon.Get(Rom).ToArray();
				entrenadores = EntrenadorPreview.GetEntrenadoresPreview(Entrenadores, ClasesEntrenador);

				for (int i = 0; i < entrenadores.Count; i++)
				{
					entrenadores[i].MouseLeftButtonUp += (s, e) => PonEntrenador(s as EntrenadorPreview);
					ugEntrenadores.Children.Add(entrenadores[i]);
					cmbEntrenadores.Items.Add(entrenadores[i].Clone());

				}
				stkObjetosEntrenador.Children.Clear();
				ugEntrenadores.Children.Sort();
				cmbEntrenadores.SelectedIndex = 0;
				Title = $"Universal Shiny Trainer v {VersionApp}: {Rom.Edicion}";

				cargadoCorrecto = true;

			}
			catch (Exception ex)
			{

				MessageBox.Show("Error inesperado mientras cargaba la rom");
				cargadoCorrecto = false;
			}
			return cargadoCorrecto;
		}

		private void Guardar()
		{
			Rom.Data.Bytes.Save(LastRom);
		}

		private void InicializaCampos()
		{
			ugEntrenadores.Children.Clear();
			tabBatallas.SelectionChanged -= TabBatallas_SelectionChanged;
			tabBatallas.Items.Clear();
			tabBatallas.SelectionChanged += TabBatallas_SelectionChanged;
			cmbEntrenadores.SelectionChanged -= cmbEntrenadores_SelectionChanged;
			cmbEntrenadores.Items.Clear();
			cmbEntrenadores.SelectionChanged += cmbEntrenadores_SelectionChanged;
			txtScript.Text = "";
		}



		public void PonEntrenador(EntrenadorPreview entrenadorPreview)
		{

			PonEntrenador(entrenadorPreview.Batallas);
		}
		public void PonEntrenador(IList<KeyValuePair<int, Entrenador>> entrenadorYBatallas)
		{
			System.Windows.Controls.Image img;
			PokemonEntrenador pokemonEntrenador;
			UniformGrid ugEquipoEntrenador;
			KeyValuePair<int, Entrenador> entrenador;
			TabItem tbAct;
			this.entrenadorYSusbatallas = entrenadorYBatallas;

			txtNombreEntrenador.Foreground = entrenadorYBatallas[0].Value.EsUnaEntrenadora ? System.Windows.Media.Brushes.HotPink : System.Windows.Media.Brushes.Blue;

			tabBatallas.SelectionChanged -= TabBatallas_SelectionChanged;
			tabBatallas.Items.Clear();
			for (int j = 0; j < entrenadorYSusbatallas.Count; j++)
			{
				ugEquipoEntrenador = new UniformGrid();
				ugEquipoEntrenador.Columns = 3;

				entrenador = entrenadorYSusbatallas[j];
				for (int i = 0; i < entrenador.Value.EquipoPokemon.Equipo.Count; i++)
					if (entrenador.Value.EquipoPokemon.Equipo[i] != null)
					{
						pokemonEntrenador = new PokemonEntrenador(Pokedex[entrenador.Value.EquipoPokemon.Equipo[i].Especie], entrenador.Value.EquipoPokemon.Equipo[i], entrenador.Value);
						pokemonEntrenador.Tag = entrenador;
						pokemonEntrenador.ShinyChanged += (s, e) =>
						{
							KeyValuePair<int, Entrenador> entrenadorAux = (KeyValuePair<int, Entrenador>)((PokemonEntrenador)s).Tag;
							GenerarScript(entrenadorAux.Key, entrenadorAux.Value);
						};
						ugEquipoEntrenador.Children.Add(pokemonEntrenador);
					}
				if (!Rom.Edicion.EsRubiOZafiro)
				{
					stkObjetosEntrenador.Children.Clear();
					if (entrenador.Value.Item1 > 0)
					{
						img = new System.Windows.Controls.Image();
						img.SetImage(Objetos[entrenador.Value.Item1].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}
					if (entrenador.Value.Item2 > 0)
					{
						img = new System.Windows.Controls.Image();
						img.SetImage(Objetos[entrenador.Value.Item2].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}
					if (entrenador.Value.Item3 > 0)
					{
						img = new System.Windows.Controls.Image();
						img.SetImage(Objetos[entrenador.Value.Item3].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}
					if (entrenador.Value.Item4 > 0)
					{
						img = new System.Windows.Controls.Image();
						img.SetImage(Objetos[entrenador.Value.Item4].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}
				}
				tbAct = new TabItem();
				if (entrenadorYSusbatallas.Count > 1)
					tbAct.Header = j + "";

				tbAct.Content = ugEquipoEntrenador;
				tabBatallas.Items.Add(tbAct);
			}
			tabBatallas.SelectedIndex = 0;
			tabBatallas.SelectionChanged += TabBatallas_SelectionChanged;
			PonBatallaEntrenador(entrenadorYSusbatallas[0].Key, entrenadorYSusbatallas[0].Value);


		}
		void PonBatallaEntrenador(int index, Entrenador entrenador)
		{


			if (entrenador.SpriteIndex < ClasesEntrenador.Length)
				imgEntrenador.SetImage(ClasesEntrenador[entrenador.SpriteIndex].Sprite);
			else imgEntrenador.SetImage(new Bitmap(16, 16));
			txtNombreEntrenador.Text = (entrenador.Nombre != "" ? entrenador.Nombre : "Sin nombre") + " - " + ((Hex)(index + 1)).ToString();
			txtInteligencia.Text = $"AI: {entrenador.Inteligencia}";
			txtMoneyClass.Text = ClasesEntrenador[entrenador.TrainerClass].Nombre.Text;
			txtMusica.Text = $"Musica: {entrenador.MusicaBatalla}";
			GenerarScript(index, entrenador);
		}

		string PonEspacios(string str)
		{
			StringBuilder strConEspacios = new StringBuilder();
			for (int i = 0; i < str.Length; i += 2)
			{
				strConEspacios.Append(str[i]);
				strConEspacios.Append(str[i + 1]);
				strConEspacios.Append(" ");
			}
			strConEspacios.Remove(strConEspacios.Length - 1, 1);
			return strConEspacios.ToString();
		}
		private void GenerarScript(int index, Entrenador entrenador)
		{
			//const bool ACABAENEND = true;
			//PokemonEntrenador[] pokemonEquipo = ((UniformGrid)((TabItem)tabBatallas.Items[tabBatallas.SelectedIndex < 0 ? 0 : tabBatallas.SelectedIndex]).Content).Children.OfType<PokemonEntrenador>().ToArray();
			//bool[] isShiny = new bool[pokemonEquipo.Length];
			//int offsetScript;
			//byte[] bytesScript;
			//for (int i = 0; i < isShiny.Length; i++)
			//	isShiny[i] = pokemonEquipo[i].IsShiny;

			//scriptActual = Shinyzer.SimpleScriptBattleShinyTrainer(Rom, index, entrenador, isShiny);
			//bytesScript = scriptActual.GetDeclaracion(Rom, ACABAENEND);
			//offsetScript = Rom.Data.SearchArray(bytesScript);
			//if (offsetScript > 0)
			//{
			//	txtOffsetScript.Text = (Hex)offsetScript;
			//	btnInsertOrRemoveScript.Content = "Quitar";
			//}
			//else
			//{
			//	txtOffsetScript.Text = "";
			//	btnInsertOrRemoveScript.Content = "Insertar";
			//}
			//txtBinScript.Text = PonEspacios((string)(Hex)bytesScript);
			//txtScript.Text = scriptActual.GetDeclaracionXSE(ACABAENEND, "Entrenador" + entrenador.Nombre);
		}



		private void cmbEntrenadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbEntrenadores.SelectedItem != null)
				PonEntrenador(cmbEntrenadores.SelectedItem as EntrenadorPreview);
		}
		void BtnInsertOrRemoveScript_Click(object sender, RoutedEventArgs e)
		{
			const bool ACABAENEND = true;
			byte[] bytesScript = scriptActual.GetBytesTemp(ACABAENEND);
			int offsetScript = Rom.Data.SearchArray(bytesScript);
			if (offsetScript > 0)
			{
				Rom.Data.Remove(offsetScript, bytesScript.Length);
				btnInsertOrRemoveScript.Content = "Insertar";
				txtOffsetScript.Text = "";
			}
			else
			{

				btnInsertOrRemoveScript.Content = "Quitar";
				offsetScript = Rom.Data.SearchEmptyBytes(bytesScript.Length);
				Rom.Data.SetArray(offsetScript, bytesScript);
				txtOffsetScript.Text = (Hex)offsetScript;
			}
			try
			{
				Guardar();
			}
			catch
			{
				if (MessageBox.Show("No se ha podido guardar los datos,cierre cualquier otro programa que use esta rom y continua", "Atención, imposible escribir en la ROM", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
					try
					{
						Guardar();
					}
					catch
					{
						MessageBox.Show("Mejor reinicia y repite de nuevo.", "Continua igual...");
					}
			}


		}
		void TabBatallas_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PonBatallaEntrenador(entrenadorYSusbatallas[tabBatallas.SelectedIndex].Key, entrenadorYSusbatallas[tabBatallas.SelectedIndex].Value);
		}
	}
}
