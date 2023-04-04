using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : PlacedObject
{
    private bool _isEaten = false;
    private bool _canBeUsed = false;
    [SerializeField]private Material _whiteColor;
    [SerializeField] private MeshRenderer _renderer;
    private int player = 0;


    public int GetPlayer(){
        return player;
    }

    public void SetPlayer(int p){
        player = p;
    }


    public void SetEaten(bool b){
        _isEaten = b;
    }

    public bool GetEaten(){
        return _isEaten;
    }

    public void ChangeColor(){
        _renderer.material = _whiteColor;
        player = 1;
    }

    public bool GetCanBeUsed(){
        return _canBeUsed;
    }

    void OnMouseEnter(){
        if(_currentTile != null)
            _currentTile.SetHighlightGrid(true);
        if(_isEaten){
            _canBeUsed = true;
        }
    }
    void OnMouseExit(){
        if(_currentTile != null)
            _currentTile.SetHighlightGrid(false);
        if(_isEaten){
            _canBeUsed = false;
        }
    }
}
