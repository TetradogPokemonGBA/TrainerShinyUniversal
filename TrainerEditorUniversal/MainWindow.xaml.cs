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
            InitializeComponent();
            PideRom();
            if (rom == null)
                this.Close();
        }

        private void PideRom()
        {
            OpenFileDialog opn = new OpenFileDialog();
            Image img;
            opn.Filter = "GBA|*.gba";

            if (opn.ShowDialog().GetValueOrDefault())
            {

                rom = new RomData(new RomGBA(opn.FileName));
                Pokemon.Orden = Pokemon.OrdenPokemon.Nacional;
                rom.Pokedex.Sort();
                for (int i = 0; i < rom.Entrenadores.Count; i++)
                {
                    if (rom.Entrenadores[i].Pokemon.HayObjetosEquipados())
                    {
                        try
                        {
                            img = new Image();
                            img.SetImage(rom.SpritesEntrenadores[rom.Entrenadores[i]]);
                            img.Tag = rom.Entrenadores[i];
                            img.MouseLeftButtonUp += PonEntrenador;
                            ugEntrenadores.Children.Add(img);
                        }

                        catch { }
                    }
                }
                Title = rom.RomGBA.NombreRom;

            }
        }

        private void PonEntrenador(object sender, MouseButtonEventArgs e)
        {
            Entrenador entrenadorSeleccionado = ((Image)sender).Tag as Entrenador;
            Title = entrenadorSeleccionado.Nombre;
            ugEquipoEntrenador.Children.Clear();
            for (int i = 0; i < entrenadorSeleccionado.Pokemon.PokemonEquipo.Length; i++)
                if (entrenadorSeleccionado.Pokemon[i] != null)
                    ugEquipoEntrenador.Children.Add(new PokemonEntrenador(rom, entrenadorSeleccionado.Pokemon[i]));
        }
    }
}
