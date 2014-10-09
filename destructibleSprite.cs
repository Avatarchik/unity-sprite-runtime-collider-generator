using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class destructionPolygon : MonoBehaviour {

	public Texture2D tex;

	public bool doBI;
	public bool doErosion;
	public bool doDilation;
	public bool doSub;
	public bool doVert;
	
	public float pixelsToUnits = 100f; // Pixels to (unity)Units  100 to 1 
	public float pixelOffset = 0.5f;

	public bool[,] binaryImage;

	PolygonCollider2D poly;

	public float xBounds;
	public float yBounds;

	public int islandCount=0;

	// Use this for initialization
	void Start () {

		poly = gameObject.GetComponent<PolygonCollider2D>();

		if(poly == null)
			poly = gameObject.AddComponent<PolygonCollider2D>();

		if(tex == null)
			tex = gameObject.GetComponent<SpriteRenderer>().sprite.texture;

	 	xBounds = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
		yBounds = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.extents.y;

		binaryImageFromTex(out binaryImage, ref tex);

		gameObject.GetComponent<SpriteRenderer>().sprite.texture.wrapMode = TextureWrapMode.Clamp;
		
		//		texFromBinaryImage(out tex, binaryImage);
//		gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, gameObject.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
	}

	void Update() {
		if(doBI) {
			doBI=false;
			texFromBinaryImage(out tex, binaryImage);
			gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, gameObject.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
		}
		
		if(doErosion) {
			doErosion=false;

			texFromBinaryImage(out tex, erosion(ref binaryImage));
			gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, gameObject.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
		}
		
		if(doDilation) {
			doDilation=false;

			texFromBinaryImage(out tex, dilation(ref binaryImage));
			gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, gameObject.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
		}
		
		if(doSub) {
			doSub=false;

			texFromBinaryImage(out tex, subtraction(binaryImage, erosion(ref binaryImage)));
			gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, gameObject.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));
		}
		
		if(doVert) {
			doVert=false;
			bool[,] binaryImageOutline = subtraction(binaryImage, erosion(ref binaryImage));
			List<List<Vector2> > paths = getPaths(ref binaryImageOutline);

			texFromBinaryImageUsingPaths(out tex, binaryImage, paths);
			gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, gameObject.GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f));

			poly.pathCount = paths.Count;
			islandCount = paths.Count;

			for(int i=0; i<paths.Count; i++) {
				for(int j=0; j<paths[i].Count; j++) {
					
					
					paths[i][j] = new Vector2((paths[i][j].x + pixelOffset)/pixelsToUnits - xBounds, (paths[i][j].y + pixelOffset)/pixelsToUnits - yBounds);
					
				}
				poly.SetPath( i, paths[i].ToArray() );
			}
		}
	}

	public void updateCollider() {

		tex = gameObject.GetComponent<SpriteRenderer>().sprite.texture;
		tex.wrapMode = TextureWrapMode.Clamp;

		binaryImageFromTex(out binaryImage, ref tex);

		bool[,] binaryImageOutline = subtraction(binaryImage, erosion(ref binaryImage));
		List<List<Vector2> > paths = getPaths(ref binaryImageOutline);
		// simplify paths
		// convert paths to world space paths
		
		poly.pathCount = paths.Count;
		islandCount = paths.Count;
		
		for(int i=0; i<paths.Count; i++) {
			for(int j=0; j<paths[i].Count; j++) {
				paths[i][j] = new Vector2((paths[i][j].x + pixelOffset)/pixelsToUnits - xBounds, (paths[i][j].y + pixelOffset)/pixelsToUnits - yBounds);			
			}
			poly.SetPath( i,paths[i].ToArray() );
		}
	}

	public void updateCollider(Texture2D t) {

//		tex = t;

		binaryImageFromTex(out binaryImage, ref t);
		
		bool[,] binaryImageOutline = subtraction(binaryImage, erosion(ref binaryImage));
		List<List<Vector2> > paths = getPaths(ref binaryImageOutline);
		// simplify paths
		// convert paths to world space paths

		poly.pathCount = paths.Count;
		islandCount = paths.Count;
		
		for(int i=0; i<paths.Count; i++) {
			for(int j=0; j<paths[i].Count; j++) {
				paths[i][j] = new Vector2((paths[i][j].x + pixelOffset)/pixelsToUnits - xBounds, (paths[i][j].y + pixelOffset)/pixelsToUnits - yBounds);			
			}
			poly.SetPath( i, paths[i].ToArray() );
		}
	}

	// reduces the vert count about 90%
	List<Vector2> simplifyPath(ref List<Vector2> path) {

		List<Vector2> shortPath = new List<Vector2>();

		Vector2 prevPoint = path[0];
		int x=(int)path[0].x, y=(int)path[0].y;

		shortPath.Add(prevPoint);

		for(int i=1; i<path.Count; i++) {
			// if x||y is the same as the previous x||y then we can skip that point
			if(x!=(int)path[i].x && y!=(int)path[i].y)
			{	
				shortPath.Add(prevPoint);
				x = (int)prevPoint.x;
				y = (int)prevPoint.y;

				if(shortPath.Count>3) { // if we have more than 3 points we can start checking if we can remove triangle points
					Vector2 first = shortPath[shortPath.Count-1];
					Vector2 last = shortPath[shortPath.Count-3];
					if(first.x == last.x-1 && first.y == last.y-1 ||
					   first.x == last.x+1 && first.y == last.y+1 ||
					   first.x == last.x-1 && first.y == last.y+1 ||
					   first.x == last.x+1 && first.y == last.y-1) {
						shortPath.RemoveAt(shortPath.Count-2);
					}
				}
				if(shortPath.Count>3) {
					Vector2 first = shortPath[shortPath.Count-1];
					Vector2 middle = shortPath[shortPath.Count-2];
					Vector2 last = shortPath[shortPath.Count-3];

					if((first.x==middle.x+1&&middle.x+1==last.x+2 && first.y==middle.y+1&&middle.y+1==last.y+2) ||
					   (first.x==middle.x+1&&middle.x+1==last.x+2 && first.y==middle.y-1&&middle.y-1==last.y-2) ||
					   (first.x==middle.x-1&&middle.x-1==last.x-2 && first.y==middle.y+1&&middle.y+1==last.y+2) ||
					   (first.x==middle.x-1&&middle.x-1==last.x-2 && first.y==middle.y-1&&middle.y-1==last.y-2)) {
						shortPath.RemoveAt(shortPath.Count-2);
					}
				}
			}
			prevPoint = path[i];
		}

//		for(int i=1; i<shortPath.Count; i++) {
//			// if x||y is the same as the previous x||y then we can skip that point
//			if(x!=(int)path[i].x && y!=(int)path[i].y)
//			{	
//				shortPath.Add(prevPoint);
//				x = (int)prevPoint.x;
//				y = (int)prevPoint.y;
//			}
//			prevPoint = path[i];
//		}

		return shortPath;
	}

	List<List<Vector2> > getPaths(ref bool[,] b) {
		int w = b.GetLength(0); // width
		int h = b.GetLength(1); // height

		Vector2 startPoint = Vector2.zero;
		List<List<Vector2> > paths = new List<List<Vector2> >();

		bool[,] temp = b;//(bool[,]) b.Clone();

		while( findStartPoint(ref temp, ref startPoint) ) {
			List<Vector2> points = new List<Vector2>();

			// Get vertices from outline
			List<Vector2> path = getPath2(ref temp, ref points, startPoint);

			// remove points from temp
			foreach(Vector2 point in path) {
				temp[(int)point.x,(int)point.y] = false;
			}
			paths.Add ( simplifyPath( ref path ) );
//			paths.Add (  path ); //REMOVE

		}

		return paths;
	}
	
	// returns true if found a start point
	bool findStartPoint(ref bool[,] b, ref Vector2 startPoint) {
		int w = b.GetLength(0); // width
		int h = b.GetLength(1); // height

		for(int x= 0; x<w; x++){
			for(int y= 0; y<h; y++){
				if(b[x,y]) {
					startPoint = new Vector2(x,y);
					Debug.Log("StartPoint: "+ startPoint);
					return true;
				}
			}
		}
		return false; // Cannot find any start points.
	}

	List<Vector2> getPath2(ref bool[,] b, ref List<Vector2> prevPoints, Vector2 startPoint) {

		int[,] dirs = {{0,1},{1,0},{0,-1},{-1,0}};
		
		int w = b.GetLength(0); // width
		int h = b.GetLength(1); // height

		Vector2 currPoint= Vector2.zero, newPoint = Vector2.zero;
		bool isOpen = true; // Is the path closed?

		for(int z=0; z<dirs.GetLength(0); z++) {
			int i = (int)startPoint.x + dirs[z,0];
			int j = (int)startPoint.y + dirs[z,1];
			if(i<w && i>=0 && j<h && j>=0) {
				if(b[i,j]) {
					currPoint = new Vector2(i,j);
				}
			}
		}

		prevPoints.Add(startPoint);

		int count = 0;

		while(isOpen && count<500) {
			count++;

			Debug.Log(currPoint);

			prevPoints.Add(currPoint);
			
			// Check each direction around the start point and repeat for each new point
			for(int z=0; z<dirs.GetLength(0); z++) {
				int i = (int)currPoint.x + dirs[z,0];
				int j = (int)currPoint.y + dirs[z,1];
				if(i<w && i>=0 && j<h && j>=0) {
					if(b[i,j]) {
						if(!prevPoints.Contains(new Vector2(i,j))) {
							newPoint = new Vector2(i,j);
							break;
						} else {
							if(new Vector2(i,j)==startPoint) {
								isOpen = false;
								break;
							}
						}
					}
				}
			}

			if(!isOpen) continue;

			// Deadend
			if(newPoint==currPoint) {
				for(int p=prevPoints.Count-1; p>=0; p--) {
					for(int z=0; z<dirs.GetLength(0); z++) {
						int i = (int)prevPoints[p].x + dirs[z,0];
						int j = (int)prevPoints[p].y + dirs[z,1];
						if(i<w && i>=0 && j<h && j>=0) {
							if(b[i,j]) {
								if(!prevPoints.Contains(new Vector2(i,j))) {
									newPoint = new Vector2(i,j);
									break;
								}
							}
						}
					}
					if(newPoint!=currPoint) break;
				}
				Debug.Log("NEVER GETS PRINTED");
			}
			currPoint = newPoint;
		}
		Debug.Log("count<500?: "+count);
		return prevPoints;
	}

	// recursive function - its gonna explode
	// Single island vert mapping
	List<Vector2> getPath(ref bool[,] b, ref List<Vector2> prevPoints, Vector2 currPoint) {
		int[,] dirs = {{0,1},{1,0},{0,-1},{-1,0}};

		int w = b.GetLength(0); // width
		int h = b.GetLength(1); // height

		// get direction
		if(prevPoints.Count == 0) {
			prevPoints.Add(currPoint); // startpoint

			for(int z=0; z<dirs.GetLength(0); z++) {
				int i = (int)currPoint.x + dirs[z,0];
				int j = (int)currPoint.y + dirs[z,1];
				if(i<w && i>=0 && j<h && j>=0) {
					if(b[i,j]) {
						return getPath (ref b, ref prevPoints, new Vector2(i,j));
					}
				}
			}
			return prevPoints;
		}

		for(int z=0; z<dirs.GetLength(0); z++) {
			int i = (int)currPoint.x + dirs[z,0];
			int j = (int)currPoint.y + dirs[z,1];
			if(i<w && i>=0 && j<h && j>=0) {
				if(b[i,j]) { // if there is a point
					Vector2 point = new Vector2(i,j);
					if( prevPoints.Contains(point) ) {
						if(prevPoints[0] == point && prevPoints.Count>2) {
							prevPoints.Add (currPoint);
							return prevPoints;
						}
					} else {
						prevPoints.Add (currPoint);
						return getPath (ref b, ref prevPoints, point);
					}

//					if(!(i== prevPoints[prevPoints.Count-1].x && j== prevPoints[prevPoints.Count-1].y)) { // check its not the point we just added
//						if(i==prevPoints[0].x && j==prevPoints[0].y && prevPoints[0]!=prevPoints[prevPoints.Count-1]) { // Is it the start point?
//							prevPoints.Add (currPoint);
//							return prevPoints;
//						} else { // Add it and start looking for the next point
//							prevPoints.Add (currPoint);
//							return getPath (ref b, ref prevPoints, new Vector2(i,j));
//						}
//					}
				}
			}
		}

		// Deadend? backtrack to find another path to take
		for(int p=prevPoints.Count-1; p>=0; p--) {
			for(int z=0; z<dirs.GetLength(0); z++) {
				int i = (int)prevPoints[p].x + dirs[z,0];
				int j = (int)prevPoints[p].y + dirs[z,1];
				if(i<w && i>=0 && j<h && j>=0) {
					if(b[i,j]) {
						if(!prevPoints.Contains(new Vector2(i,j))) {
							return getPath (ref b, ref prevPoints, new Vector2(i,j));
						}
					}
				}
			}
		}

		foreach(Vector2 point in prevPoints) {
			for(int z=0; z<dirs.GetLength(0); z++) {
				int i = (int)point.x + dirs[z,0];
				int j = (int)point.y + dirs[z,1];
				if(i<w && i>=0 && j<h && j>=0) {
					if(b[i,j]) {
						if(!prevPoints.Contains(new Vector2(i,j))) {
							return getPath (ref b, ref prevPoints, new Vector2(i,j));
						}
					}
				}
			}
		}

		return prevPoints; // stupid c# all paths must return crap
	}

	bool[,] subtraction(bool[,] b1, bool[,] b2) {

		int w = b1.GetLength(0); // width
		int h = b1.GetLength(1); // height

		bool[,] temp = new bool[w,h];

		for(int x=0; x<w; x++){
			for(int y=0; y<h; y++){
				temp[x,y] = (b1[x,y]!=b2[x,y]);
			}
		}
		return temp;
	}

	// if there is any pixel in a 3x3 grid make the centre one black
	bool[,] erosion( ref bool[,] b) {
	
		int[,] dirs = {{0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1},{-1,0},{-1,1}};

		int w = b.GetLength(0); // width
		int h = b.GetLength(1);	// height

		bool[,] temp = new bool[w,h];
		
		for(int x=0; x<w; x++){
			for(int y=0; y<h; y++){
				temp[x,y] = true;
				for(int z=0; z<dirs.GetLength(0); z++) {
					int i = x+dirs[z,0];
					int j = y+dirs[z,1];
					if(i<w && i>=0 && j<h && j>=0) {
						if(!b[i,j]) temp[x,y] = false;
					}
					else temp[x,y] = false;
				}
			}
		}

		return temp;
	}

	bool[,] dilation( ref bool[,] b) {

		bool[,] temp = b; //(bool[,]) b.Clone();
		int[,] dirs = {{0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1},{-1,0},{-1,1}};
		
		int w = b.GetLength(0); // width
		int h = b.GetLength(1);	// height

		for(int x=0; x<w; x++){
			for(int y=0; y<h; y++){
				if(b[x,y]) {
					for(int z=0; z<dirs.GetLength(0); z++) {
						int i = x+dirs[z,0];
						int j = y+dirs[z,1];
						if(i<w && i>=0 && j<h && j>=0)
							temp[i,j] = true;
//						else temp[i,j] = false; // Should already be false when initialsslslslsed
					}
				}
			}
		}
		return temp;
	}

	void binaryImageFromTex(out bool[,] b, ref Texture2D t) {

		b = new bool[t.width,t.height];

		for(int x=0; x<t.width; x++){
			for(int y=0; y<t.height; y++){
				b[x,y] = (t.GetPixel(x,y).a > 0); // If alpha >0 true then 1 else 0
			}
		}
	}

	// test functions below
	public void texFromBinaryImage(out Texture2D t, bool[,] b) {

		t = new Texture2D(b.GetLength(0),b.GetLength(1));
		t.wrapMode = TextureWrapMode.Clamp;
		
		for(int x=0; x<t.width; x++){
			for(int y=0; y<t.height; y++){
				t.SetPixel(x,y, (b[x,y] ? Color.white : Color.black) ); // if true then white else black
			}
		}
		t.Apply();
	}

	void texFromBinaryImageUsingPaths(out Texture2D t, bool[,] b, List<List<Vector2> > paths) {

		List<Color> colorList = new List<Color>() {
			Color.red,
			Color.green,
			Color.blue,
			Color.magenta,
			Color.yellow
		};

		t = new Texture2D(b.GetLength(0),b.GetLength(1));
		t.wrapMode = TextureWrapMode.Clamp;

		for(int i=0; i<paths.Count&&i<colorList.Count; i++) {
			for(int j=0; j<paths[i].Count; j++) {
				t.SetPixel((int)paths[i][j].x, (int)paths[i][j].y, colorList[i]);
			}
		}
		t.Apply();

	}
	
}
