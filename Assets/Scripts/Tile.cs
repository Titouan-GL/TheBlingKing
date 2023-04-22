using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Material _baseColor, _offsetColor, _highlightColor, _SelectColor, _CanMoveToColor;
    [SerializeField] private MeshRenderer _renderer;
    private GridManager _grid;
    private PlacedObject _currentObj;
    private Material _actualColor;
    private int x,y;
    private bool isHighlighted = false;
    private bool isSelected = false;
    private bool canMoveTo = false;
    

    public void Init(bool isOffset){
        _actualColor = isOffset ? _offsetColor : _baseColor;
        _renderer.material = _actualColor;
    }

    public void SetX(int x){
        this.x = x;
    }

    public void SetY(int y){
        this.y = y;
    }

    public int GetX(){
        return x;
    }
    public int GetY(){
        return y;
    }

    public bool CanMove(){
        return canMoveTo;
    }

    public void SetGrid(GridManager grid){
        _grid = grid;
    }

    public void setHighlighted(bool h){
        if(_grid._kingDies == 0){
            isHighlighted = h;
            if (!isSelected){
                if(h){
                    _renderer.material = _highlightColor;
                }
                else if(canMoveTo){
                    _renderer.material = _CanMoveToColor;
                }
                else{
                    _renderer.material = _actualColor;
                }
            }
        }
    }

    public void setSelected(bool s){
        isSelected = s;
        if(s){
            _renderer.material = _SelectColor;
        }
        else{
            _renderer.material = _actualColor;
        }
    }

    public void SetMoveTo(bool m){
        canMoveTo = m;
        if (!isHighlighted && !isSelected){
            if(m){
                _renderer.material = _CanMoveToColor;
            }
            else{
                _renderer.material = _actualColor;
            }
        }
    }

    public void SetHighlightGrid(bool b){
        _grid.SetCurrentHighlight(this, b);
    }

    void OnMouseEnter(){
        if(!CanvasManager.gamePaused)
            SetHighlightGrid(true);
    }
    void OnMouseExit(){
        if(!CanvasManager.gamePaused)
            SetHighlightGrid(false);
    }

    public void ChangeCurrentPiece(Piece piece, string type){
        if(type != "none"){
            _currentObj = piece;
            _currentObj.SetObjectType(type);
        }
        else{
            _currentObj = null;
        }
    }    

    public PlacedObject GetCurrentObject(){
        return _currentObj;
    }

    public void SetCurrentObject(PlacedObject po){
        _currentObj = po;
        if(po != null){
            _currentObj.SetTile(this);
        }
    }

    public string GetObjectType(){
        if(_currentObj == null){
            return null;
        }
        else{
            return _currentObj.GetObjectType();
        }
    }
 
}
