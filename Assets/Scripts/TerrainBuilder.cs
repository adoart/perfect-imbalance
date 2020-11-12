using UnityEngine;
using System.Collections;

public class TerrainBuilder : MonoBehaviour {

	public Texture2D heightMap;
	public Texture2D mapImage;

	public Material material;
	public Vector3 size = new Vector3(200, 30, 200);
	public int triangleScale = 1;
	public int colorRadius = 10;
	public float colorAverage;

	void Awake() {
		CreateWorld();
		applyTexture();

	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		GenerateHeightmap();
	}
	//called by game manager
	public void updateMap(TankManager[] tanks) {

		Texture2D texture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;

		Color[] textureColors = texture.GetPixels ();
		float colorTotal = 0f;
		foreach (Color color in textureColors) {
			colorTotal += color.grayscale;
		}
		colorAverage = colorTotal / textureColors.Length;

		foreach (TankManager tank in tanks) {
			Color[] colorBlock = new Color[tank.colorRadius * tank.colorRadius];
			for (int i = 0; i<colorBlock.Length; i++) {
				colorBlock[i] = tank.m_PlayerColor;
			}
			int convertedx = (int) (((tank.m_Instance.transform.position.x + size.x/2f) / size.x) * texture.height);
			int convertedz = (int) (((tank.m_Instance.transform.position.z + size.z/2f) / size.z) * texture.width);

			texture.SetPixels(convertedx, convertedz, tank.colorRadius, tank.colorRadius, colorBlock);
			//Debug.Log("Tank position x(int):" + (int) tank.m_Instance.transform.position.x);
			//Debug.Log("Tank position converted x :" + convertedx);
			//Debug.Log("Tank position z(int):" + (int) tank.m_Instance.transform.position.z);
			//Debug.Log("Tank position converted z :" + convertedz);
			tank.updateColorRadius(colorAverage);
			//tank.updateHealth(colorAverage);
			tank.updateSpeed(colorAverage);
		}

		texture.Apply();
	}

	public void resetMap() {
		applyTexture ();
	}
	
	private void CreateWorld() {
		// Create the game object containing the renderer
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshCollider> ();
		if (material) {
			GetComponent<Renderer>().material = material;
		} else {
			GetComponent<Renderer>().material.color = Color.white;
		}

		GenerateHeightmap();
	}

	private void applyTexture() {
		Texture2D newTexture = new Texture2D (mapImage.width, mapImage.height, TextureFormat.RGB24, false);
		newTexture.SetPixels (mapImage.GetPixels ());
		GetComponent<Renderer>().material.mainTexture = newTexture;

		newTexture.Apply();
	}


	private void GenerateHeightmap () {
		// Retrieve a mesh instance
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		
		int width = Mathf.Min(heightMap.width, 255) / triangleScale;
		int height = Mathf.Min(heightMap.height, 255) / triangleScale;
		int y = 0;
		int x = 0;
		
		// Build vertices and UVs
		Vector3[] vertices = new Vector3[height * width];
		Vector2[] uv = new Vector2[height * width];
		Vector4[] tangents = new Vector4[height * width];
		
		Vector2 uvScale = new Vector2 (1.0f / (width - 1), 1.0f / (height - 1));
		Vector3 sizeScale = new Vector3 (size.x / (width - 1) , size.y, size.z / (height - 1) );
		
		for (y=0;y<height;y++)
		{
			for (x=0;x<width;x++)
			{
				float pixelHeight = heightMap.GetPixel(x, y).grayscale;
				Vector3 vertex = new Vector3 (x, pixelHeight, y);
				vertices[y*width + x] = Vector3.Scale(sizeScale, vertex);
				uv[y*width + x] = Vector2.Scale(new Vector2 (x, y), uvScale);

				// Calculate tangent vector: a vector that goes from previous vertex
				// to next along X direction. We need tangents if we intend to
				// use bumpmap shaders on the mesh.
				Vector3 vertexL = new Vector3( x-1, heightMap.GetPixel(x-1, y).grayscale, y );
				Vector3 vertexR = new Vector3( x+1, heightMap.GetPixel(x+1, y).grayscale, y );
				Vector3 tan = Vector3.Scale( sizeScale, vertexR - vertexL ).normalized;
				tangents[y*width + x] = new Vector4( tan.x, tan.y, tan.z, -1.0f );
			}
		}
		
		// Assign them to the mesh
		mesh.vertices = vertices;
		mesh.uv = uv;
		
		// Build triangle indices: 3 indices into vertex array for each triangle
		int[] triangles = new int[(height - 1) * (width - 1) * 6];
		int index = 0;
		for (y=0;y<height-1;y++)
		{
			for (x=0;x<width-1;x++)
			{
				// For each grid cell output two triangles
				triangles[index++] = (y     * width) + x;
				triangles[index++] = ((y+1) * width) + x;
				triangles[index++] = (y     * width) + x + 1;
				
				triangles[index++] = ((y+1) * width) + x;
				triangles[index++] = ((y+1) * width) + x + 1;
				triangles[index++] = (y     * width) + x + 1;
			}
		}
		// And assign them to the mesh
		mesh.triangles = triangles;
		
		// Auto-calculate vertex normals from the mesh
		mesh.RecalculateNormals();
		
		// Assign tangents after recalculating normals
		mesh.tangents = tangents;

		GetComponent<MeshCollider> ().sharedMesh = mesh;
	}
}
