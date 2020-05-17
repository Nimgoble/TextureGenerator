using System.Collections.Generic;
using Caliburn.Micro;
using HelixToolkit.Wpf.SharpDX;

namespace TextureGenerator.ViewModels
{
	public class TriangleViewModel : Screen
	{
		private Geometry3D.Triangle model;
		private int index;
		private List<SharpDX.Vector2> textureCoordinates;
		public TriangleViewModel(Geometry3D.Triangle model, int index, List<SharpDX.Vector2> textureCoordinates)
		{
			this.model = model;
			this.index = index;
			this.textureCoordinates = textureCoordinates;
		}
		public Geometry3D.Triangle Model { get { return this.model; } }
		public int Index { get { return this.index; } }
		public string Name { get { return $"Triangle {index}"; } }
		public List<SharpDX.Vector2> TextureCoordinates { get { return textureCoordinates; } }
	}
}
