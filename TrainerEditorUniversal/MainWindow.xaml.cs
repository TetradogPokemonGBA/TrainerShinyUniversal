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

namespace TrainerEditorUniversal
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RomData rom;
        public MainWindow()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = new MenuItem();
            InitializeComponent();
            item.Header = "Cambiar Rom";
            item.Click += (s, e) => PideRom();
            menu.Items.Add(item);
            ContextMenu = menu;

            PideRom();
            if (rom == null)
                this.Close();
        }

        private void PideRom()
        {
            OpenFileDialog opn = new OpenFileDialog();
            Image img=null;
            opn.Filter = "GBA|*.gba";

            if (opn.ShowDialog().GetValueOrDefault())
            {

                rom = new RomData(new RomGBA(opn.FileName));
                /*  Pokemon.Orden = Pokemon.OrdenPokemon.GameFreak;
                    rom.Pokedex.Sort(); */
                if (rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONROJOFUEGO || rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONVERDEHOJA)//de momento no es compatible con todos :C
                {
                    ugEntrenadores.Children.Clear();
                    ugEquipoEntrenador.Children.Clear();
                    for (int i = 0; i < rom.Entrenadores.Count; i++)
                    {
                        if (rom.Entrenadores[i].Pokemon.HayObjetosEquipados())
                        {

                            img = new Image();
                            img.SetImage(rom.SpritesEntrenadores[rom.Entrenadores[i]]);
                            img.Tag = rom.Entrenadores[i];
                            img.MouseLeftButtonUp += PonEntrenador;
                            ugEntrenadores.Children.Add(img);

                        }
                    }
                    if (img != null)
                        PonEntrenador(img);
                    Title = rom.RomGBA.NombreRom;
                }else { PideRom(); }

            }
        }

        private void PonEntrenador(object sender, MouseButtonEventArgs e=null)
        {
            Entrenador entrenadorSeleccionado = ((Image)sender).Tag as Entrenador;
            txtNombreEntrenador.Text = entrenadorSeleccionado.Nombre;
            imgEntrenador.SetImage(rom.SpritesEntrenadores[entrenadorSeleccionado]);
            ugEquipoEntrenador.Children.Clear();
            for (int i = 0; i < entrenadorSeleccionado.Pokemon.PokemonEquipo.Length; i++)
                if (entrenadorSeleccionado.Pokemon[i] != null)
                    ugEquipoEntrenador.Children.Add(new PokemonEntrenador(rom, entrenadorSeleccionado.Pokemon[i]));
        }
    }
}
