using UnityEngine;
using AccidentalNoise;
using System.Collections.Generic;
using Assets.Scripts.Units;

namespace Assets.Scripts.Map
{
    class HexWorldGenerator : Generator
    {
		protected ImplicitFractal HeightMap;
		protected ImplicitCombiner HeatMap;
		protected ImplicitFractal MoistureMap;

		public World _world;

		protected override void Initialize()
		{
			// HeightMap
			HeightMap = new ImplicitFractal(FractalType.MULTI,
											 BasisType.SIMPLEX,
											 InterpolationType.QUINTIC,
											 TerrainOctaves,
											 TerrainFrequency,
											 Seed);

			// Heat Map
			ImplicitGradient gradient = new ImplicitGradient(1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1);
			ImplicitFractal heatFractal = new ImplicitFractal(FractalType.MULTI,
															  BasisType.SIMPLEX,
															  InterpolationType.QUINTIC,
															  HeatOctaves,
															  HeatFrequency,
															  Seed);

			HeatMap = new ImplicitCombiner(CombinerType.MULTIPLY);
			HeatMap.AddSource(gradient);
			HeatMap.AddSource(heatFractal);

			// Moisture Map
			MoistureMap = new ImplicitFractal(FractalType.MULTI,
											   BasisType.SIMPLEX,
											   InterpolationType.QUINTIC,
											   MoistureOctaves,
											   MoistureFrequency,
											   Seed);
		}

		protected override void GetData()
		{
			HeightData = new MapData(Width, Height);
			HeatData = new MapData(Width, Height);
			MoistureData = new MapData(Width, Height);

			// loop through each x,y point - get height value
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{

					// WRAP ON BOTH AXIS
					// Noise range
					float x1 = 0, x2 = 2;
					float y1 = 0, y2 = 2;
					float dx = x2 - x1;
					float dy = y2 - y1;

					// Sample noise at smaller intervals
					float s = x / (float)Width;
					float t = y / (float)Height;

					// Calculate our 4D coordinates
					float nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
					float ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);
					float nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
					float nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);

					float heightValue = (float)HeightMap.Get(nx, ny, nz, nw);
					float heatValue = (float)HeatMap.Get(nx, ny, nz, nw);
					float moistureValue = (float)MoistureMap.Get(nx, ny, nz, nw);

					// keep track of the max and min values found
					if (heightValue > HeightData.Max) HeightData.Max = heightValue;
					if (heightValue < HeightData.Min) HeightData.Min = heightValue;

					if (heatValue > HeatData.Max) HeatData.Max = heatValue;
					if (heatValue < HeatData.Min) HeatData.Min = heatValue;

					if (moistureValue > MoistureData.Max) MoistureData.Max = moistureValue;
					if (moistureValue < MoistureData.Min) MoistureData.Min = moistureValue;

