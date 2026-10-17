using System.Collections.Generic;
using UnityEngine;

public interface ITableSource
{
    List<Dictionary<string,string>> LoadRows();
}
