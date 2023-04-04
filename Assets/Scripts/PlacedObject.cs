using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacedObject : MonoBehaviour
{
    protected int x, y;
    protected Tile _currentTile;
    [SerializeField]protected Transform objTransform;
    protected string _type = "null";

    public void SetPos(Tile tile){
        SetTile(tile);
        objTransform.position = new Vector3(x, 0, y);
    }

    public void SetTile(Tile tile){
        x = tile.GetX();
        y = tile.GetY();
        _currentTile = tile;
    }

    public void SetTile(int x, int y){
        this.x = x;
        this.y = y;
        _currentTile = null;
    }

    public int[] GetPos(){
        return new int[] {x, y};
    }
    public int GetX(){
        return x;
    }
    public int GetY(){
        return y;
    }

    void OnMouseEnter(){
        if(_currentTile != null)
            _currentTile.SetHighlightGrid(true);
    }
    void OnMouseExit(){
        if(_currentTile != null)
            _currentTile.SetHighlightGrid(false);
    }

    public string GetObjectType(){
        return _type;
    }

    public void SetObjectType(string type){
        _type = type;
    }

}
