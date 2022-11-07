using System;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using UnityEngine.Pool;

public class FigureSpawnerModel
{
    public Dictionary<int, Tween> Tweens = new ();
    public Dictionary<string, ObjectPool<Figure>> PoolsDictionary;
    public Pool PoolObject;
    public CancellationTokenSource SpawnerTokenSource;
    public Action<float, float> OnFigureClick;
    public int GameLevel = 1;
    public List<Figure> ActiveFigures;
}
