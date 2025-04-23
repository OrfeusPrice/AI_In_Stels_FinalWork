using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameEntitiesInstaller : MonoInstaller
{
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform _playerPosition;

    public override void InstallBindings()
    {
        BindPlayer();
        //BindEnemyFabric();
    }

    private void BindPlayer()
    {
        var playerInstance = Container.InstantiatePrefabForComponent<Player>(_playerPrefab, _playerPosition.transform);

        Container.Bind<Player>().FromInstance(playerInstance).AsSingle().NonLazy();
    }
}