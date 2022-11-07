using System;
using UnityEngine;

namespace Zenject.Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        [Inject]
        Settings _settings = null;

        public override void InstallBindings()
        {

            InstallAsteroids();
            InstallShip();
            InstallMisc();
            InstallSignals();
            InstallExecutionOrder();
        }

        void InstallAsteroids()
        {

            Container.BindInterfacesAndSelfTo<AsteroidManager>().AsSingle();

            Container.BindFactory<Asteroid, Asteroid.Factory>()
                // This means that any time Asteroid.Factory.Create is called, it will instantiate
                // this prefab and then search it for the Asteroid component
                .FromComponentInNewPrefab(_settings.AsteroidPrefab)
                // We can also tell Zenject what to name the new gameobject here
                .WithGameObjectName("Asteroid")
                // GameObjectGroup's are just game objects used for organization
                // This is nice so that it doesn't clutter up our scene hierarchy
                .UnderTransformGroup("Asteroids");
        }

        void InstallMisc()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.Bind<LevelHelper>().AsSingle();

            Container.BindInterfacesTo<AudioHandler>().AsSingle();

            // FromComponentInNewPrefab matches the first transform only just like GetComponentsInChildren
            // So can be useful in cases where we don't need a custom MonoBehaviour attached
            Container.BindFactory<Transform, ExplosionFactory>()
                .FromComponentInNewPrefab(_settings.ExplosionPrefab);

            Container.BindFactory<Transform, BrokenShipFactory>()
                .FromComponentInNewPrefab(_settings.BrokenShipPrefab);
        }

        void InstallSignals()
        {
            // Every scene that uses signals needs to install the built-in installer SignalBusInstaller
            // Or alternatively it can be installed at the project context level (see docs for details)
            SignalBusInstaller.Install(Container);

            // Signals can be useful for game-wide events that could have many interested parties
            Container.DeclareSignal<ShipCrashedSignal>();
        }

        void InstallShip()
        {
            Container.Bind<ShipStateFactory>().AsSingle();

            // Note that the ship itself is bound using a ZenjectBinding component (see Ship
            // game object in scene heirarchy)

            Container.BindFactory<ShipStateWaitingToStart, ShipStateWaitingToStart.Factory>().WhenInjectedInto<ShipStateFactory>();
            Container.BindFactory<ShipStateDead, ShipStateDead.Factory>().WhenInjectedInto<ShipStateFactory>();
            Container.BindFactory<ShipStateMoving, ShipStateMoving.Factory>().WhenInjectedInto<ShipStateFactory>();
        }

        void InstallExecutionOrder()
        {
            // In many cases you don't need to worry about execution order,
            // however sometimes it can be important
            // If for example we wanted to ensure that AsteroidManager.Initialize
            // always gets called before GameController.Initialize (and similarly for Tick)
            // Then we could do the following:
            Container.BindExecutionOrder<AsteroidManager>(-20);
            Container.BindExecutionOrder<GameController>(-10);

            // Note that they will be disposed of in the reverse order given here
        }

        [Serializable]
        public class Settings
        {
            public GameObject ExplosionPrefab;
            public GameObject BrokenShipPrefab;
            public GameObject AsteroidPrefab;
            public GameObject ShipPrefab;
        }
    }
}

