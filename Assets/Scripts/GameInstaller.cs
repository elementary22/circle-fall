using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private PrefabSettings _prefabSettings;
    [SerializeField] private LevelSettings _levelSettings;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        
        Container.BindInstance(_prefabSettings);
        Container.BindInstance(_levelSettings);

        Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelUIMediator>().AsSingle();
        Container.Bind<FigureSpawnerModel>().AsSingle();

        #region ViewSignals

        Container.DeclareSignal<OnViewEnableViewSignal>();
        Container.DeclareSignal<OnViewDisableViewSignal>();
        Container.DeclareSignal<OnPlayViewSignal>();
        Container.DeclareSignal<OnCloseViewSignal>();
        Container.DeclareSignal<OnLevelCompleteViewSignal>();
        Container.DeclareSignal<OnResetGameViewSignal>();

        #endregion
        
        
        Container.DeclareSignal<OnApplicationLoadedCommandSignal>();
        Container.BindSignal<OnApplicationLoadedCommandSignal>().ToMethod<CreateFigureSpawnerCommand>(signal => signal.Execute).FromNew();

        Container.DeclareSignal<StartSpawnCommandSignal>();
        Container.BindSignal<StartSpawnCommandSignal>().ToMethod<StartSpawnCommand>(signal => signal.Execute).FromNew();
        
        Container.DeclareSignal<StopSpawnCommandSignal>();
        Container.BindSignal<StopSpawnCommandSignal>().ToMethod<StopSpawnCommand>(signal => signal.Execute).FromNew();

        Container.DeclareSignal<CheckLevelCompleteCommandSignal>();
        Container.BindSignal<CheckLevelCompleteCommandSignal>().ToMethod<CheckLevelCompleteCommand>(signal => signal.Execute).FromNew();

        Container.DeclareSignal<ChangeLevelCommandSignal>();
        Container.BindSignal<ChangeLevelCommandSignal>().ToMethod<ChangeLevelCommand>(signal => signal.Execute).FromNew();

        Container.DeclareSignal<GameCompleteCommandSignal>();
        Container.BindSignal<GameCompleteCommandSignal>().ToMethod<GameCompleteCommand>(signal => signal.Execute).FromNew();
        
        Container.DeclareSignal<ResetGameCommandSignal>();
        Container.BindSignal<ResetGameCommandSignal>().ToMethod<ResetGameCommand>(signal => signal.Execute).FromNew();

        Container.BindInterfacesAndSelfTo<FigureFactory>().AsCached();
        Container.BindFactory<Object, Figure, Figure.Factory>().FromFactory<FigureFactory>();

    }
}
