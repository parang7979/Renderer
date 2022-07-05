using ParangEngine;
using ParangEngine.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Game
{
    static public class Cube
    {
        static public readonly List<Vector3> V = new List<Vector3>()
            {
                new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 1.0f),
            };

        static public readonly List<Vector2> UV = new List<Vector2>()
            {
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f),
            };

        static public readonly List<int> VI = new List<int>()
            {
	            0, 2, 1, 0, 3, 2,
            };
    }

    public class Game
    {
        private Engine engine;
        private Scene scene;

        public Game(Graphics graphics, Size resolution)
        {
            engine = new Engine(graphics, resolution);
        }

        public void LoadResource()
        {
            // Resource Load
            ResourceManager.AddMesh("cube.mesh", new Mesh(Cube.V, Cube.UV, null, Cube.VI, null, null));
            ResourceManager.AddMesh("Ship01.obj");

            ResourceManager.AddTexture("wall1_color.png");
            ResourceManager.AddTexture("wall1_n.png");
            ResourceManager.AddTexture("wall1_shga.png");
            ResourceManager.AddTexture("rock_diffuse.png");
            ResourceManager.AddTexture("rock_normal.png");
        }

        public void CreateScene()
        {
            scene = new Scene();

            // camera
            var cameraGo = new GameObject();
            cameraGo.Transform.Rotation = new Vector3(45f, 0f, 0f);
            cameraGo.Transform.Position = new Vector3(0f, 12.5f, -12.5f);

            var camera = new Camera(400, 300, 60f);
            camera.ClearColor = new ParangEngine.Types.Color(System.Drawing.Color.Black);
            cameraGo.AddComponent(camera);
            scene.Add(cameraGo);
            {
                {
                    var meshs = ResourceManager.GetMesh("Ship01.obj");
                    var material = new Material();
                    material.AddTexture(Material.Type.Albedo, ResourceManager.GetTexture("wall1_color.png"));
                    material.AddTexture(Material.Type.Normal, ResourceManager.GetTexture("wall1_n.png"));
                    material.Roughness = 0.8f;
                    material.Metalic = 0f;
                    var meshGo = new GameObject();
                    meshGo.Transform.Position = new Vector3(0f, 1f, 0f);
                    meshGo.AddComponent(new MeshRenderer(meshs, material));
                    meshGo.AddComponent(new Input());
                    
                    scene.Add(meshGo);
                    {
                        var engineGo = new GameObject();
                        engineGo.Transform.Parents = meshGo.Transform;
                        engineGo.Transform.Position = new Vector3(0f, 0f, -1f);
                        engineGo.AddComponent(new JetEngine
                        {
                            Color = new ParangEngine.Types.Color(System.Drawing.Color.OrangeRed)
                        });
                        scene.Add(engineGo);
                    }
                    
                    {
                        var gunGo = new GameObject();
                        gunGo.Transform.Parents = meshGo.Transform;
                        gunGo.Transform.Position = new Vector3(0.5f, 0f, 0f);
                        gunGo.AddComponent(new Gun());
                        scene.Add(gunGo);
                    }

                    {
                        var gunGo = new GameObject();
                        gunGo.Transform.Parents = meshGo.Transform;
                        gunGo.Transform.Position = new Vector3(-0.5f, 0f, 0f);
                        gunGo.AddComponent(new Gun());
                        scene.Add(gunGo);
                    }
                }

                {
                    var meshs = ResourceManager.GetMesh("cube.mesh");
                    var material = new Material();
                    material.AddTexture(Material.Type.Albedo, ResourceManager.GetTexture("rock_diffuse.png"));
                    material.AddTexture(Material.Type.Normal, ResourceManager.GetTexture("rock_normal.png"));
                    material.Roughness = 0.5f;
                    material.Metalic = 0.5f;
                    for (int i = 0; i < 25; i++)
                    {
                        var meshGo = new GameObject();
                        meshGo.Transform.Rotation = new Vector3(0f, 0f, 0f);
                        meshGo.Transform.Position = new Vector3(((i % 5) - 2) * 4, 0f, ((i / 5) - 2) * 4);
                        // meshGo.Transform.Position = new Vector3(0, 0, 0);
                        meshGo.AddComponent(new MeshRenderer(meshs, material));
                        scene.Add(meshGo);
                    }
                }
            }

            /* {
                // material
                var material = new Material();
                // material.AddTexture(Material.Type.Albedo, Resource.GetTexture("rock_diffuse.png"));
                // material.AddTexture(Material.Type.Normal, Resource.GetTexture("rock_normal.png"));
                material.Roughness = 0f;
                material.Metalic = 1f;

                {
                    var meshs = Resource.GetMesh("oldlady-v2.obj");
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(1.5f, 0f, -1f);
                    meshGo.AddComponent(new MeshRenderer(meshs, material));
                    scene.Add(meshGo);
                }

                {
                    var meshs = Resource.GetMesh("oldlady-v2.obj");
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(0.5f, 0f, -1f);
                    meshGo.AddComponent(new MeshRenderer(meshs, material));
                    scene.Add(meshGo);
                }

                {
                    var meshs = Resource.GetMesh("oldlady-v2.obj");
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(-0.5f, 0f, -1f);
                    meshGo.AddComponent(new MeshRenderer(meshs, material));
                    scene.Add(meshGo);
                }

                {
                    var meshs = Resource.GetMesh("oldlady-v2.obj");
                    var meshGo = new GameObject();
                    meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);
                    meshGo.Transform.Position = new Vector3(-1.5f, 0f, -1f);
                    meshGo.AddComponent(new MeshRenderer(meshs, material));
                    scene.Add(meshGo);
                }
            } */
            var lightGo = new GameObject();
            var dirLight = new DirectionalLight();
            dirLight.Color = ParangEngine.Types.Color.White;
            dirLight.Intensity = 0.8f;
            lightGo.AddComponent(dirLight);
            lightGo.AddComponent(new Rotater());
            lightGo.Transform.Rotation = new Vector3(45f, 0f, 45f);
            scene.Add(lightGo);

            {
                var go = new GameObject();
                var l = new SpotLight();
                l.Color = new ParangEngine.Types.Color("red");
                l.Intensity = 8f;
                l.Length = 4f;
                l.Degree = 30f;
                go.Transform.Position = new Vector3(0f, 3f, 0f);
                go.Transform.Rotation = new Vector3(90f, 0f, 0f);
                go.AddComponent(l);
                scene.Add(go);
            }

            {
                var go = new GameObject();
                var l = new SpotLight();
                l.Color = new ParangEngine.Types.Color("blue");
                l.Intensity = 8f;
                l.Length = 4f;
                l.Degree = 30f;
                go.Transform.Position = new Vector3(1f, 3f, 0f);
                go.Transform.Rotation = new Vector3(90f, 0f, 0f);
                go.AddComponent(l);
                scene.Add(go);
            }


            /* {
                var go = new GameObject();
                var l = new PointLight();
                l.Color = new ParangEngine.Types.Color("blue");
                l.Intensity = 5f;
                l.Radius = 5f;
                go.Transform.Position = new Vector3(-2f, 1f, -2f);
                go.AddComponent(l);
                scene.Add(go);
            }

            {
                var go = new GameObject();
                var l = new PointLight();
                l.Color = new ParangEngine.Types.Color("yellow");
                l.Intensity = 5f;
                l.Radius = 5f;
                go.Transform.Position = new Vector3(2f, 1f, -2f);
                go.AddComponent(l);
                scene.Add(go);
            }

            {
                var go = new GameObject();
                var l = new PointLight();
                l.Color = new ParangEngine.Types.Color("green");
                l.Intensity = 5f;
                l.Radius = 5f;
                go.Transform.Position = new Vector3(2f, 1f, 2f);
                go.AddComponent(l);
                scene.Add(go);
            } */
            SceneManager.LoadScene(scene);
        }

        public void Start()
        {
            engine.Start();
        }

        public void Stop()
        {
            engine.Stop();
        }

        public void KeyDown(string key)
        {
            engine.KeyDown(key);
        }

        public void KeyUp(string key)
        {
            engine.KeyUp(key);
        }
    }
}