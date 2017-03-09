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
            RomGBA romGBA;
            System.Windows.Controls.Image img = null;
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

                        img = new System.Windows.Controls.Image();
                        if (rom.Entrenadores[i].SpriteIndex < rom.EntrenadoresClases.Total)
                            img.SetImage(rom.EntrenadoresClases.Sprites[rom.Entrenadores[i].SpriteIndex]);
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
        }

        private void InicializaCampos()
        {
            ugEntrenadores.Children.Clear();
            ugEquipoEntrenador.Children.Clear();
            cmbEntrenadores.Items.Clear();

            InicializaEntrenadorSeleccionado();
        }

        private void InicializaEntrenadorSeleccionado()
        {
            txtItem1.Text = "";
            txtItem2.Text = "";
            txtItem3.Text = "";
            txtItem4.Text = "";
            imgItem1.SetImage(new Bitmap(1, 1));
            imgItem2.SetImage(new Bitmap(1, 1));
            imgItem3.SetImage(new Bitmap(1, 1));
            imgItem4.SetImage(new Bitmap(1, 1));
        }

        private void PonEntrenador(object sender, MouseButtonEventArgs e = null)
        {
            PonEntrenador(((System.Windows.Controls.Image)sender).Tag as Entrenador);

        }
        public void PonEntrenador(Entrenador entrenador)
        {

            txtNombreEntrenador.Text = entrenador.Nombre;
            if (entrenador.SpriteIndex < rom.EntrenadoresClases.Total)
                imgEntrenador.SetImage(rom.EntrenadoresClases.Sprites[entrenador.SpriteIndex]);
            else imgEntrenador.SetImage(new Bitmap(16, 16));
            InicializaEntrenadorSeleccionado();
            if (!(rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONRUBI || rom.Edicion.AbreviacionRom == Edicion.ABREVIACIONZAFIRO))//NO TIENEN IMAGEN
            {
                if (entrenador.Item1 > 0)
                    imgItem1.SetImage(rom.Objetos[entrenador.Item1].ImagenObjeto);
            
                if (entrenador.Item2 > 0)
                    imgItem2.SetImage(rom.Objetos[entrenador.Item2].ImagenObjeto);
   
                if (entrenador.Item3 > 0)
                    imgItem3.SetImage(rom.Objetos[entrenador.Item3].ImagenObjeto);
           
                if (entrenador.Item4 > 0)
                    imgItem4.SetImage(rom.Objetos[entrenador.Item4].ImagenObjeto);
               
            }
            else
            {
                if (entrenador.Item1 > 0)
                    txtItem1.Text = rom.Objetos[entrenador.Item1].Nombre;
        
                if (entrenador.Item2 > 0)
                    txtItem2.Text = rom.Objetos[entrenador.Item2].Nombre;
          
                if (entrenador.Item3 > 0)
                    txtItem3.Text = rom.Objetos[entrenador.Item3].Nombre;
         
                if (entrenador.Item4 > 0)
                    txtItem4.Text = rom.Objetos[entrenador.Item4].Nombre;
    
            }


            ugEquipoEntrenador.Children.Clear();
            for (int i = 0; i < entrenador.Pokemon.PokemonEquipo.Length; i++)
                if (entrenador.Pokemon[i] != null)
                    ugEquipoEntrenador.Children.Add(new PokemonEntrenador(rom, entrenador.Pokemon[i]));
            txtInteligencia.Text = "AI:" + entrenador.Inteligencia;
            txtMoneyClass.Text = rom.EntrenadoresClases.Nombres[entrenador.TrainerClass];
            if (rom.EntrenadoresClases.RatesMoney.Count > 0)
                txtMoneyClass.Text +="-"+ rom.EntrenadoresClases.RatesMoney[entrenador.TrainerClass];
            txtMusica.Text = "Musica:" + entrenador.MusicaBatalla;

        }

        private void cmbEntrenadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEntrenadores.SelectedItem != null)
                PonEntrenador(cmbEntrenadores.SelectedItem as Entrenador);
        }
    }
}
