using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateEvn : MonoBehaviour {
    public Tilemap ground;
    public Tilemap lightmap;
    public Tilemap border;
    public Tile[] tiles = new Tile[16];
    public Tile darkness;
    public Tile border_tile;
    public int width = 101, height = 101;
    public float[] scale = { 10f, 16f, 8f };
    public float[] ore_treshhold = { 0.3f, 0.4f, 0.5f, 0.5f, 0.6f, 0.7f, 0.75f, 0.5f };
    public float light_intencity = 0.75f;

    static public float[,,] Environment;
    static public int[,] PlacedTileIndexes;

    static public Vector2Int getIndexs(Vector2 pos) {
        GenerateEvn gn = GameObject.FindGameObjectWithTag("Game").GetComponent<GenerateEvn>();
        Vector2Int v2i = new Vector2Int((int)pos.x + gn.width / 2 - 1, (int)pos.y + gn.height / 2 - 1);
        if ( v2i.x < 0 || v2i.x > gn.width || v2i.y < 0 || v2i.y > gn.height) {
            return new Vector2Int(-1,-1);
        }
        return v2i;
    }

    void Start() {
        Environment = new float[3, width+1, height+1];
        PlacedTileIndexes = new int[width+1,height+1];
        if (!ground) return;
        PerlinNoise();

        // in case the player got unlucky
        Environment[1, 0, 0] = 1;
        Environment[0, 0, 0] = 1;
        Environment[1, width, 0] = 1;
        Environment[0, width, 0] = 0.85f;
        Environment[1, 0, height] = 1;
        Environment[0, 0, height] = 0.7f;
        Environment[1, width, height] = 1;
        Environment[0, width, height] = 0.6f;

        for (int i = 0; i <= width; i++) {
            for (int j = 0; j <= height; j++) {
                int rock_type = Mathf.Min(Mathf.Max(Mathf.FloorToInt(Environment[0, i, j] * 8), 0), 7);

                int index_offset = 0;
                if (Environment[1, i, j] >= ore_treshhold[rock_type]) { // it is ore
                    index_offset = 8;
                }

                Vector3Int v3i = new Vector3Int(i - (width / 2), j - (height / 2), 0);
                float light = Environment[2, i, j] * light_intencity;

                ground.SetTile(v3i, tiles[rock_type + index_offset]);
                PlacedTileIndexes[i, j] = rock_type + index_offset;
                lightmap.SetTile(v3i, darkness);
                lightmap.SetTileFlags(v3i, TileFlags.None);
                lightmap.SetColor(v3i,new Color(0, 0, 0, light));
            }
        }
        Vector3Int v;
        for (int i = -1; i <= width+1; i++) {
            v = new Vector3Int(i - (width / 2), 0 - (height / 2) -1, 0);
            border.SetTile(v, border_tile);
            v = new Vector3Int(i - (width / 2), height - (height / 2) + 1, 0);
            border.SetTile(v, border_tile);
        }
        for (int i = -1; i <= height+1; i++) {
            v = new Vector3Int(0 - (width / 2) - 1, i - (height / 2) - 1, 0);
            border.SetTile(v, border_tile);
            v = new Vector3Int(width - (width / 2) + 1, i - (height / 2) + 1, 0);
            border.SetTile(v, border_tile);
        }
        border.GetComponent<TilemapCollider2D>().ProcessTilemapChanges();
    }

    private void PerlinNoise() {
        for (int l = 0; l < 3; l++) {
            float offsetX = Random.Range(0f, 1000f), offesetY = Random.Range(0f, 1000f);
            for (int i = 0; i <= width; i++) {
                float xf = (float)i / width * scale[l] + offsetX;
                for (int j = 0; j <= height; j++) {
                    float yf = (float)j / height * scale[l] + offesetY;

                    Environment[l, i, j] = Mathf.PerlinNoise(xf, yf);
                }
            }
        }
    }
}
