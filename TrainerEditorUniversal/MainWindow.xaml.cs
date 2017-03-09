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
        public MainWindow()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item;
            InitializeComponent();

            item = new MenuItem();
            item.Header = "Cambiar Rom";
            item.Click += (s, e) => PideRom();
            menu.Items.Add(item);

            itemActivarDesactivar = new MenuItem();
            itemActivarDesactivar.Header = "Activar";
            itemActivarDesactivar.Click += (s, e) => ActivarDesactivar();
            menu.Items.Add(itemActivarDesactivar);

            ContextMenu = menu;

            PideRom();
            if (rom == null)
                this.Close();
        }

        private void ActivarDesactivar()
        {
            if (Shinytzer.EstaActivado(rom.RomGBA))
                Shinytzer.Desactivar(rom.RomGBA);
            else
                Shinytzer.Activar(rom.RomGBA);
            PonTexto();

        }

        private void PonTexto()
        {
            itemActivarDesactivar.Header = Shinytzer.EstaActivado(rom.RomGBA) ? "Desactivar" : "Activar";
        }

        private void PideRom()
        {
            OpenFileDialog opn = new OpenFileDialog();
            RomGBA romGBA;
            EntrenadorPreview entrenador=null;
            opn.Filter = "GBA|*.gba";

            if (opn.ShowDialog().GetValueOrDefault())
            {

                romGBA = new RomGBA(opn.FileName);
                
                if (Edicion.EsUnaEdicionDePokemon(Edicion.GetEdicion(romGBA)))
                {
                    rom = new RomData(romGBA);
                    InicializaCampos();

                    for (int i = 0; i < rom.Entrenadores.Count; i++)
                    {
                        entrenador = new EntrenadorPreview(i, rom.Entrenadores[i], rom.EntrenadoresClases);
                        entrenador.MouseLeftButtonUp +=(s,e)=> PonEntrenador(s as EntrenadorPreview);
                        ugEntrenadores.Children.Add(entrenador);
                        cmbEntrenadores.Items.Add(rom.Entrenadores[i]);

                    }

                    if (entrenador != null)
                        PonEntrenador(entrenador);
                    Title = "Universal Shiny Trainer:"+ rom.RomGBA.NombreRom;
                    PonTexto();
                }

            }
        }

        private void InicializaCampos()
        {
            ugEntrenadores.Children.Clear();
            ugEquipoEntrenador.Children.Clear();
            cmbEntrenadores.Items.Clear();
            txtScript.Text = "";
        }



        public void PonEntrenador(EntrenadorPreview entrenadorPreview)
        {
            PonEntrenador(entrenadorPreview.Entrenador);
        }
        public void PonEntrenador(Entrenador entrenador)
        {
            PokemonEntrenador pokemonEntrenador;
            txtNombreEntrenador.Text = entrenador.Nombre;
            if (entrenador.SpriteIndex < rom.EntrenadoresClases.Total)
                imgEntrenador.SetImage(rom.EntrenadoresClases.Sprites[entrenador.SpriteIndex]);
            else imgEntrenador.SetImage(new Bitmap(16, 16));

            ugEquipoEntrenador.Children.Clear();
            for (int i = 0; i < entrenador.Pokemon.PokemonEquipo.Length; i++)
                if (entrenador.Pokemon[i] != null)
                {
                    pokemonEntrenador = new PokemonEntrenador(rom, entrenador.Pokemon[i],entrenador);
                    pokemonEntrenador.ShinyChanged += (s, e) => GenerarScript(pokemonEntrenador.Entrenador);
                    ugEquipoEntrenador.Children.Add(pokemonEntrenador);
                }

            txtInteligencia.Text = "AI:" + entrenador.Inteligencia;
            txtMoneyClass.Text = rom.EntrenadoresClases.Nombres[entrenador.TrainerClass];
            txtMusica.Text = "Musica:" + entrenador.MusicaBatalla;
            GenerarScript(entrenador);

        }

        private void GenerarScript(Entrenador entrenador)
        {
            PokemonEntrenador[] pokemonEquipo = ugEquipoEntrenador.Children.OfType<PokemonEntrenador>().ToArray();
            bool[] isShiny = new bool[pokemonEquipo.Length];
            for (int i = 0; i < isShiny.Length; i++)
                isShiny[i] = pokemonEquipo[i].IsShiny;
            txtScript.Text = Shinytzer.ScriptLineaPokemonShinyEntrenador(entrenador, isShiny);
        }

     

        private void cmbEntrenadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEntrenadores.SelectedItem != null)
                PonEntrenador(cmbEntrenadores.SelectedItem as Entrenador);
        }
    }
}
