/* Taken from http://answers.unity3d.com/questions/391561/create-a-mesh-and-color-cubes.html */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaceColoredCube : MonoBehaviour {
	private Mesh mesh;

	void Start() {
		MeshFilter mf = GetComponent<MeshFilter>();
		if (mf == null)
			return;
		mesh = MakeCube(1.0f);
		mf.mesh = mesh;
		MeshCollider mc = GetComponent<MeshCollider>();
		if (mc != null)
			mc.sharedMesh = mesh;
	}

	public void ChangeColor() {
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hit;
		if (!this.gameObject.GetComponent<Collider>().Raycast(ray, out hit, 100f))
			return;

		Color colorT = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
		Color[] colors = mesh.colors;
		int iStart = mesh.triangles[hit.triangleIndex * 3];
		for (int i = iStart; i < iStart + 4; i++)
			colors[i] = colorT;
		mesh.colors = colors;
	}

	Mesh MakeCube(float fSize) {
		float fHS = fSize / 2.0f;
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[] {
			new Vector3(-fHS,  fHS, -fHS), new Vector3( fHS,  fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3(-fHS, -fHS, -fHS),  // Front
            new Vector3(-fHS,  fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS,  fHS, -fHS), new Vector3(-fHS,  fHS, -fHS),  // Top
            new Vector3(-fHS, -fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS,  fHS,  fHS), new Vector3(-fHS,  fHS,  fHS),  // Back
            new Vector3(-fHS, -fHS, -fHS), new Vector3( fHS, -fHS, -fHS), new Vector3( fHS, -fHS,  fHS), new Vector3(-fHS, -fHS,  fHS),  // Bottom
            new Vector3(-fHS,  fHS,  fHS), new Vector3(-fHS,  fHS, -fHS), new Vector3(-fHS, -fHS, -fHS), new Vector3(-fHS, -fHS,  fHS),  // Left
            new Vector3( fHS,  fHS, -fHS), new Vector3( fHS,  fHS,  fHS), new Vector3( fHS, -fHS,  fHS), new Vector3( fHS, -fHS, -fHS)}; // right

		int[] triangles = new int[mesh.vertices.Length / 4 * 2 * 3];
		int iPos = 0;
		for (int i = 0; i < mesh.vertices.Length; i = i + 4) {
			triangles[iPos++] = i;
			triangles[iPos++] = i + 1;
			triangles[iPos++] = i + 2;
			triangles[iPos++] = i;
			triangles[iPos++] = i + 2;
			triangles[iPos++] = i + 3;
		}

		mesh.triangles = triangles;
		Color colorT = Color.red;
		Color[] colors = new Color[mesh.vertices.Length];
		for (int i = 0; i < colors.Length; i++) {
			if ((i % 4) == 0)
				colorT = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
			colors[i] = colorT;
		}

		mesh.colors = colors;
		mesh.RecalculateNormals();
		return mesh;
	}
}