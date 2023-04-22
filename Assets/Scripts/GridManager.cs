using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridManager : MonoBehaviour
{

    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject _whiteKing;
    [SerializeField] private GameObject _rook;
    [SerializeField] private GameObject _bishop;
    [SerializeField] private GameObject _queen;
    [SerializeField] private GameObject _knight;
    [SerializeField] private GameObject _pawn;
    [SerializeField] private GameObject _bling;
    private List<List<Tile>> grid = new List<List<Tile>>();
    private List<Piece> pieces = new List<Piece>();
    private List<Piece> piecesEaten = new List<Piece>();
    private List<Bling> blings = new List<Bling>();
    private int[] currentTileIndex = new int[2] {-1, -1};
    private Tile currentTile = null;
    private Tile currentHighlight = null;
    private string[] _typeArray = new string[] {"q", "n", "b", "r", "p"};
    private List<int[]> knightMovement = new List<int[]>() {new int[]{-1, 2}, new int[]{1, 2}, new int[]{2, 1}, new int[]{2, -1}
        , new int[]{-1, -2}, new int[]{1, -2}, new int[]{-2, 1}, new int[]{-2, -1}};
    public int _kingDies = 0;
    private int player = 1;
    private int nbUsedPieces = 0;
    public List<Vector2> piecelistPos;
    public List<string> piecelistName;
    public int blingLeft;
    public int movesLeft;
    public bool SpecialBlingQuantity = false;
    public CanvasManager canvasManager;

    void Start(){
        GenerateGrid();
        for(int i = 0; i < piecelistPos.Count; i ++){
            int posx = (int)piecelistPos[i].x;
            int posy = (int)piecelistPos[i].y;
            if (piecelistName[i] == "k"){
                CreateNewPiece(_whiteKing, "k", posx, posy);
                currentTile = grid[posx][posy];
                piecesEaten.Add(ToPiece(grid[posx][posy].GetCurrentObject()));
                ToPiece(grid[posx][posy].GetCurrentObject()).SetPlayer(1);
            }
            if (piecelistName[i] == "r"){
                CreateNewPiece(_rook, "r", posx, posy);
            }
            if (piecelistName[i] == "b"){
                CreateNewPiece(_bishop, "b", posx, posy);
            }
            if (piecelistName[i] == "q"){
                CreateNewPiece(_queen, "q", posx, posy);
            }
            if (piecelistName[i] == "n"){
                CreateNewPiece(_knight, "n", posx, posy);
            }
            if (piecelistName[i] == "p"){
                CreateNewPiece(_pawn, "p", posx, posy);
            }
            if (piecelistName[i] == "bling"){
                CreateNewBling(posx, posy);
                if(!SpecialBlingQuantity){
                    blingLeft ++;
                }
            }
            
        }
    }

    void CreateNewPiece(GameObject obj, string pieceType, int x,int y){
        var spawnedObject = Instantiate(obj, new Vector3(x, 0, y), Quaternion.identity);
        var newPiece = spawnedObject.GetComponent<Piece>();
        newPiece.SetPos(grid[x][y]);
        newPiece.SetObjectType(pieceType);
        grid[x][y].SetCurrentObject(newPiece);
        pieces.Add(newPiece);

    }    
    
    void CreateNewBling(int x,int y){
        var spawnedObject = Instantiate(_bling, new Vector3(x, 0, y), Quaternion.identity);
        var newBling = spawnedObject.GetComponentInChildren<Bling>();
        newBling.SetPos(grid[x][y]);
        newBling.SetObjectType("bling");
        grid[x][y].SetCurrentObject(newBling);
        blings.Add(newBling);

    }

    void GenerateGrid(){
        for (int x = 0; x < _width; x++){
            grid.Add(new List<Tile>());
            for (int y = 0; y < _height; y ++){
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.SetX(x);
                spawnedTile.SetY(y);

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (y % 2 == 0 && x % 2 != 0);
                spawnedTile.Init(isOffset);

                spawnedTile.SetGrid(this);

                grid[x].Add(spawnedTile);
            }
        }
    }

    public void SetCurrentHighlight(Tile h, bool b){
        if(b){
            currentHighlight = h;
            currentHighlight.setHighlighted(true);
        }
        else if(currentHighlight = h){
            currentHighlight.setHighlighted(false);
            currentHighlight = null;
        }
    }

    public void SetSelectedPos(int x, int y){
        currentTileIndex = new int[2] {x, y};
        if(x >= 0 && y >= 0){
            currentTile = grid[currentTileIndex[0]][currentTileIndex[1]];
        }
        else{
            currentTile = null;
        }
    }

    public void MoveSelectedPos(int x, int y){
        StartCoroutine(MovingPiece(currentTile.GetCurrentObject()));
    }

    IEnumerator MovingPiece(PlacedObject piece, int speed = 6)
    {
        while(piece.transform.position != new Vector3(piece.GetX(), 0, piece.GetY())){
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, new Vector3(piece.GetX(), 0, piece.GetY()), speed*Time.deltaTime);
            yield return null;
        }
        if(_kingDies == 1){
            _kingDies = 2;
            ThrowKing();
        }
    }

    public bool CanUseTile(int x, int y){
        return (x >= 0 && x < _width && y >= 0 && y < _height);
    }

    private void UnselectCanMoveTo(){
        for(int i = 0; i < _width; i++){
            for (int j = 0; j < _height; j++){
                grid[i][j].SetMoveTo(false);
            }
        }
    }

    private void Unselect(){
        if(currentTile != null){
            currentTile.setSelected(false);
        }
        UnselectCanMoveTo();
    }

    private void ShowCanMoveTo(){
        List<Tile> possibleTiles = new List<Tile>();
        if(currentTile.GetObjectType() == "k"){
            possibleTiles = KingMovement(currentTile.GetCurrentObject());
        }
        if(currentTile.GetObjectType() == "r"){
            possibleTiles = RookMovement(currentTile.GetCurrentObject());
        }
        if(currentTile.GetObjectType() == "b"){
            possibleTiles = BishopMovement(currentTile.GetCurrentObject());
        }
        if(currentTile.GetObjectType() == "q"){
            possibleTiles = QueenMovement(currentTile.GetCurrentObject());
        }
        if(currentTile.GetObjectType() == "n"){
            possibleTiles = KnightMovement(currentTile.GetCurrentObject());
        }
        if(currentTile.GetObjectType() == "p"){
            possibleTiles = PawnMovement(currentTile.GetCurrentObject(), true);
        }
        foreach(Tile t in possibleTiles){
            t.SetMoveTo(true);
        }
    }

    private List<Tile> KingMovement(PlacedObject obj){
        List<Tile> returning = new List<Tile>();
        for(int i = -1; i <= 1; i++){
            for(int j = -1; j <= 1; j++){
                if(CanUseTile(obj.GetX()+i, obj.GetY()+j) && (i!= 0 || j!= 0)){
                    returning.Add(grid[obj.GetX()+i][obj.GetY()+j]);
                }
            }
        }
        return returning;
    }    
    
    private List<Tile> RookMovement(PlacedObject obj){
        List<Tile> returning = new List<Tile>();
        for(int i = -1; i <= 1; i++){
            int tot = i;
            bool canContinue = true;
            while(CanUseTile(obj.GetX()+tot, obj.GetY()) && i!= 0 && canContinue){
                returning.Add(grid[obj.GetX()+tot][obj.GetY()]);
                if(IsPiece(grid[obj.GetX()+tot][obj.GetY()].GetObjectType())){
                    canContinue = false;
                }
                tot += i;
            }
        }
        for(int i = -1; i <= 1; i++){
            int tot = i;
            bool canContinue = true;
            while(CanUseTile(obj.GetX(), obj.GetY()+tot) && i!= 0 && canContinue){
                returning.Add(grid[obj.GetX()][obj.GetY()+tot]);
                if(IsPiece(grid[obj.GetX()][obj.GetY()+tot].GetObjectType())){
                    canContinue = false;
                }
                tot += i;
            }
        }
        return returning;
    }

    private List<Tile> BishopMovement(PlacedObject obj){
        List<Tile> returning = new List<Tile>();
        for(int i = -1; i <= 1; i++){
            int tot = i;
            bool canContinue = true;
            while(CanUseTile(obj.GetX()+tot, obj.GetY()+tot) && i!= 0 && canContinue){
                returning.Add(grid[obj.GetX()+tot][obj.GetY()+tot]);
                if(IsPiece(grid[obj.GetX()+tot][obj.GetY()+tot].GetObjectType())){
                    canContinue = false;
                }
                tot += i;
            }
        }
        for(int i = -1; i <= 1; i++){
            int tot = i;
            bool canContinue = true;
            while(CanUseTile(obj.GetX()-tot, obj.GetY()+tot) && i!= 0 && canContinue){
                returning.Add(grid[obj.GetX()-tot][obj.GetY()+tot]);
                if(IsPiece(grid[obj.GetX()-tot][obj.GetY()+tot].GetObjectType())){
                    canContinue = false;
                }
                tot += i;
            }
        }
        return returning;
    }
    
    private List<Tile> QueenMovement(PlacedObject obj){
        List<Tile> returning = new List<Tile>();
        returning.AddRange(BishopMovement(obj));
        returning.AddRange(RookMovement(obj));
        return returning;
    }

    private List<Tile> KnightMovement(PlacedObject obj){
        List<Tile> returning = new List<Tile>();
        foreach(int[] km in knightMovement){
            if(CanUseTile(obj.GetX()+km[0], obj.GetY()+km[1])){
                returning.Add(grid[obj.GetX()+km[0]][obj.GetY()+km[1]]);
            }
        }
        return returning;
    }

    private List<Tile> PawnMovement(PlacedObject obj, bool isWhite){
        List<Tile> returning = new List<Tile>();
        if(CanUseTile(obj.GetX(),obj.GetY()+1 )){
            if(grid[obj.GetX()][obj.GetY()+1].GetCurrentObject() == null && isWhite){
                returning.Add(grid[obj.GetX()][obj.GetY()+1]);
            }
        }
        if(CanUseTile(obj.GetX()-1,obj.GetY()+1 )){
            if(grid[obj.GetX()-1][obj.GetY()+1].GetCurrentObject() != null || !isWhite){
                returning.Add(grid[obj.GetX()-1][obj.GetY()+1]);
            }
        }
        if(CanUseTile(obj.GetX()+1,obj.GetY()+1 )){
            if(grid[obj.GetX()+1][obj.GetY()+1].GetCurrentObject() != null || !isWhite){
                returning.Add(grid[obj.GetX()+1][obj.GetY()+1]);
            }
        }

            
        return returning;
    }

    private bool IsPiece(string s){
        foreach(string i in _typeArray){
            if (i == s){
                return true;
            }
        }
        return false;
    }

    private Piece ToPiece(PlacedObject obj){
        foreach(Piece p in pieces){
            if(p == obj){
                return p;
            }
        }
        return null;
    }

    private Bling ToBling(PlacedObject obj){
        foreach(Bling b in blings){
            if(b == obj){
                return b;
            }
        }
        return null;
    }

    private void EatPiece(PlacedObject obj){
        Piece p = ToPiece(obj);
        p.SetEaten(true);
        p.SetTile(-1, piecesEaten.Count-1);
        StartCoroutine(MovingPiece(p, 10));
        piecesEaten.Add(p);
        p.ChangeColor();
        currentHighlight.SetCurrentObject(null);
    }

    private void VerifyOkCases(){
        foreach(Piece p in pieces){
            List<Tile> l = new List<Tile>();
            if (p.GetEaten() == false && _kingDies == 0){
                if(p.GetObjectType() == "r"){
                    l.AddRange(RookMovement(p));
                }
                if(p.GetObjectType() == "b"){
                    l.AddRange(BishopMovement(p));
                }
                if(p.GetObjectType() == "q"){
                    l.AddRange(QueenMovement(p));
                }
                if(p.GetObjectType() == "n"){
                    l.AddRange(KnightMovement(p));
                }
                if(p.GetObjectType() == "p"){
                    l.AddRange(PawnMovement(p, false));
                }
                foreach(Tile t in l){
                    if(t == currentTile){
                        _kingDies = 1;
                        StartCoroutine(KillKing(t.GetX(), t.GetY(), p));
                    }
                }
            }
        }
    }

    IEnumerator KillKing(int x, int y, PlacedObject piece, int speed = 4)
    {
        while(_kingDies != 2){
            yield return null;
        }
        Unselect();
        piece.SetTile(x, y);
        while(piece.transform.position != new Vector3(x, 0, y)){
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, new Vector3(x, 0, y), speed*Time.deltaTime);
            yield return null;
        }
    }

    private void ExchangePiece(Piece p){
        if(currentTile != null){
            Piece p2 = ToPiece(currentTile.GetCurrentObject());
            p2.SetTile(p.GetX(), p.GetY());
            currentTile.SetCurrentObject(p);
            p2.SetEaten(true);
            StartCoroutine(MovingPiece(p,8));
            StartCoroutine(MovingPiece(p2,8));
            UnselectCanMoveTo();
            ShowCanMoveTo();
        }
        
    }
    
    private void ThrowPiece(){
        currentTile.GetCurrentObject().SetTile(_width, nbUsedPieces);
        piecesEaten.Remove(ToPiece(currentTile.GetCurrentObject()));
        nbUsedPieces ++;
        ToPiece(currentTile.GetCurrentObject()).ChangeColorGrey();
        StartCoroutine(MovingPiece(currentTile.GetCurrentObject(),8));
        Piece p2 = null;
        int posYmin = 0;
        foreach(Piece p in piecesEaten){
            if(p.GetObjectType() == "k"){
                p2 = p;
                posYmin = p.GetY();
            }
            else if (p.GetY() > posYmin){
                p.SetTile(p.GetX(), p.GetY()-1);
                StartCoroutine(MovingPiece(p,8));
            }
        }
        currentTile.SetCurrentObject(p2);
        StartCoroutine(MovingPiece(p2,8));
        UnselectCanMoveTo();
    }
    
    private void ThrowKing(){
        currentTile.GetCurrentObject().SetTile(_width, nbUsedPieces);
        ToPiece(currentTile.GetCurrentObject()).ChangeColorGrey();
        StartCoroutine(MovingPiece(currentTile.GetCurrentObject(),8));
        UnselectCanMoveTo();
    }

    private void gatherbling(int x, int y){
        Destroy(grid[x][y].GetCurrentObject().gameObject);
        blingLeft --;
        grid[x][y].SetCurrentObject(null);
    }

    private void GetBling(int newposX, int newposY, int prevposX, int prevposY){
        int relative_pos_x = newposX - prevposX;
        int relative_pos_y = newposY - prevposY;
        int rel_pos_x_1 = relative_pos_x/Math.Abs(relative_pos_x == 0 ? 1 : relative_pos_x);
        int rel_pos_y_1 = relative_pos_y/Math.Abs(relative_pos_y == 0 ? 1 : relative_pos_y);
        while((relative_pos_x != 0 || relative_pos_y != 0)){
            if(grid[relative_pos_x+prevposX][relative_pos_y+prevposY].GetObjectType() == "bling"){
                gatherbling(relative_pos_x+prevposX, relative_pos_y+prevposY);
            }
            relative_pos_x -= rel_pos_x_1;
            relative_pos_y -= rel_pos_y_1;
        }
    }

    private void Promotion(){
        if(currentTile.GetY() == _height-1){
            bool done = false;
            Piece deleted = null;
            foreach(Piece p in piecesEaten){
                if (p.GetObjectType() == "p" && !done){
                    done = true;
                    deleted = p;
                }
            }
            if(done && deleted != null){
                var spawnedObject = Instantiate(_queen, new Vector3(deleted.GetX(), 0, deleted.GetY()), Quaternion.identity);
                var newPiece = spawnedObject.GetComponent<Piece>();
                newPiece.SetTile(deleted.GetX(), deleted.GetY());
                newPiece.SetObjectType("q");
                pieces.Add(newPiece);
                piecesEaten.Add(newPiece);
                newPiece.SetEaten(true);
                newPiece.ChangeColor(); 
                piecesEaten.Remove(deleted);
                Destroy(deleted.gameObject);
            }
        }
    }


    void Update(){
        if (blingLeft <= 0 && _kingDies <= 0){
            _kingDies = -1;
            Unselect();
            canvasManager.Win();
        }
        else if(movesLeft <= 0 || _kingDies == 2){
            _kingDies = 2;
            Unselect();
            canvasManager.Lose();
        }
        if (Input.GetMouseButtonDown(0) &&_kingDies == 0 && !CanvasManager.gamePaused){
            if(currentHighlight != null){
                if(currentHighlight.CanMove() && currentHighlight != currentTile){//if we select a piece
                    if(ToPiece(currentTile.GetCurrentObject()).GetPlayer() == player){//if the piece is the king
                        //gather bling
                        if(currentTile.GetObjectType() != "n"){
                            GetBling(currentHighlight.GetX(), currentHighlight.GetY(), currentTile.GetX(), currentTile.GetY());
                        }
                        else{
                            if(currentHighlight.GetObjectType() == "bling"){
                                gatherbling(currentHighlight.GetX(), currentHighlight.GetY());
                            }
                        }
                        //eat piece
                        if(IsPiece(currentHighlight.GetObjectType())){
                            EatPiece(currentHighlight.GetCurrentObject());
                        }
                        //then move
                        UnselectCanMoveTo();
                        currentHighlight.SetCurrentObject(currentTile.GetCurrentObject());
                        MoveSelectedPos(currentHighlight.GetX(), currentHighlight.GetY());
                        currentTile.SetCurrentObject(null);
                        currentTile.setSelected(false);
                        currentHighlight.setSelected(true);
                        currentTile = currentHighlight;
                        Promotion();
                        if(currentTile.GetObjectType() != "k"){
                            ThrowPiece();
                        }
                        movesLeft --;
                        ShowCanMoveTo();
                    }
                    else{
                        Unselect();
                    }
                }
                else{
                    if(currentTile != null){//unselect previously selected tile
                        Unselect();
                    }
                    if(currentHighlight.GetCurrentObject() != null){//select tile where there is a piece
                        currentHighlight.setSelected(true);
                        currentTile = currentHighlight;

                        ShowCanMoveTo();
                        
                    }
                }
            }
            else{
                bool unselect = true;
                if(currentTile != null){
                    if(ToPiece(currentTile.GetCurrentObject()).GetPlayer() == player){
                        foreach(Piece p in piecesEaten){
                            if (p.GetCanBeUsed()){
                                ExchangePiece(p);
                                unselect = false;
                            }
                        }
                    }
                }
                if (unselect){
                    Unselect();
                    currentTile = null;
                }
            }
            if(currentTile != null){
                if(ToPiece(currentTile.GetCurrentObject()).GetPlayer() == player){
                    VerifyOkCases();
                }
            }
        }
    }
}
