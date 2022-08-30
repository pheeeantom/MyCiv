using UnityEngine;
using System.Collections.Generic;

public abstract class Generator : MonoBehaviour {

	protected int Seed;

	// Adjustable variables for Unity Inspector
	[Header("Generator Values")]
	[SerializeField]
	public int Width = 512;
	[SerializeField]
	public int Height = 512;

	[Header("Height Map")]
	[SerializeField]
	protected int TerrainOctaves = 6;
	[SerializeField]
	protected double TerrainFrequency = 1.25;
	[SerializeField]
	public float DeepWater = 0.2f;
	[SerializeField]
	public float ShallowWater = 0.4f;	
	[SerializeField]
	public float Sand = 0.5f;
	[SerializeField]
	public float Grass = 0.7f;
	[SerializeField]
	public float Forest = 0.8f;
	[SerializeField]
	public float Rock = 0.9f;

	[Header("Heat Map")]
	[SerializeField]
	protected int HeatOctaves = 4;
	[SerializeField]
	protected double HeatFrequency = 3.0;
	[SerializeField]
	public float ColdestValue = 0.05f;
	[SerializeField]
	public float ColderValue = 0.18f;
	[SerializeField]
	public float ColdValue = 0.4f;
	[SerializeField]
	public float WarmValue = 0.6f;
	[SerializeField]
	public float WarmerValue = 0.8f;

	[Header("Moisture Map")]
	[SerializeField]
	protected int MoistureOctaves = 4;
	[SerializeField]
	protected double MoistureFrequency = 3.0;
	[SerializeField]
	public float DryerValue = 0.27f;
	[SerializeField]
	public float DryValue = 0.4f;
	[SerializeField]
	public float WetValue = 0.6f;
	[SerializeField]
	public float WetterValue = 0.8f;
	[SerializeField]
	public float WettestValue = 0.9f;

	[Header("Rivers")]
	[SerializeField]
	protected int RiverCount = 40;
	[SerializeField]
	protected float MinRiverHeight = 0.6f;
	[SerializeField]
	protected int MaxRiverAttempts = 1000;
	[SerializeField]
	protected int MinRiverTurns = 18;
	[SerializeField]
	protected int MinRiverLength = 20;
	[SerializeField]
	protected int MaxRiverIntersections = 2;

	public MapData HeightData;
	public MapData HeatData;
	public MapData MoistureData;
	protected MapData Clouds1;
    protected MapData Clouds2;

    protected Cell[,] Cells;

	protected List<CellGroup> Waters = new List<CellGroup> ();
	protected List<CellGroup> Lands = new List<CellGroup> ();

	protected List<River> Rivers = new List<River>();	
	protected List<RiverGroup> RiverGroups = new List<RiverGroup>();
		
	// Our texture output gameobject
	protected MeshRenderer HeightMapRenderer;
	protected MeshRenderer HeatMapRenderer;
	protected MeshRenderer MoistureMapRenderer;
	protected MeshRenderer BiomeMapRenderer;

	protected BiomeType[,] BiomeTable = new BiomeType[6,6] {   
		//COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
	};

	void Start()
	{
		Instantiate ();
		Generate ();
	}
    
    protected abstract void Initialize();
    protected abstract void GetData();

    protected abstract Cell GetTop(Cell cell);
    protected abstract Cell GetBottom(Cell cell);
    protected abstract Cell GetLeft(Cell cell);
    protected abstract Cell GetRight(Cell cell);

    protected virtual void Instantiate () {

		Seed = UnityEngine.Random.Range (0, int.MaxValue);
		
		HeightMapRenderer = transform.Find ("HeightTexture").GetComponent<MeshRenderer> ();
		HeatMapRenderer = transform.Find ("HeatTexture").GetComponent<MeshRenderer> ();
		MoistureMapRenderer = transform.Find ("MoistureTexture").GetComponent<MeshRenderer> ();
		BiomeMapRenderer = transform.Find ("BiomeTexture").GetComponent<MeshRenderer> ();

		Initialize ();
	}

	protected virtual void Generate()
	{		
		GetData ();
		LoadCells ();		
		UpdateNeighbors ();
		
		GenerateRivers ();
		BuildRiverGroups ();
		DigRiverGroups ();
		AdjustMoistureMap ();
		
		UpdateBitmasks ();
		FloodFill ();
		
		GenerateBiomeMap ();
		UpdateBiomeBitmask();
		
		HeightMapRenderer.materials [0].mainTexture = TextureGenerator.GetHeightMapTexture (Width, Height, Cells);
		HeatMapRenderer.materials[0].mainTexture = TextureGenerator.GetHeatMapTexture (Width, Height, Cells);
		MoistureMapRenderer.materials[0].mainTexture = TextureGenerator.GetMoistureMapTexture (Width, Height, Cells);
		BiomeMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture (Width, Height, Cells, ColdestValue, ColderValue, ColdValue);
	}

