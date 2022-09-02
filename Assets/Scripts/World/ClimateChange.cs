using UnityEngine;

namespace World
{
    class ClimateChange : MonoBehaviour
    {
        [SerializeField] protected World _world;
        [SerializeField] protected HexWorldGenerator _generator;
        float _heightChange = 0;
        float _heatChange = 0;
        float _moistureChange = 0;

        void GlobalWarming(float height, float heat, float moisture)
        {
			Debug.Log("helo");
			for (int x = 0; x < this._generator.Width; x++)
			{
				for (int y = 0; y < this._generator.Height; y++)
				{
					//Debug.Log(this._generator.HeightData.Data[x, y]);
					this._generator.HeightData.Data[x, y] += height;
					this._generator.HeatData.Data[x, y] += heat;
					this._generator.MoistureData.Data[x, y] += moisture;
					//Debug.Log(this._generator.HeightData.Data[x, y]);
				}
			}
			for (int x = 0; x < this._generator.Width; x++)
			{
				for (int y = 0; y < this._generator.Height; y++)
				{
					Hex t = this._world.HexMatrix[x, y];
					t.X = x;
					t.Y = y;

					//set heightmap value
					//Debug.Log(t.HeightValue);
					float heightValue = this._generator.HeightData.Data[x, y];
					heightValue = (heightValue - this._generator.HeightData.Min) / (this._generator.HeightData.Max - this._generator.HeightData.Min);
					t.HeightValue = heightValue;


					if (heightValue < this._generator.DeepWater)
					{
						t.HeightType = HeightType.DeepWater;
						t.Collidable = false;
					}
					else if (heightValue < this._generator.ShallowWater)
					{
						t.HeightType = HeightType.ShallowWater;
						t.Collidable = false;
					}
					else if (heightValue < this._generator.Sand)
					{
						t.HeightType = HeightType.Sand;
						t.Collidable = true;
					}
					else if (heightValue < this._generator.Grass)
					{
						t.HeightType = HeightType.Grass;
						t.Collidable = true;
					}
					else if (heightValue < this._generator.Forest)
					{
						t.HeightType = HeightType.Forest;
						t.Collidable = true;
					}
					else if (heightValue < this._generator.Rock)
					{
						t.HeightType = HeightType.Rock;
						t.Collidable = true;
					}
					else
					{
						t.HeightType = HeightType.Snow;
						t.Collidable = true;
					}

					//Moisture Map Analyze	
					float moistureValue = this._generator.MoistureData.Data[x, y];
					moistureValue = (moistureValue - this._generator.MoistureData.Min) / (this._generator.MoistureData.Max - this._generator.MoistureData.Min);
					t.MoistureValue = moistureValue;

					//set moisture type
					if (moistureValue < this._generator.DryerValue) t.MoistureType = MoistureType.Dryest;
					else if (moistureValue < this._generator.DryValue) t.MoistureType = MoistureType.Dryer;
					else if (moistureValue < this._generator.WetValue) t.MoistureType = MoistureType.Dry;
					else if (moistureValue < this._generator.WetterValue) t.MoistureType = MoistureType.Wet;
					else if (moistureValue < this._generator.WettestValue) t.MoistureType = MoistureType.Wetter;
					else t.MoistureType = MoistureType.Wettest;

					// Set heat value
					float heatValue = this._generator.HeatData.Data[x, y];
					heatValue = (heatValue - this._generator.HeatData.Min) / (this._generator.HeatData.Max - this._generator.HeatData.Min);
					t.HeatValue = heatValue;

					// set heat type
					if (heatValue < this._generator.ColdestValue) t.HeatType = HeatType.Coldest;
					else if (heatValue < this._generator.ColderValue) t.HeatType = HeatType.Colder;
					else if (heatValue < this._generator.ColdValue) t.HeatType = HeatType.Cold;
					else if (heatValue < this._generator.WarmValue) t.HeatType = HeatType.Warm;
					else if (heatValue < this._generator.WarmerValue) t.HeatType = HeatType.Warmer;
					else t.HeatType = HeatType.Warmest;

					//Debug.Log(t.HeightValue);

					//t.HexType = HexType.Ocean;
					t.HexType = HexType.GetHexType(t.HeightType, t.MoistureType, t.HeatType);
					//Debug.Log(t.HeightType);

					//_world.AddHex(t, x, y);
					_world.landscape.SetTile((Vector3Int)t.Position, t.HexType.Tile);

					//Debug.Log(t.MoistureValue);
					//Debug.Log(t.Position);
				}
			}
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.K))
            {
                _heightChange = -0.00015f;
                _heatChange = 0.005f;
                _moistureChange = 0.0001f;
                GlobalWarming(_heightChange, _heatChange, _moistureChange);
            }
        }
    }
}
