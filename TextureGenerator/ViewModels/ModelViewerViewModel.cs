using System;
using System.Collections.Generic;
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

namespace TextureGenerator.ViewModels
{
	public class ModelViewerViewModel : Screen, IDisposable
	{
		private readonly IEffectsManager effectsManager;
		private readonly IWindowManager windowManager;
		private Camera camera = new PerspectiveCamera();
		private SceneNode sceneNode = null;

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
		}
		~ModelViewerViewModel()
		{
			this.Dispose();
		}
		public void Dispose()
		{
			effectsManager.Dispose();
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
			foreach (var node in this.sceneNode.Traverse())
			{
				if (node is MaterialGeometryNode m)
				{
					m.Material = this.TestMaterial;
				}
			}
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
		#endregion
	}
}
