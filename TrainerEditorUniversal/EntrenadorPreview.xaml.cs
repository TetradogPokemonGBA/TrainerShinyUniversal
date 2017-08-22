using Gabriel.Cat.Extension;
using PokemonGBAFrameWork;
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

namespace TrainerEditorUniversal
{
	/// <summary>
	/// Lógica de interacción para EntrenadorPreview.xaml
	/// </summary>
	public partial class EntrenadorPreview : UserControl
	{
		List<KeyValuePair<int,Entrenador>> batallas;
		IList<ClaseEntrenador> entrenadores;
		public EntrenadorPreview(IList<KeyValuePair<int,Entrenador>> entrenadorYSusBatallas,IList<ClaseEntrenador> entrenadores)
		{
			batallas=new List<KeyValuePair<int,Entrenador>>();
			batallas.AddRange(entrenadorYSusBatallas);
			this.entrenadores=entrenadores;
			InitializeComponent();
			try{
				imgEntrenador.SetImage(entrenadores[entrenadorYSusBatallas[0].Value.SpriteIndex].Sprite);
			}catch{}
			txtIdNombre.Text = ToString();
		}

		public List<KeyValuePair<int,Entrenador>> Batallas
		{
			get
			{
				return batallas;
			}

		}
		public EntrenadorPreview Clone()
		{
			return new EntrenadorPreview(batallas,entrenadores);
		}
		public override string ToString()
		{
			return batallas[0].Value.Nombre!=""?batallas[0].Value.Nombre:"Sin Nombres";
		}
		public static EntrenadorPreview[] GetEntrenadoresPreview(IList<Entrenador> entrenadores,IList<ClaseEntrenador> clasesEntrenador)
		{
			Gabriel.Cat.LlistaOrdenada<string,List<KeyValuePair<int,Entrenador>>> entrenadoresSeparadosPorBatallas=new Gabriel.Cat.LlistaOrdenada<string, List<KeyValuePair<int,Entrenador>>>();
			EntrenadorPreview[] entrenadoresPreview;
			string nombre;
			for(int i=0;i<entrenadores.Count;i++)
			{
				nombre=entrenadores[i].Nombre;
				if(!entrenadoresSeparadosPorBatallas.ContainsKey(nombre))
					entrenadoresSeparadosPorBatallas.Add(nombre,new List<KeyValuePair<int,Entrenador>>());
				entrenadoresSeparadosPorBatallas[nombre].Add(new KeyValuePair<int, Entrenador>(i,entrenadores[i]));
			}
			entrenadoresPreview=new EntrenadorPreview[entrenadoresSeparadosPorBatallas.Count];
			for(int i=0;i<entrenadoresSeparadosPorBatallas.Count;i++){
				nombre=entrenadoresSeparadosPorBatallas.GetKey(i);
				entrenadoresPreview[i]=new EntrenadorPreview(entrenadoresSeparadosPorBatallas[nombre],clasesEntrenador);
			}
			return entrenadoresPreview;
		}
	}
}
