using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Vector3 = SharpDX.Vector3;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Media3D = System.Windows.Media.Media3D;

using Caliburn.Micro;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Assimp;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System.Net.Http.Headers;

namespace TextureGenerator.ViewModels
{
	public class TriangleSelectionEventArgs : EventArgs
	{
		public TriangleViewModel ViewModel { get; set; }
	}
	public class ModelViewerViewModel : Screen, IDisposable
	{
		private readonly IEffectsManager effectsManager;
		private readonly IWindowManager windowManager;
		private Camera camera = new PerspectiveCamera();
		private SceneNode sceneNode = null;
		private BoneSkinnedMeshGeometry3D bsmg = null;

		public ModelViewerViewModel(IWindowManager windowManager, IEffectsManager effectsManager)
		{
			this.windowManager = windowManager;
			this.effectsManager = effectsManager;
			this.camera = new PerspectiveCamera
			{
				Position = new Point3D(3, 3, 5),
				LookDirection = new Vector3D(-3, -3, -5),
				UpDirection = new Vector3D(0, 1, 0),
				FarPlaneDistance = 5000000
			};
			this.testMaterial = PhongMaterials.Red;
			// setup lighting            
			AmbientLightColor = Colors.DimGray;
			DirectionalLightColor = Colors.White;
			// floor plane grid
			Grid = LineBuilder.GenerateGrid(new Vector3(0, 1, 0), -5, 5, -5, 5);
			GridColor = Colors.Black;
			GridTransform = new Media3D.TranslateTransform3D(0, -3, 0);
			this.Triangles = new ObservableCollection<TriangleViewModel>();
		}
		~ModelViewerViewModel()
		{
			this.Dispose();
		}
		public void Dispose()
		{
			effectsManager.Dispose();
		}

		public delegate void TriangleSelectionEventHandler(object sender, TriangleSelectionEventArgs e);
		public event TriangleSelectionEventHandler TriangleSelection;
		protected void OnTriangleSelection(TriangleViewModel viewModel)
		{
			this.TriangleSelection?.Invoke(this, new TriangleSelectionEventArgs() { ViewModel = viewModel });
		}

		#region Methods
		public void LoadModel(string fileName)
		{
			var dialogViewModel = new DialogViewModel();
			HelixToolkitScene scene = null;
			dialogViewModel.AddTask
			(
				(taskContext) =>
				{
					taskContext.UpdateMessage($"Loading model {Path.GetFileNameWithoutExtension(fileName)}");
					var loader = new Importer();
					scene = loader.Load(fileName);
					taskContext.UpdateProgress(100);
				}
			);
			this.windowManager.ShowDialog(dialogViewModel);
			GroupModel.Clear();
			if (scene != null)
			{
				this.sceneNode = scene.Root;
				GroupModel.AddNode(this.sceneNode);
				this.SetSceneMaterials();
			}
		}
		private void SetSceneMaterials()
		{
			if (this.sceneNode == null || this.TestMaterial == null)
				return;
			this.bsmg = null;
			foreach (var node in this.sceneNode.Traverse())
			{
				if (node is MaterialGeometryNode m)
				{
					m.Material = this.TestMaterial;
					if(node is BoneSkinMeshNode bsmn)
					{
						if(bsmn.Geometry is BoneSkinnedMeshGeometry3D geo3D)
						{
							this.bsmg = geo3D;
							this.Triangles.Clear();
							for(int i = 0; i < geo3D.Triangles.Count(); ++i)
							{
								var triangle = geo3D.Triangles.ElementAt(i);
								var startIndex = 3 * i;
								var textureCoordinatesIndices = new int[]
								{
									geo3D.TriangleIndices[startIndex],
									geo3D.TriangleIndices[startIndex + 1],
									geo3D.TriangleIndices[startIndex + 2]
								};
								List<SharpDX.Vector2> textureCoordinates = new List<SharpDX.Vector2>()
								{
									geo3D.TextureCoordinates[textureCoordinatesIndices[0]],
									geo3D.TextureCoordinates[textureCoordinatesIndices[1]],
									geo3D.TextureCoordinates[textureCoordinatesIndices[2]]
								};
								this.Triangles.Add(new TriangleViewModel(triangle, i, textureCoordinates));
							}
						}
					}
				}
			}
		}
		private void SetSelectionTriangle(TriangleViewModel triangle)
		{
			if(this.selectedTriangleMeshNode != null)
			{
				this.GroupModel.RemoveNode(this.selectedTriangleMeshNode);
				this.selectedTriangleMeshNode = null;
			}
			if (triangle == null)
				return;
			var b1 = new MeshBuilder();
			b1.AddTriangle(triangle.Model.P0, triangle.Model.P1, triangle.Model.P2);
			var triangleMeshGeometry = b1.ToMeshGeometry3D();
			this.selectedTriangleMeshNode = new MeshNode()
			{
				Geometry = triangleMeshGeometry,
				Material = PhongMaterials.Green
			};
			this.GroupModel.AddNode(this.selectedTriangleMeshNode);
		}
		#endregion

		#region Properties
		public IEffectsManager EffectsManager { get { return this.effectsManager; } }
		public Camera Camera { get { return this.camera; } }
		public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();
		private PhongMaterial testMaterial = null;
		public PhongMaterial TestMaterial
		{
			get { return this.testMaterial; }
			set
			{
				this.testMaterial = value;
				this.SetSceneMaterials();
				NotifyOfPropertyChange(() => TestMaterial);
			}
		}
		public LineGeometry3D Grid { get; private set; }
		public Transform3D GridTransform { get; private set; }
		public Color GridColor { get; private set; }
		public Color AmbientLightColor { get; private set; }
		public Color DirectionalLightColor { get; private set; }
		private MeshNode selectedTriangleMeshNode = null;
		public ObservableCollection<TriangleViewModel> Triangles { get; set; }
		private TriangleViewModel selectedTriangle = null;
		public TriangleViewModel SelectedTriangle
		{
			get { return this.selectedTriangle; }
			set
			{
				this.selectedTriangle = value;
				this.SetSelectionTriangle(this.selectedTriangle);
				this.OnTriangleSelection(this.selectedTriangle);
				NotifyOfPropertyChange(() => SelectedTriangle);
			}
		}
		#endregion
	}
}
