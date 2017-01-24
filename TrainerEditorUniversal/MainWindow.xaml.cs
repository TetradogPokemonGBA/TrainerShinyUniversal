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
            System.Windows.Controls.Image img =null;
            opn.Filter = "GBA|*.gba";

            if (opn.ShowDialog().GetValueOrDefault())
            {

                rom = new RomData(new RomGBA(opn.FileName));
  
         
                    ugEntrenadores.Children.Clear();
                    ugEquipoEntrenador.Children.Clear();
                    for (int i = 0; i < rom.Entrenadores.Count; i++)
                    {
                         
                            img = new System.Windows.Controls.Image();
                            if (rom.Entrenadores[i].SpriteIndex < rom.SpritesEntrenadores.Total)
                                img.SetImage(rom.SpritesEntrenadores[rom.Entrenadores[i]]);
                            else img.SetImage(new Bitmap(16, 16));
                            img.Tag = rom.Entrenadores[i];
                            img.MouseLeftButtonUp += PonEntrenador;
                            ugEntrenadores.Children.Add(img);
                   
                    }
                    if (img != null)
                        PonEntrenador(img);
                    Title = rom.RomGBA.NombreRom;
        

            }
        }

        private void PonEntrenador(object sender, MouseButtonEventArgs e = null)
        {
            Entrenador entrenadorSeleccionado = ((System.Windows.Controls.Image)sender).Tag as Entrenador;
            byte[] bytes;
            txtNombreEntrenador.Text = entrenadorSeleccionado.Nombre;
            if (entrenadorSeleccionado.SpriteIndex < rom.SpritesEntrenadores.Total)
                imgEntrenador.SetImage(rom.SpritesEntrenadores[entrenadorSeleccionado]);
            else imgEntrenador.SetImage(new Bitmap(16, 16));

            if (rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONROJOFUEGO || rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONVERDEHOJA)
            {
                imgItem1.SetImage(rom.Objetos[entrenadorSeleccionado.Item1].ImagenObjeto);
                imgItem2.SetImage(rom.Objetos[entrenadorSeleccionado.Item2].ImagenObjeto);
                imgItem3.SetImage(rom.Objetos[entrenadorSeleccionado.Item3].ImagenObjeto);
                imgItem4.SetImage(rom.Objetos[entrenadorSeleccionado.Item4].ImagenObjeto);
                
                ugEquipoEntrenador.Children.Clear();
                for (int i = 0; i < entrenadorSeleccionado.Pokemon.PokemonEquipo.Length; i++)
                    if (entrenadorSeleccionado.Pokemon[i] != null)
                        ugEquipoEntrenador.Children.Add(new PokemonEntrenador(rom, entrenadorSeleccionado.Pokemon[i]));
            }
            try
            {
                txtDatos.Text = "-Equipo- numero de pokemon "+ entrenadorSeleccionado.Pokemon.NumeroPokemon;
                bytes=rom.RomGBA.Datos.SubArray((int)entrenadorSeleccionado.Pokemon.OffsetToDataPokemon, (entrenadorSeleccionado.Pokemon.HayAtaquesCustom() ? 16 : 8) * entrenadorSeleccionado.Pokemon.NumeroPokemon);
                for(int i=0;i<entrenadorSeleccionado.Pokemon.NumeroPokemon;i++)
                txtDatos.Text +="\n"+(i+1)+"-"+((Hex) bytes.SubArray(i* (entrenadorSeleccionado.Pokemon.HayAtaquesCustom() ? 16 : 8), entrenadorSeleccionado.Pokemon.HayAtaquesCustom() ? 16 : 8)).ToString();
            }
            catch { }
        }
    }
}
