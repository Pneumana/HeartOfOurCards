using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class VNParticleSystem : MonoBehaviour
{
    public int population;
    // Range to draw meshes within.
    public float range;

    //private Vector2 _targetDirection = Vector2.left;

    // Material to use for drawing the meshes.
    public Material material;
    public Texture texture;

    private MaterialPropertyBlock block;

    public float width;
    public float height;

    private Matrix4x4[] matrices;

    private NativeArray<Vector2> _directionInput;
    private NativeArray<Vector2> _directionResult;
    private NativeArray<Vector2> _positionResult;
    private NativeArray<Vector2> _positionInput;
    public NativeArray<uint> _seeds;

    private Mesh mesh;

    JobHandle _jobHandle;

    // Start is called before the first frame update
    private void Awake()
    {
        _directionInput = new NativeArray<Vector2>(population, Allocator.Persistent);
        _directionResult = new NativeArray<Vector2>(population, Allocator.Persistent);
        _positionResult = new NativeArray<Vector2>(population, Allocator.Persistent);
        _positionInput = new NativeArray<Vector2>(population, Allocator.Persistent);
        _seeds = new NativeArray<uint>(population, Allocator.Persistent);

        material.enableInstancing = true;
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
            //loop and mak
            //sampler = CustomSampler.Create("Leaves");
            Mesh mesh = CreateQuad(width, height);
            this.mesh = mesh;

            matrices = new Matrix4x4[population];
            //colors = new Vector4[population];

            //block = new MaterialPropertyBlock();
            for (int i = 0; i < population; i++)
            {
                // Build matrix.
                //_speed[i] = baseSpeed;
                Vector3 position = new Vector3(transform.position.x + Random.Range(-range, range), transform.position.y, transform.position.z + Random.Range(-range, range));
                Quaternion rotation = Quaternion.Euler(Random.Range(60, 120), Random.Range(-180, 180), 0);
                Vector3 scale = Vector3.one;
                _positionInput[i] = position;
                //_rotation[i] = rotation;
                //_targetDirection[i] = Vector2.up;
                //Debug.Log("set target direction to " + _targetDirection[i]);
                var mat = Matrix4x4.TRS(position, rotation, scale);
                //Debug.Log(position);
                //_matrixResult[i] = mat;
                _seeds[i] = (uint)Random.Range(1, 1000);
                //colors[i] = Color.Lerp(Color.red, Color.blue, Random.value);
            }
            Debug.Log("setup " + population + " quads");
            // Custom shader needed to read these!!
    }

    private Mesh CreateQuad(float width = 1f, float height = 1f)
    {
        // Create a quad mesh.
        var mesh = new Mesh();

        float w = width * .5f;
        float h = height * .5f;
        var vertices = new Vector3[4] {
            new Vector3(-w, -h, 0),
            new Vector3(w, -h, 0),
            new Vector3(-w, h, 0),
            new Vector3(w, h, 0)
        };

        var tris = new int[6] {
            // lower left tri.
            0, 2, 1,
            // lower right tri
            2, 3, 1
        };

        var normals = new Vector3[4] {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };

        var uv = new Vector2[4] {
            new Vector3(0, 0),
            new Vector3(1, 0),
            new Vector3(0, 1),
            new Vector3(1, 1),
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }

    // Update is called once per frame
    void Update()
    {
        ParticleJob job = new ParticleJob(
            Vector2.down,
            Time.deltaTime,
            _seeds,
            _positionInput,
            _directionResult,
            _positionResult,
            _directionInput
            );
        _jobHandle = job.Schedule();
    }

    private void LateUpdate()
    {

        _jobHandle.Complete();

        /*//_cooldown = _coolDownResult[0];
        //var outputRotation = _rotationResult[0];
        var matrixArray = new Matrix4x4[1];
        //var cooldownArray = _cooldownResult.ToArray();
        for (int i = 0; i < population; i++)
        {
            var position = _positionResult[i];
            var mat = Matrix4x4.TRS(position, Quaternion.LookRotation(Vector3.forward), Vector3.one);
            matrixArray[0] = mat;
            //material.SetColor("_Albedo", colors[i]);
            //position = Camera.main.WorldToScreenPoint(position);
            //var screenRect = new Rect(position.x, position.y, 100, 100);
            //var sourceRect = new Rect(0,0, 1, 1);

            Graphics.DrawTexture(new Rect(position.x, position.y, 100, 100), texture);
        }
        Graphics.DrawTexture(new Rect(10, 10, 100, 100), texture);*/
    }

    private void OnGUI()
    {
        

        //_cooldown = _coolDownResult[0];
        //var outputRotation = _rotationResult[0];
        var matrixArray = new Matrix4x4[1];
        //var cooldownArray = _cooldownResult.ToArray();
        for (int i = 0; i < population; i++)
        {
            _positionInput[i] = _positionResult[i];
            var position = _positionResult[i];
            _directionInput[i] = _directionResult[i];
            var mat = Matrix4x4.TRS(position, Quaternion.LookRotation(Vector3.forward), Vector3.one);
            matrixArray[0] = mat;
            //material.SetColor("_Albedo", colors[i]);
            //position = Camera.main.WorldToScreenPoint(position);
            //var screenRect = new Rect(position.x, position.y, 100, 100);
            //var sourceRect = new Rect(0,0, 1, 1);

            Graphics.DrawTexture(new Rect(position.x, position.y, 100, 100), texture);
        }
    }

    private void OnDestroy()
    {
        _directionInput.Dispose();
        _directionResult.Dispose();
        _positionResult.Dispose();
        _positionInput.Dispose();
        _seeds.Dispose();
    }

    private void OnDrawGizmos()
    {
        foreach(Vector2 pos in _positionResult)
        {
            Debug.DrawLine(pos + Vector2.up, pos + Vector2.down);
            Debug.DrawLine(pos + Vector2.left, pos + Vector2.right);
        }
    }
}
