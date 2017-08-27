using System;
using System.Collections;
using System.Collections.Generic;
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
using Gabriel.Cat.Extension;
using PokemonGBAFrameWork;
using Microsoft.Win32;
using System.Drawing;
using Gabriel.Cat;

namespace TrainerEditorUniversal
{
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		RomData rom;
		MenuItem itemActivarDesactivar;
		Script scriptActual;
		IList<KeyValuePair<int,Entrenador>>  entrenadorYSusbatallas;
		public MainWindow()
		{
			ContextMenu menu = new ContextMenu();
			MenuItem item;
			InitializeComponent();

			
			item = new MenuItem();
			item.Header = "Sobre";
			item.Click += (s, e) =>{
				if(MessageBox.Show("Este programa esta bajo licencia GNU, se ha hecho para facilitar la creación de scripts con el shinytzer créditos a Hackmew por la rutina, ¿Quieres ver el código fuente?","Sobre la app",MessageBoxButton.YesNo,MessageBoxImage.Information)==MessageBoxResult.Yes)
					System.Diagnostics.Process.Start("https://github.com/TetradogPokemonGBA/TrainerShinyUniversal");
				
			};
			menu.Items.Add(item);
			
			item = new MenuItem();
			item.Header = "Cambiar Rom";
			item.Click += (s, e) => PideRom();
			menu.Items.Add(item);

			//de mientras hasta que ponga bien el shinyzer
			if(System.Diagnostics.Debugger.IsAttached){
			itemActivarDesactivar = new MenuItem();
			itemActivarDesactivar.Header = "Activar";
			itemActivarDesactivar.Click += (s, e) => ActivarDesactivar();
			menu.Items.Add(itemActivarDesactivar);
			}
			ContextMenu = menu;
			menu=new ContextMenu();
			item=new MenuItem();
			item.Header="Copiar Offset";
			item.Click+=(s,m)=>{
				Clipboard.SetText(txtOffsetScript.Text);
			};
			menu.Items.Add(item);
			txtOffsetScript.ContextMenu=menu;
			
						menu=new ContextMenu();
			item=new MenuItem();
			item.Header="Copiar Script XSE";
			item.Click+=(s,m)=>{
				Clipboard.SetText(txtScript.Text);
			};
			menu.Items.Add(item);
			txtScript.ContextMenu=menu;
			
						menu=new ContextMenu();
			item=new MenuItem();
			item.Header="Copiar Bin Hex";
			item.Click+=(s,m)=>{
				Clipboard.SetText(txtBinScript.Text);
			};
			menu.Items.Add(item);
			txtBinScript.ContextMenu=menu;
			PideRom();
			if (rom == null)
				this.Close();
		}

		private void ActivarDesactivar()
		{
			if (Shinyzer.EstaActivado(rom.Rom,rom.Edicion,rom.Compilacion))
				Shinyzer.Desactivar(rom.Rom,rom.Edicion,rom.Compilacion);
			else
				Shinyzer.Activar(rom.Rom,rom.Edicion,rom.Compilacion);
			Guardar();
			PonTexto();

		}

		private void PonTexto()
		{
			itemActivarDesactivar.Header = Shinyzer.EstaActivado(rom.Rom,rom.Edicion,rom.Compilacion) ? "Desactivar" : "Activar";
		}

