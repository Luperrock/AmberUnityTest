/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

// 0 Empty
// 1 Obstacle
// 2 Player
// 3 Possible selection

public class Testing : MonoBehaviour {

    private Grid grid;
    private bool isMovePending;

    private void Start() {
        grid = new Grid(10, 10, 5f, new Vector3(0, 0));
        grid.GenerateRandomObstacles(20);
        HeatMapVisual heatMapVisual = new HeatMapVisual(grid, GetComponent<MeshFilter>());
    }

    private void Update() {
        HandleClickToModifyGrid();
    }

    private void HandleClickToModifyGrid() {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 MousePosition = UtilsClass.GetMouseWorldPosition();
            if (grid.IsPositionWithinBounds(MousePosition))
            {
                int gridValue = grid.GetValue(MousePosition);
                if (gridValue == 0)
                {
                    int x, y;
                    grid.GetXY(MousePosition, out x, out y);
                    grid.SetPlayerPosition(x,y);
                }
            }
        }
    }

    public void OnNextPressed()
    {
        if (isMovePending)
        {
            isMovePending = false;
            grid.MovePlayer();
        }
        else
        {
            grid.GetNextMove(out var nextX, out var nextY);
            grid.SetPendingMove(nextX, nextY);
            isMovePending = true;
        }
    }

    public void OnClearPressed()
    {
        isMovePending = false;
        grid.ClearBoard();
    }


    private class HeatMapVisual {

        private Grid grid;
        private Mesh mesh;

        public HeatMapVisual(Grid grid, MeshFilter meshFilter) {
            this.grid = grid;
            
            mesh = new Mesh();
            meshFilter.mesh = mesh;

            UpdateHeatMapVisual();

            grid.OnGridValueChanged += Grid_OnGridValueChanged;
        }

        private void Grid_OnGridValueChanged(object sender, System.EventArgs e) {
            UpdateHeatMapVisual();
        }

        public void UpdateHeatMapVisual() {
            Vector3[] vertices;
            Vector2[] uv;
            int[] triangles;

            MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out vertices, out uv, out triangles);

            for (int x = 0; x < grid.GetWidth(); x++) {
                for (int y = 0; y < grid.GetHeight(); y++) {
                    int index = x * grid.GetHeight() + y;
                    Vector3 baseSize = new Vector3(1, 1) * grid.GetCellSize();
                    int gridValue = grid.GetValue(x, y);
                    int maxGridValue = 4;
                    float gridValueNormalized = Mathf.Clamp01((float)gridValue / maxGridValue);
                    Vector2 gridCellUV = new Vector2(gridValueNormalized, 0f);
                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + baseSize * .5f, 0f, baseSize, gridCellUV, gridCellUV);
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }

    }
}
