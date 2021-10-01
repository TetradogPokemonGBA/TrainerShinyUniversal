using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using PokemonGBAFramework.Core;
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
    public partial class EntrenadorPreview : UserControl, IComparable
	{
		List<KeyValuePair<int, Entrenador>> batallas;
		IList<ClaseEntrenador> entrenadores;
		public EntrenadorPreview(IList<KeyValuePair<int, Entrenador>> entrenadorYSusBatallas, IList<ClaseEntrenador> entrenadores)
		{
			batallas = new List<KeyValuePair<int, Entrenador>>();
			batallas.AddRange(entrenadorYSusBatallas);
			this.entrenadores = entrenadores;
			InitializeComponent();
			try
			{
				imgEntrenador.SetImage(entrenadores[entrenadorYSusBatallas[0].Value.SpriteIndex].Sprite);
			}
			catch { }
			txtIdNombre.Text = ToString();
		}

		public List<KeyValuePair<int, Entrenador>> Batallas
		{
			get
			{
				return batallas;
			}

		}
		public EntrenadorPreview Clone()
		{
			return new EntrenadorPreview(batallas, entrenadores);
		}

		#region IComparable implementation


		public int CompareTo(object obj)
		{
			EntrenadorPreview other = obj as EntrenadorPreview;
			int compareTo;
			if (other != null)
				compareTo = Batallas[0].Value.SpriteIndex.CompareTo(other.Batallas[0].Value.SpriteIndex);
			else compareTo = -1;
			return compareTo;
		}


		#endregion

		public override string ToString()
		{
			return batallas[0].Value.Nombre != "" ? batallas[0].Value.Nombre : "Sin Nombres";
		}
		public static List<EntrenadorPreview> GetEntrenadoresPreview(IList<Entrenador> entrenadores, IList<ClaseEntrenador> clasesEntrenador)
		{
			LlistaOrdenada<string, List<KeyValuePair<int, Entrenador>>> entrenadoresSeparadosPorBatallas = new LlistaOrdenada<string, List<KeyValuePair<int, Entrenador>>>();
			LlistaOrdenada<int, List<KeyValuePair<int, Entrenador>>> entrenadoresSeparadosPorIndexSprite = new LlistaOrdenada<int, List<KeyValuePair<int, Entrenador>>>();
			List<EntrenadorPreview> entrenadoresPreview = new List<EntrenadorPreview>();
			List<KeyValuePair<int, Entrenador>> auxList;
			string nombre;
			for (int i = 0; i < entrenadores.Count; i++)
			{
				nombre = entrenadores[i].Nombre;
				if (!entrenadoresSeparadosPorBatallas.ContainsKey(nombre))
					entrenadoresSeparadosPorBatallas.Add(nombre, new List<KeyValuePair<int, Entrenador>>());
				entrenadoresSeparadosPorBatallas[nombre].Add(new KeyValuePair<int, Entrenador>(i, entrenadores[i]));
			}

			for (int i = 0; i < entrenadoresSeparadosPorBatallas.Count; i++)
			{
				entrenadoresSeparadosPorIndexSprite.Clear();
				nombre = entrenadoresSeparadosPorBatallas.GetKey(i);
				auxList = entrenadoresSeparadosPorBatallas[nombre];
				for (int j = 0; j < auxList.Count; j++)
				{
					if (!entrenadoresSeparadosPorIndexSprite.ContainsKey(auxList[j].Value.SpriteIndex))
						entrenadoresSeparadosPorIndexSprite.Add(auxList[j].Value.SpriteIndex, new List<KeyValuePair<int, Entrenador>>());
					entrenadoresSeparadosPorIndexSprite.GetValue(auxList[j].Value.SpriteIndex).Add(auxList[j]);
				}
				for (int k = 0; k < entrenadoresSeparadosPorIndexSprite.Count; k++)
					entrenadoresPreview.Add(new EntrenadorPreview(entrenadoresSeparadosPorIndexSprite.GetValueAt(k), clasesEntrenador));
			}
			return entrenadoresPreview;
		}
	}
}