	void Update()
	{
        // Refresh with inspector values
		if (Input.GetKeyDown (KeyCode.F5)) {
            Seed = UnityEngine.Random.Range(0, int.MaxValue);
            Initialize();
            Generate();
		}
	}

	protected virtual void UpdateBiomeBitmask()
	{
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				Cells [x, y].UpdateBiomeBitmask ();
			}
		}
	}

	public BiomeType GetBiomeType(Cell cell)
	{
		return BiomeTable [(int)cell.MoistureType, (int)cell.HeatType];
	}

	protected virtual void GenerateBiomeMap()
	{
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				
				if (!Cells[x, y].Collidable) continue;
				
				Cell t = Cells[x,y];
				t.BiomeType = GetBiomeType(t);
			}
		}
	}

	private void AddMoisture(Cell t, int radius)
	{
		int startx = MathHelper.Mod (t.X - radius, Width);
		int endx = MathHelper.Mod (t.X + radius, Width);
		Vector2 center = new Vector2(t.X, t.Y);
		int curr = radius;

		while (curr > 0) {

			int x1 = MathHelper.Mod (t.X - curr, Width);
			int x2 = MathHelper.Mod (t.X + curr, Width);
			int y = t.Y;

			AddMoisture(Cells[x1, y], 0.025f / (center - new Vector2(x1, y)).magnitude);

			for (int i = 0; i < curr; i++)
			{
				AddMoisture (Cells[x1, MathHelper.Mod (y + i + 1, Height)], 0.025f / (center - new Vector2(x1, MathHelper.Mod (y + i + 1, Height))).magnitude);
				AddMoisture (Cells[x1, MathHelper.Mod (y - (i + 1), Height)], 0.025f / (center - new Vector2(x1, MathHelper.Mod (y - (i + 1), Height))).magnitude);

				AddMoisture (Cells[x2, MathHelper.Mod (y + i + 1, Height)], 0.025f / (center - new Vector2(x2, MathHelper.Mod (y + i + 1, Height))).magnitude);
				AddMoisture (Cells[x2, MathHelper.Mod (y - (i + 1), Height)], 0.025f / (center - new Vector2(x2, MathHelper.Mod (y - (i + 1), Height))).magnitude);
			}
			curr--;
		}
	}

	private void AddMoisture(Cell t, float amount)
	{
		MoistureData.Data[t.X, t.Y] += amount;
		t.MoistureValue += amount;
		if (t.MoistureValue > 1)
			t.MoistureValue = 1;
				
		//set moisture type
		if (t.MoistureValue < DryerValue) t.MoistureType = MoistureType.Dryest;
		else if (t.MoistureValue < DryValue) t.MoistureType = MoistureType.Dryer;
		else if (t.MoistureValue < WetValue) t.MoistureType = MoistureType.Dry;
		else if (t.MoistureValue < WetterValue) t.MoistureType = MoistureType.Wet;
		else if (t.MoistureValue < WettestValue) t.MoistureType = MoistureType.Wetter;
		else t.MoistureType = MoistureType.Wettest;
	}

	protected void AdjustMoistureMap()
	{
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {

				Cell t = Cells[x,y];
				if (t.HeightType == HeightType.River)
				{
					AddMoisture (t, (int)60);
				}
			}
		}
	}

	protected void DigRiverGroups()
	{
		for (int i = 0; i < RiverGroups.Count; i++) {

			RiverGroup group = RiverGroups[i];
			River longest = null;

			//Find longest river in this group
			for (int j = 0; j < group.Rivers.Count; j++)
			{
				River river = group.Rivers[j];
				if (longest == null)
					longest = river;
				else if (longest.Cells.Count < river.Cells.Count)
					longest = river;
			}

			if (longest != null)
			{				
				//Dig out longest path first
				DigRiver (longest);

				for (int j = 0; j < group.Rivers.Count; j++)
				{
					River river = group.Rivers[j];
					if (river != longest)
					{
						DigRiver (river, longest);
					}
				}
			}
		}
	}

	protected void BuildRiverGroups()
	{
		//loop each cell, checking if it belongs to multiple rivers
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				Cell t = Cells[x,y];

				if (t.Rivers.Count > 1)
				{
					// multiple rivers == intersection
					RiverGroup group = null;

					// Does a rivergroup already exist for this group?
					for (int n=0; n < t.Rivers.Count; n++)
					{
						River cellriver = t.Rivers[n];
						for (int i = 0; i < RiverGroups.Count; i++)
						{
							for (int j = 0; j < RiverGroups[i].Rivers.Count; j++)
							{
								River river = RiverGroups[i].Rivers[j];
								if (river.ID == cellriver.ID)
								{
									group = RiverGroups[i];
								}
								if (group != null) break;
							}
							if (group != null) break;
						}
						if (group != null) break;
					}

					// existing group found -- add to it
					if (group != null)
					{
						for (int n=0; n < t.Rivers.Count; n++)
						{
							if (!group.Rivers.Contains(t.Rivers[n]))
								group.Rivers.Add(t.Rivers[n]);
						}
					}
					else   //No existing group found - create a new one
					{
						group = new RiverGroup();
						for (int n=0; n < t.Rivers.Count; n++)
						{
							group.Rivers.Add(t.Rivers[n]);
						}
						RiverGroups.Add (group);
					}
				}
			}
		}	
	}

	public float GetHeightValue(Cell cell)
	{
		if (cell == null)
			return int.MaxValue;
		else
			return cell.HeightValue;
	}

	protected void GenerateRivers()
	{
		int attempts = 0;
		int rivercount = RiverCount;
		Rivers = new List<River> ();

		// Generate some rivers
		while (rivercount > 0 && attempts < MaxRiverAttempts) {

			// Get a random cell
			int x = UnityEngine.Random.Range (0, Width);
			int y = UnityEngine.Random.Range (0, Height);			
			Cell cell = Cells[x,y];

			// validate the cell
			if (!cell.Collidable) continue;
			if (cell.Rivers.Count > 0) continue;

			if (cell.HeightValue > MinRiverHeight)
			{				
				// Cell is good to start river from
				River river = new River(rivercount);

				// Figure out the direction this river will try to flow
				river.CurrentDirection = cell.GetLowestNeighbor (this);

				// Recursively find a path to water
				FindPathToWater(cell, river.CurrentDirection, ref river);

				// Validate the generated river 
				if (river.TurnCount < MinRiverTurns || river.Cells.Count < MinRiverLength || river.Intersections > MaxRiverIntersections)
				{
					//Validation failed - remove this river
					for (int i = 0; i < river.Cells.Count; i++)
					{
						Cell t = river.Cells[i];
						t.Rivers.Remove (river);
					}
				}
				else if (river.Cells.Count >= MinRiverLength)
				{
					//Validation passed - Add river to list
					Rivers.Add (river);
					cell.Rivers.Add (river);
					rivercount--;	
				}
			}		
			attempts++;
		}
	}

	// Dig river based on a parent river vein
	private void DigRiver(River river, River parent)
	{
		int intersectionID = 0;
		int intersectionSize = 0;

		// determine point of intersection
		for (int i = 0; i < river.Cells.Count; i++) {
			Cell t1 = river.Cells[i];
			for (int j = 0; j < parent.Cells.Count; j++) {
				Cell t2 = parent.Cells[j];
				if (t1 == t2)
				{
					intersectionID = i;
					intersectionSize = t2.RiverSize;
				}
			}
		}

		int counter = 0;
		int intersectionCount = river.Cells.Count - intersectionID;
		int size = UnityEngine.Random.Range(intersectionSize, 5);
		river.Length = river.Cells.Count;  

		// randomize size change
		int two = river.Length / 2;
		int three = two / 2;
		int four = three / 2;
		int five = four / 2;
		
		int twomin = two / 3;
		int threemin = three / 3;
		int fourmin = four / 3;
		int fivemin = five / 3;
		
		// randomize length of each size
		int count1 = UnityEngine.Random.Range (fivemin, five);  
		if (size < 4) {
			count1 = 0;
		}
		int count2 = count1 + UnityEngine.Random.Range(fourmin, four);  
		if (size < 3) {
			count2 = 0;
			count1 = 0;
		}
		int count3 = count2 + UnityEngine.Random.Range(threemin, three); 
		if (size < 2) {
			count3 = 0;
			count2 = 0;
			count1 = 0;
		}
		int count4 = count3 + UnityEngine.Random.Range (twomin, two); 

		// Make sure we are not digging past the river path
		if (count4 > river.Length) {
			int extra = count4 - river.Length;
			while (extra > 0)
			{
				if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
				else if (count2 > 0) { count2--; count3--; count4--; extra--; }
				else if (count3 > 0) { count3--; count4--; extra--; }
				else if (count4 > 0) { count4--; extra--; }
			}
		}
				
		// adjust size of river at intersection point
		if (intersectionSize == 1) {
			count4 = intersectionCount;
			count1 = 0;
			count2 = 0;
			count3 = 0;
		} else if (intersectionSize == 2) {
			count3 = intersectionCount;		
			count1 = 0;
			count2 = 0;
		} else if (intersectionSize == 3) {
			count2 = intersectionCount;
			count1 = 0;
		} else if (intersectionSize == 4) {
			count1 = intersectionCount;
		} else {
			count1 = 0;
			count2 = 0;
			count3 = 0;
			count4 = 0;
		}

		// dig out the river
		for (int i = river.Cells.Count - 1; i >= 0; i--) {

			Cell t = river.Cells [i];

			if (counter < count1) {
				t.DigRiver (river, 4);				
			} else if (counter < count2) {
				t.DigRiver (river, 3);				
			} else if (counter < count3) {
				t.DigRiver (river, 2);				
			} 
			else if ( counter < count4) {
				t.DigRiver (river, 1);
			}
			else {
				t.DigRiver (river, 0);
			}			
			counter++;			
		}
	}

	// Dig river
	private void DigRiver(River river)
	{
		int counter = 0;
		
		// How wide are we digging this river?
		int size = UnityEngine.Random.Range(1,5);
		river.Length = river.Cells.Count;  

		// randomize size change
		int two = river.Length / 2;
		int three = two / 2;
		int four = three / 2;
		int five = four / 2;
		
		int twomin = two / 3;
		int threemin = three / 3;
		int fourmin = four / 3;
		int fivemin = five / 3;

		// randomize lenght of each size
		int count1 = UnityEngine.Random.Range (fivemin, five);             
		if (size < 4) {
			count1 = 0;
		}
		int count2 = count1 + UnityEngine.Random.Range(fourmin, four); 
		if (size < 3) {
			count2 = 0;
			count1 = 0;
		}
		int count3 = count2 + UnityEngine.Random.Range(threemin, three); 
		if (size < 2) {
			count3 = 0;
			count2 = 0;
			count1 = 0;
		}
		int count4 = count3 + UnityEngine.Random.Range (twomin, two);  
		
		// Make sure we are not digging past the river path
		if (count4 > river.Length) {
			int extra = count4 - river.Length;
			while (extra > 0)
			{
				if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
				else if (count2 > 0) { count2--; count3--; count4--; extra--; }
				else if (count3 > 0) { count3--; count4--; extra--; }
				else if (count4 > 0) { count4--; extra--; }
			}
		}

		// Dig it out
		for (int i = river.Cells.Count - 1; i >= 0 ; i--)
		{
			Cell t = river.Cells[i];

			if (counter < count1) {
				t.DigRiver (river, 4);				
			}
			else if (counter < count2) {
				t.DigRiver (river, 3);				
			} 
			else if (counter < count3) {
				t.DigRiver (river, 2);				
			} 
			else if ( counter < count4) {
				t.DigRiver (river, 1);
			}
			else {
				t.DigRiver(river, 0);
			}			
			counter++;			
		}
	}
	
	private void FindPathToWater(Cell cell, Direction direction, ref River river)
	{
		if (cell.Rivers.Contains (river))
			return;

		// check if there is already a river on this cell
		if (cell.Rivers.Count > 0)
			river.Intersections++;

		river.AddCell (cell);

		// get neighbors
		Cell left = GetLeft (cell);
		Cell right = GetRight (cell);
		Cell top = GetTop (cell);
		Cell bottom = GetBottom (cell);
		
		float leftValue = int.MaxValue;
		float rightValue = int.MaxValue;
		float topValue = int.MaxValue;
		float bottomValue = int.MaxValue;
		
		// query height values of neighbors
		if (left != null && left.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(left)) 
			leftValue = left.HeightValue;
		if (right != null && right.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(right)) 
			rightValue = right.HeightValue;
		if (top != null && top.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(top)) 
			topValue = top.HeightValue;
		if (bottom != null && bottom.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(bottom)) 
			bottomValue = bottom.HeightValue;
		
		// if neighbor is existing river that is not this one, flow into it
		if (bottom != null && bottom.Rivers.Count == 0 && !bottom.Collidable)
			bottomValue = 0;
		if (top != null && top.Rivers.Count == 0 && !top.Collidable)
			topValue = 0;
		if (left != null && left.Rivers.Count == 0 && !left.Collidable)
			leftValue = 0;
		if (right != null && right.Rivers.Count == 0 && !right.Collidable)
			rightValue = 0;

		// override flow direction if a cell is significantly lower
		if (direction == Direction.Left)
			if (Mathf.Abs (rightValue - leftValue) < 0.1f)
				rightValue = int.MaxValue;
		if (direction == Direction.Right)
			if (Mathf.Abs (rightValue - leftValue) < 0.1f)
				leftValue = int.MaxValue;
		if (direction == Direction.Top)
			if (Mathf.Abs (topValue - bottomValue) < 0.1f)
				bottomValue = int.MaxValue;
		if (direction == Direction.Bottom)
			if (Mathf.Abs (topValue - bottomValue) < 0.1f)
				topValue = int.MaxValue;
		
		// find mininum
		float min = Mathf.Min (Mathf.Min (Mathf.Min (leftValue, rightValue), topValue), bottomValue);
		
		// if no minimum found - exit
		if (min == int.MaxValue)
			return;
		
		//Move to next neighbor
		if (min == leftValue) {
			if (left != null && left.Collidable)
			{
				if (river.CurrentDirection != Direction.Left){
					river.TurnCount++;
					river.CurrentDirection = Direction.Left;
				}
				FindPathToWater (left, direction, ref river);
			}
		} else if (min == rightValue) {
			if (right != null && right.Collidable)
			{
				if (river.CurrentDirection != Direction.Right){
					river.TurnCount++;
					river.CurrentDirection = Direction.Right;
				}
				FindPathToWater (right, direction, ref river);
			}
		} else if (min == bottomValue) {
			if (bottom != null && bottom.Collidable)
			{
				if (river.CurrentDirection != Direction.Bottom){
					river.TurnCount++;
					river.CurrentDirection = Direction.Bottom;
				}
				FindPathToWater (bottom, direction, ref river);
			}
		} else if (min == topValue) {
			if (top != null && top.Collidable)
			{
				if (river.CurrentDirection != Direction.Top){
					river.TurnCount++;
					river.CurrentDirection = Direction.Top;
				}
				FindPathToWater (top, direction, ref river);
			}
		}
	}

	// Build a Cell array from our data
	public virtual void LoadCells()
	{
		Cells = new Cell[Width, Height];
		
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				Cell t = new Cell();
				t.X = x;
				t.Y = y;

				//set heightmap value
				float heightValue = HeightData.Data[x, y];
				heightValue = (heightValue - HeightData.Min) / (HeightData.Max - HeightData.Min);
				t.HeightValue = heightValue;
					

				if (heightValue < DeepWater)  {
					t.HeightType = HeightType.DeepWater;
					t.Collidable = false;
				}
				else if (heightValue < ShallowWater)  {
					t.HeightType = HeightType.ShallowWater;
					t.Collidable = false;
				}
				else if (heightValue < Sand) {
					t.HeightType = HeightType.Sand;
					t.Collidable = true;
				}
				else if (heightValue < Grass) {
					t.HeightType = HeightType.Grass;
					t.Collidable = true;
				}
				else if (heightValue < Forest) {
					t.HeightType = HeightType.Forest;
					t.Collidable = true;
				}
				else if (heightValue < Rock) {
					t.HeightType = HeightType.Rock;
					t.Collidable = true;
				}
				else  {
					t.HeightType = HeightType.Snow;
					t.Collidable = true;
				}


				//adjust moisture based on height
				if (t.HeightType == HeightType.DeepWater) {
					MoistureData.Data[t.X, t.Y] += 8f * t.HeightValue;
				}
				else if (t.HeightType == HeightType.ShallowWater) {
					MoistureData.Data[t.X, t.Y] += 3f * t.HeightValue;
				}
				else if (t.HeightType == HeightType.Shore) {
					MoistureData.Data[t.X, t.Y] += 1f * t.HeightValue;
				}
				else if (t.HeightType == HeightType.Sand) {
					MoistureData.Data[t.X, t.Y] += 0.2f * t.HeightValue;
				}				
				
				//Moisture Map Analyze	
				float moistureValue = MoistureData.Data[x,y];
				moistureValue = (moistureValue - MoistureData.Min) / (MoistureData.Max - MoistureData.Min);
				t.MoistureValue = moistureValue;
				
				//set moisture type
				if (moistureValue < DryerValue) t.MoistureType = MoistureType.Dryest;
				else if (moistureValue < DryValue) t.MoistureType = MoistureType.Dryer;
				else if (moistureValue < WetValue) t.MoistureType = MoistureType.Dry;
				else if (moistureValue < WetterValue) t.MoistureType = MoistureType.Wet;
				else if (moistureValue < WettestValue) t.MoistureType = MoistureType.Wetter;
				else t.MoistureType = MoistureType.Wettest;


				// Adjust Heat Map based on Height - Higher == colder
				if (t.HeightType == HeightType.Forest) {
					HeatData.Data[t.X, t.Y] -= 0.1f * t.HeightValue;
				}
				else if (t.HeightType == HeightType.Rock) {
					HeatData.Data[t.X, t.Y] -= 0.25f * t.HeightValue;
				}
				else if (t.HeightType == HeightType.Snow) {
					HeatData.Data[t.X, t.Y] -= 0.4f * t.HeightValue;
				}
				else {
					HeatData.Data[t.X, t.Y] += 0.01f * t.HeightValue;
				}

				// Set heat value
				float heatValue = HeatData.Data[x,y];
				heatValue = (heatValue - HeatData.Min) / (HeatData.Max - HeatData.Min);
				t.HeatValue = heatValue;

				// set heat type
				if (heatValue < ColdestValue) t.HeatType = HeatType.Coldest;
				else if (heatValue < ColderValue) t.HeatType = HeatType.Colder;
				else if (heatValue < ColdValue) t.HeatType = HeatType.Cold;
				else if (heatValue < WarmValue) t.HeatType = HeatType.Warm;
				else if (heatValue < WarmerValue) t.HeatType = HeatType.Warmer;
				else t.HeatType = HeatType.Warmest;

                if (Clouds1 != null)
                {
                    t.Cloud1Value = Clouds1.Data[x, y];
                    t.Cloud1Value = (t.Cloud1Value - Clouds1.Min) / (Clouds1.Max - Clouds1.Min);
                }

                if (Clouds2 != null)
                {
                    t.Cloud2Value = Clouds2.Data[x, y];
                    t.Cloud2Value = (t.Cloud2Value - Clouds2.Min) / (Clouds2.Max - Clouds2.Min);
                }

                Cells[x,y] = t;
			}
		}
	}
	
	protected void UpdateNeighbors()
	{
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				Cell t = Cells[x,y];
				
				t.Top = GetTop(t);
				t.Bottom = GetBottom (t);
				t.Left = GetLeft (t);
				t.Right = GetRight (t);
			}
		}
	}

	protected virtual void UpdateBitmasks()
	{
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				Cells [x, y].UpdateBitmask ();
			}
		}
	}

	protected virtual void FloodFill()
	{
		// Use a stack instead of recursion
		Stack<Cell> stack = new Stack<Cell>();
		
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				
				Cell t = Cells[x,y];

				//Cell already flood filled, skip
				if (t.FloodFilled) continue;

				// Land
				if (t.Collidable)   
				{
					CellGroup group = new CellGroup();
					group.Type = CellGroupType.Land;
					stack.Push(t);
					
					while(stack.Count > 0) {
						FloodFill(stack.Pop(), ref group, ref stack);
					}
					
					if (group.Cells.Count > 0)
						Lands.Add (group);
				}
				// Water
				else {				
					CellGroup group = new CellGroup();
					group.Type = CellGroupType.Water;
					stack.Push(t);
					
					while(stack.Count > 0)	{
						FloodFill(stack.Pop(), ref group, ref stack);
					}
					
					if (group.Cells.Count > 0)
						Waters.Add (group);
				}
			}
		}
	}

	protected virtual void FloodFill(Cell cell, ref CellGroup cells, ref Stack<Cell> stack)
	{
		// Validate
		if (cell == null)
			return;
		if (cell.FloodFilled) 
			return;
		if (cells.Type == CellGroupType.Land && !cell.Collidable)
			return;
		if (cells.Type == CellGroupType.Water && cell.Collidable)
			return;

		// Add to CellGroup
		cells.Cells.Add (cell);
		cell.FloodFilled = true;

		// floodfill into neighbors
		Cell t = GetTop (cell);
		if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
			stack.Push (t);
		t = GetBottom (cell);
		if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
			stack.Push (t);
		t = GetLeft (cell);
		if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
			stack.Push (t);
		t = GetRight (cell);
		if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
			stack.Push (t);
	}
    
}
