using ParangEngine;
using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Renderer
{
    static public class Cube
    {
        static public readonly List<Vector3> V = new List<Vector3>()
            {
                new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f)
            };

        static public readonly List<Vector2> UV = new List<Vector2>()
            {
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)
            };

        static public readonly List<int> VI = new List<int>()
            {
                // 0, 1, 2, 0, 2, 3, // Right
	            4, 6, 5, 4, 7, 6, // Front
	            /* 8, 9, 10, 8, 10, 11, // Back
	            12, 14, 13, 12, 15, 14, // Left*/
	            16, 18, 17, 16, 19, 18, // Top
	            /* 20, 21, 22, 20, 22, 23  // Bottom */
            };
    }

    public partial class Renderer : Form
    {
        private Engine engine;
        private Scene scene;

        public Renderer()
        {
            InitializeComponent();

            Load += Renderer_Load;
            HandleDestroyed += Renderer_HandleDestroyed;

            // Resource Load
            Resource.AddMesh("cube.mesh", new Mesh(Cube.V, Cube.UV, Cube.VI));
            Resource.AddMesh("airboat.obj");


            Resource.AddTexture("wall1_color.png");
            Resource.AddTexture("wall1_n.png");
            Resource.AddTexture("wall1_shga.png");
            Resource.AddTexture("rock_diffuse.png");
            Resource.AddTexture("rock_normal.png");

            scene = new Scene();

            // camera
            var cameraGo = new GameObject();
            cameraGo.Transform.Rotation = new Vector3(45f, 0f, 0f);
            cameraGo.Transform.Position = new Vector3(0f, 10f, -10f);

            var camera = new Camera(320, 240, 60f);
            cameraGo.AddComponent(camera);
            scene.Add(cameraGo);

            {
                var meshGo = new GameObject();
                meshGo.Transform.Rotation = new Vector3(0f, 0f, 0f);
                // meshGo.Transform.Position = new Vector3(0f, 2f, 0f);
                // meshGo.Transform.Scale = new Vector3(0.5f, 0.5f, 0.5f);
                meshGo.AddComponent(new MeshRenderer(
                    Resource.GetMesh("airboat.obj"), new Material()));
                // meshGo.AddComponent(new RoughnessController());
                // meshGo.AddComponent(new MatalicController());
                // meshGo.AddComponent(new Rotater());
                scene.Add(meshGo);
            }

            {
                // material
                var material = new Material();
                material.AddTexture(Material.Type.Albedo, Resource.GetTexture("rock_diffuse.png"));
                material.AddTexture(Material.Type.Normal, Resource.GetTexture("rock_normal.png"));
                material.Roughness = 0f;
                material.Metalic = 1f;

                {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new RoughnessController());
                    // meshGo.AddComponent(new MatalicController());
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                }

                /* {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(-2f, 0f, 0f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                }

                {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(-2f, -2f, 2f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                }

                {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(0f, -2f, 2f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                }

                {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(-2f, -2f, -2f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                } */
            }

            /* {
                // material
                var material = new Material();
                material.AddTexture(Material.Type.Albedo, Resource.GetTexture("rock_diffuse.png"));
                material.AddTexture(Material.Type.Normal, Resource.GetTexture("rock_normal.png"));
                material.Roughness = 0f;
                material.Metalic = 0f;
                
                {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(2f, 0f, 0f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new RoughnessController());
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                }

                {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(2f, -2f, -2f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                }

                {
                    // mesh
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(0f, -2f, -2f);
                    meshGo.AddComponent(new MeshRenderer(
                        Resource.GetMesh("cube.mesh"), material));
                    // meshGo.AddComponent(new Rotater());
                    scene.Add(meshGo);
                }
            } */


            var lightGo = new GameObject();
            var dirLight = new DirectionalLight();
            dirLight.Color = ParangEngine.Types.Color.White;
            dirLight.Intensity = 0.8f;
            lightGo.AddComponent(dirLight);
            lightGo.AddComponent(new Rotater());
            lightGo.Transform.Rotation = new Vector3(45f, 0f, 0f);
            scene.Add(lightGo);

            {
                var go = new GameObject();
                var l = new PointLight();
                l.Color = new ParangEngine.Types.Color("red");
                l.Intensity = 4f;
                l.Radius = 5f;
                go.Transform.Position = new Vector3(0f, 0f, -3f);
                go.AddComponent(l);
                scene.Add(go);
            }

            
            {
                var go = new GameObject();
                var l = new PointLight();
                l.Color = new ParangEngine.Types.Color("green");
                l.Intensity = 4f;
                l.Radius = 5f;
                go.Transform.Position = new Vector3(0f, 2f, 0f);
                go.AddComponent(l);
                scene.Add(go);
            }

            {
                var go = new GameObject();
                var l = new PointLight();
                l.Color = new ParangEngine.Types.Color("blue");
                l.Intensity = 4f;
                l.Radius = 3f;
                go.Transform.Position = new Vector3(1f, 1f, -2f);
                go.AddComponent(l);
                scene.Add(go);
            }

            engine = new Engine(CreateGraphics(), new Size(640, 480));
            engine.SetScene(scene);
        }

        private void Renderer_Load(object sender, EventArgs e)
        {
            engine.Start();
        }

        private void Renderer_HandleDestroyed(object sender, EventArgs e)
        {
            engine.Stop();
        }
    }
}
