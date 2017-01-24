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
               // Pokemon.Orden = Pokemon.OrdenPokemon.Local;
              //  rom.Pokedex.RemoveAt(rom.Pokedex.Count - 1);//huevo
              //  rom.Pokedex.Pop();//quito a missigno
             //   rom.Pokedex.Sort();
         
                    ugEntrenadores.Children.Clear();
                    ugEquipoEntrenador.Children.Clear();
                cmbEntrenadores.Items.Clear();
                    for (int i = 0; i < rom.Entrenadores.Count; i++)
                    {
                         
                            img = new System.Windows.Controls.Image();
                            if (rom.Entrenadores[i].SpriteIndex < rom.SpritesEntrenadores.Total)
                                img.SetImage(rom.SpritesEntrenadores[rom.Entrenadores[i]]);
                            else img.SetImage(new Bitmap(16, 16));
                            img.Tag = rom.Entrenadores[i];
                            img.MouseLeftButtonUp += PonEntrenador;
                            ugEntrenadores.Children.Add(img);
                            cmbEntrenadores.Items.Add(rom.Entrenadores[i]);
                   
                    }
                    if (img != null)
                        PonEntrenador(img);
                    Title = rom.RomGBA.NombreRom;
        

            }
        }

        private void PonEntrenador(object sender, MouseButtonEventArgs e = null)
        {
           PonEntrenador(((System.Windows.Controls.Image)sender).Tag as Entrenador);
            
        }
        public void PonEntrenador(Entrenador entrenador)
        {
            byte[] bytes;
            try
            {
                txtNombreEntrenador.Text = entrenador.Nombre;
            if (entrenador.SpriteIndex < rom.SpritesEntrenadores.Total)
                imgEntrenador.SetImage(rom.SpritesEntrenadores[entrenador]);
            else imgEntrenador.SetImage(new Bitmap(16, 16));
            if (!(rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONRUBI || rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONZAFIRO)) { 
                imgItem1.SetImage(rom.Objetos[entrenador.Item1].ImagenObjeto);
                imgItem2.SetImage(rom.Objetos[entrenador.Item2].ImagenObjeto);
                imgItem3.SetImage(rom.Objetos[entrenador.Item3].ImagenObjeto);
                imgItem4.SetImage(rom.Objetos[entrenador.Item4].ImagenObjeto);
            }
           
          //  if (rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONROJOFUEGO || rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONVERDEHOJA)
            {
                ugEquipoEntrenador.Children.Clear();
                for (int i = 0; i < entrenador.Pokemon.PokemonEquipo.Length; i++)
                    if (entrenador.Pokemon[i] != null)
                        ugEquipoEntrenador.Children.Add(new PokemonEntrenador(rom, entrenador.Pokemon[i]));
            }
           
                txtDatos.Text = "-Equipo- numero de pokemon " + entrenador.Pokemon.NumeroPokemon;
                bytes = rom.RomGBA.Datos.SubArray((int)entrenador.Pokemon.OffsetToDataPokemon, (entrenador.Pokemon.HayAtaquesCustom() ? 16 : 8) * entrenador.Pokemon.NumeroPokemon);
                for (int i = 0; i < entrenador.Pokemon.NumeroPokemon; i++)
                    txtDatos.Text += "\n" + (i + 1) + "-" + ((Hex)bytes.SubArray(i * (entrenador.Pokemon.HayAtaquesCustom() ? 16 : 8), entrenador.Pokemon.HayAtaquesCustom() ? 16 : 8)).ToString();
            }
            catch { }
        }

        private void cmbEntrenadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbEntrenadores.SelectedItem!=null)
               PonEntrenador(cmbEntrenadores.SelectedItem as Entrenador);
        } 
    }
}
