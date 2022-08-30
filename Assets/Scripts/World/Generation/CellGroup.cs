using System.Collections.Generic;

public enum CellGroupType
{
	Water, 
	Land
}

public class CellGroup  {
	
	public CellGroupType Type;
	public List<Cell> Cells;

	public CellGroup()
	{
		Cells = new List<Cell> ();
	}
}