					HeightData.Data[x, y] = heightValue;
					HeatData.Data[x, y] = heatValue;
					MoistureData.Data[x, y] = moistureValue;
				}
			}
		}

		protected override Cell GetTop(Cell t)
		{
			return this._world.hexes[t.X, MathHelper.Mod(t.Y - 1, Height)];
		}
		protected override Cell GetBottom(Cell t)
		{
			return this._world.hexes[t.X, MathHelper.Mod(t.Y + 1, Height)];
		}
		protected override Cell GetLeft(Cell t)
		{
			return this._world.hexes[MathHelper.Mod(t.X - 1, Width), t.Y];
		}
		protected override Cell GetRight(Cell t)
		{
			return this._world.hexes[MathHelper.Mod(t.X + 1, Width), t.Y];
		}

		protected override void UpdateBiomeBitmask()
		{
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					this._world.hexes[x, y].UpdateBiomeBitmask();
				}
			}
		}

		private void Start()
		{
			//Debug.Log(100);
			//Seed = UnityEngine.Random.Range(0, int.MaxValue);
			Seed = 100;
			Initialize();
			this._world = GetComponent<World>();
			this._world.createHexesRect();
			Generate();
		}

		protected override void GenerateBiomeMap()
		{
			for (var x = 0; x < this.Width; x++)
			{
				for (var y = 0; y < this.Height; y++)
				{

					if (!_world.hexes[x, y].Collidable) continue;

					Hex t = _world.hexes[x, y];
					t.BiomeType = GetBiomeType(t);
				}
			}
		}

		protected override void Generate()
		{
			GetData();
			LoadCells();




			//UpdateNeighbors();

			//GenerateRivers();
			//BuildRiverGroups();
			//DigRiverGroups();
			//AdjustMoistureMap();

			UpdateBitmasks();
			FloodFill();

			GenerateBiomeMap();
			UpdateBiomeBitmask();
			
			new Pop(2, new Hex(new Vector2Int(0, 5)), this._world);
		}

		protected override void FloodFill()
		{
			// Use a stack instead of recursion
			Stack<Cell> stack = new Stack<Cell>();

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{

					Hex t = this._world.hexes[x, y];

					//Cell already flood filled, skip
					if (t.FloodFilled) continue;

					// Land
					if (t.Collidable)
					{
						CellGroup group = new CellGroup();
						group.Type = CellGroupType.Land;
						stack.Push(t);

						while (stack.Count > 0)
						{
							FloodFill(stack.Pop(), ref group, ref stack);
						}

						if (group.Cells.Count > 0)
							Lands.Add(group);
					}
					// Water
					else
					{
						CellGroup group = new CellGroup();
						group.Type = CellGroupType.Water;
						stack.Push(t);

						while (stack.Count > 0)
						{
							FloodFill(stack.Pop(), ref group, ref stack);
						}

						if (group.Cells.Count > 0)
							Waters.Add(group);
					}
				}
			}
		}

		protected override void FloodFill(Cell cell, ref CellGroup cells, ref Stack<Cell> stack)
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
			cells.Cells.Add(cell);
			cell.FloodFilled = true;

			// floodfill into neighbors
			Hex t = (Hex)GetTop(cell);
			if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
				stack.Push(t);
			t = (Hex)GetBottom(cell);
			if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
				stack.Push(t);
			t = (Hex)GetLeft(cell);
			if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
				stack.Push(t);
			t = (Hex)GetRight(cell);
			if (t != null && !t.FloodFilled && cell.Collidable == t.Collidable)
				stack.Push(t);
		}

		protected override void UpdateBitmasks()
		{
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					this._world.hexes[x, y].UpdateBitmask();
				}
			}
		}

		public override void LoadCells()
        {
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					Hex t = new Hex(new Vector2Int(x, y));

					//set heightmap value
					float heightValue = HeightData.Data[x, y];
					heightValue = (heightValue - HeightData.Min) / (HeightData.Max - HeightData.Min);
					t.HeightValue = heightValue;


					if (heightValue < DeepWater)
					{
						t.HeightType = HeightType.DeepWater;
						t.Collidable = false;
					}
					else if (heightValue < ShallowWater)
					{
						t.HeightType = HeightType.ShallowWater;
						t.Collidable = false;
					}
					else if (heightValue < Sand)
					{
						t.HeightType = HeightType.Sand;
						t.Collidable = true;
					}
					else if (heightValue < Grass)
					{
						t.HeightType = HeightType.Grass;
						t.Collidable = true;
					}
					else if (heightValue < Forest)
					{
						t.HeightType = HeightType.Forest;
						t.Collidable = true;
					}
					else if (heightValue < Rock)
					{
						t.HeightType = HeightType.Rock;
						t.Collidable = true;
					}
					else
					{
						t.HeightType = HeightType.Snow;
						t.Collidable = true;
					}


					//adjust moisture based on height
					if (t.HeightType == HeightType.DeepWater)
					{
						MoistureData.Data[t.X, t.Y] += 8f * t.HeightValue;
					}
					else if (t.HeightType == HeightType.ShallowWater)
					{
						MoistureData.Data[t.X, t.Y] += 3f * t.HeightValue;
					}
					else if (t.HeightType == HeightType.Shore)
					{
						MoistureData.Data[t.X, t.Y] += 1f * t.HeightValue;
					}
					else if (t.HeightType == HeightType.Sand)
					{
						MoistureData.Data[t.X, t.Y] += 0.2f * t.HeightValue;
					}

					//Moisture Map Analyze	
					float moistureValue = MoistureData.Data[x, y];
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
					if (t.HeightType == HeightType.Forest)
					{
						HeatData.Data[t.X, t.Y] -= 0.1f * t.HeightValue;
					}
					else if (t.HeightType == HeightType.Rock)
					{
						HeatData.Data[t.X, t.Y] -= 0.25f * t.HeightValue;
					}
					else if (t.HeightType == HeightType.Snow)
					{
						HeatData.Data[t.X, t.Y] -= 0.4f * t.HeightValue;
					}
					else
					{
						HeatData.Data[t.X, t.Y] += 0.01f * t.HeightValue;
					}

					// Set heat value
					float heatValue = HeatData.Data[x, y];
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

					//t.HexType = HexType.Ocean;
					t.HexType = HexType.GetHexType(t.HeightType, t.MoistureType, t.HeatType);
					//Debug.Log(t.HeightType);
					_world.AddHex(t, x, y);
					//Debug.Log(t.MoistureValue);
					//Debug.Log(t.Position);
				}
			}
		}
    }
}