		private void PideRom()
		{
			OpenFileDialog opn = new OpenFileDialog();
			RomGba romGBA;
			IList<EntrenadorPreview> entrenadores;
			opn.Filter = "GBA|*.gba";
			bool cargadoCorrecto=false;
			while(!cargadoCorrecto)
			if (opn.ShowDialog().GetValueOrDefault())
			{

				try{
				romGBA = new RomGba(opn.FileName);
				
				try{
					rom = new RomData(romGBA);
					InicializaCampos();
					entrenadores=EntrenadorPreview.GetEntrenadoresPreview(rom.Entrenadores,rom.ClasesEntrenadores);
			
					for (int i = 0; i < entrenadores.Count; i++)
					{
						entrenadores[i].MouseLeftButtonUp +=(s,e)=> PonEntrenador(s as EntrenadorPreview);
						ugEntrenadores.Children.Add(entrenadores[i]);
						cmbEntrenadores.Items.Add(entrenadores[i].Clone());

					}
					stkObjetosEntrenador.Children.Clear();
					ugEntrenadores.Children.Sort();
					cmbEntrenadores.SelectedIndex=0;
					Title = "Universal Shiny Trainer:"+ rom.Rom.Nombre;
					if (System.Diagnostics.Debugger.IsAttached&&!Shinyzer.EstaActivado(rom.Rom,rom.Edicion,rom.Compilacion))
					{
						if (MessageBox.Show("No esta instalada la rutina Shinyzer de HackMew, quieres instalarla?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
						{ Shinyzer.Activar(rom.Rom,rom.Edicion,rom.Compilacion); Guardar(); }
					}
					PonTexto();
					cargadoCorrecto=true;
					
				}catch{
				
					MessageBox.Show("Error inesperado mientras cargaba la rom");
					cargadoCorrecto=true;
				}
				}catch{
				cargadoCorrecto=	MessageBox.Show("No se ha podido cargar la rom, puede que tengas abierto otro programa que lo use, cierralo y vuelve a probar","Atención error al cargar la rom",MessageBoxButton.YesNo,MessageBoxImage.Error)==MessageBoxResult.No;
				
				}

			}else cargadoCorrecto=true;
		}
		private void Guardar()
		{
			rom.Rom.Save();
		}

		private void InicializaCampos()
		{
			ugEntrenadores.Children.Clear();
			tabBatallas.SelectionChanged-=TabBatallas_SelectionChanged;
			tabBatallas.Items.Clear();
			tabBatallas.SelectionChanged+=TabBatallas_SelectionChanged;
			cmbEntrenadores.SelectionChanged-=cmbEntrenadores_SelectionChanged;
			cmbEntrenadores.Items.Clear();
			cmbEntrenadores.SelectionChanged+=cmbEntrenadores_SelectionChanged;
			txtScript.Text = "";
		}



		public void PonEntrenador(EntrenadorPreview entrenadorPreview)
		{
			
			PonEntrenador(entrenadorPreview.Batallas);
		}
		public void PonEntrenador(IList<KeyValuePair<int,Entrenador>> entrenadorYBatallas)
		{
			System.Windows.Controls.Image img;
			PokemonEntrenador pokemonEntrenador;
			UniformGrid ugEquipoEntrenador;
			KeyValuePair<int,Entrenador> entrenador;
			TabItem tbAct;
			this.entrenadorYSusbatallas=entrenadorYBatallas;
			
			txtNombreEntrenador.Foreground=entrenadorYBatallas[0].Value.EsUnaEntrenadora?System.Windows.Media.Brushes.HotPink:System.Windows.Media.Brushes.Blue;

			tabBatallas.SelectionChanged-=TabBatallas_SelectionChanged;
			tabBatallas.Items.Clear();
			for(int j=0;j<entrenadorYSusbatallas.Count;j++){
				ugEquipoEntrenador=new UniformGrid();
				ugEquipoEntrenador.Columns=3;
				
				entrenador=entrenadorYSusbatallas[j];
				for (int i = 0; i < entrenador.Value.EquipoPokemon.Equipo.Length; i++)
					if (entrenador.Value.EquipoPokemon.Equipo[i] != null)
				{
					pokemonEntrenador = new PokemonEntrenador(rom, entrenador.Value.EquipoPokemon.Equipo[i],entrenador.Value);
					pokemonEntrenador.Tag=entrenador;
					pokemonEntrenador.ShinyChanged += (s, e) =>{
						KeyValuePair<int,Entrenador> entrenadorAux=(KeyValuePair<int,Entrenador>)((PokemonEntrenador)s).Tag ;
						GenerarScript(entrenadorAux.Key,entrenadorAux.Value);
					};
					ugEquipoEntrenador.Children.Add(pokemonEntrenador);
				}
				if(rom.Edicion.AbreviacionRom!=AbreviacionCanon.AXP&&rom.Edicion.AbreviacionRom!=AbreviacionCanon.AXV){
					stkObjetosEntrenador.Children.Clear();
					if(entrenador.Value.Item1>0){
						img=new System.Windows.Controls.Image();
						img.SetImage(rom.Objetos[entrenador.Value.Item1].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}
					if(entrenador.Value.Item2>0){
						img=new System.Windows.Controls.Image();
						img.SetImage(rom.Objetos[entrenador.Value.Item2].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}
					if(entrenador.Value.Item3>0){
						img=new System.Windows.Controls.Image();
						img.SetImage(rom.Objetos[entrenador.Value.Item3].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}
					if(entrenador.Value.Item4>0){
						img=new System.Windows.Controls.Image();
						img.SetImage(rom.Objetos[entrenador.Value.Item4].Sprite);
						stkObjetosEntrenador.Children.Add(img);
					}}
				tbAct=new TabItem();
				if(entrenadorYSusbatallas.Count>1)
					tbAct.Header=j+"";
				
				tbAct.Content=ugEquipoEntrenador;
				tabBatallas.Items.Add(tbAct);
			}
			tabBatallas.SelectedIndex=0;
			tabBatallas.SelectionChanged+=TabBatallas_SelectionChanged;
			PonBatallaEntrenador(entrenadorYSusbatallas[0].Key,entrenadorYSusbatallas[0].Value);
			
			
		}
		void PonBatallaEntrenador(int index,Entrenador entrenador)
		{
			
			
			if (entrenador.SpriteIndex < rom.ClasesEntrenadores.Count)
				imgEntrenador.SetImage(rom.ClasesEntrenadores[entrenador.SpriteIndex].Sprite);
			else imgEntrenador.SetImage(new Bitmap(16, 16));
			txtNombreEntrenador.Text = (entrenador.Nombre!=""?entrenador.Nombre:"Sin nombre")+" - "+((Hex)(index+1)).ToString();
			txtInteligencia.Text = "AI:" + entrenador.Inteligencia;
			txtMoneyClass.Text = rom.ClasesEntrenadores[entrenador.TrainerClass].Nombre;
			txtMusica.Text = "Musica:" + entrenador.MusicaBatalla;
			GenerarScript(index,entrenador);
		}

		string PonEspacios(string str)
		{
			StringBuilder strConEspacios=new StringBuilder();
			for(int i=0;i<str.Length;i+=2)
			{
				strConEspacios.Append(str[i]);
				strConEspacios.Append(str[i+1]);
				strConEspacios.Append(" ");
			}
			strConEspacios.Remove(strConEspacios.Length-1,1);
			return strConEspacios.ToString();
		}
		private void GenerarScript(int index,Entrenador entrenador)
		{
			const bool ACABAENEND=true;
			PokemonEntrenador[] pokemonEquipo = ((UniformGrid)((TabItem)tabBatallas.Items[tabBatallas.SelectedIndex<0?0:tabBatallas.SelectedIndex]).Content).Children.OfType<PokemonEntrenador>().ToArray();
			bool[] isShiny = new bool[pokemonEquipo.Length];
			int offsetScript;
			byte[] bytesScript;
			for (int i = 0; i < isShiny.Length; i++)
				isShiny[i] = pokemonEquipo[i].IsShiny;
			
			scriptActual=Shinyzer.SimpleScriptBattleShinyTrainer(rom.Rom,index,entrenador, isShiny);
			bytesScript=scriptActual.GetDeclaracion(rom.Rom,ACABAENEND);
			offsetScript=rom.Rom.Data.SearchArray(bytesScript);
			if(offsetScript>0){
				txtOffsetScript.Text=(Hex)offsetScript;
				btnInsertOrRemoveScript.Content="Quitar";
			}else{
				txtOffsetScript.Text="";
				btnInsertOrRemoveScript.Content="Insertar";
			}
			txtBinScript.Text=PonEspacios((string)(Hex)bytesScript);
			txtScript.Text = scriptActual.GetDeclaracionXSE(ACABAENEND,"Entrenador"+entrenador.Nombre);
		}

		

		private void cmbEntrenadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbEntrenadores.SelectedItem != null)
				PonEntrenador(cmbEntrenadores.SelectedItem as EntrenadorPreview);
		}
		void BtnInsertOrRemoveScript_Click(object sender, RoutedEventArgs e)
		{
			const bool ACABAENEND=true;
			byte[] bytesScript=scriptActual.GetDeclaracion(rom.Rom,ACABAENEND);
			int offsetScript=rom.Rom.Data.SearchArray(bytesScript);
			if(offsetScript>0)
			{
				rom.Rom.Data.Remove(offsetScript,bytesScript.Length);
				btnInsertOrRemoveScript.Content="Insertar";
				txtOffsetScript.Text="";
			}
			else
			{
				
				btnInsertOrRemoveScript.Content="Quitar";
				offsetScript=rom.Rom.Data.SearchEmptyBytes(bytesScript.Length);
				rom.Rom.Data.SetArray(offsetScript,bytesScript);
				txtOffsetScript.Text=(Hex)offsetScript;
			}
			try{
				rom.Rom.Save();
			}catch{
				if(MessageBox.Show("No se ha podido guardar los datos,cierre cualquier otro programa que use esta rom y continua","Atención, imposible escribir en la ROM",MessageBoxButton.YesNo,MessageBoxImage.Error)==MessageBoxResult.Yes)
					try{
					rom.Rom.Save();
				}catch{
					MessageBox.Show("Mejor reinicia y repite de nuevo.","Continua igual...");
				}
			}
			
			
		}
		void TabBatallas_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PonBatallaEntrenador(entrenadorYSusbatallas[tabBatallas.SelectedIndex].Key,entrenadorYSusbatallas[tabBatallas.SelectedIndex].Value);
		}
	}
}
