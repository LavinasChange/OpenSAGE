﻿using System;
using System.Numerics;
using OpenSage.Data.Ini;
using OpenSage.Logic.Object;
using OpenSage.Logic.OrderGenerators;
using OpenSage.Logic.Orders;

namespace OpenSage.Logic
{
    public class OrderGeneratorSystem : GameSystem
    {
        public IOrderGenerator ActiveGenerator;

        public bool HasActiveOrderGenerator => ActiveGenerator != null;

        public OrderGeneratorSystem(Game game) : base(game) { }

        private Vector3? _worldPosition;

        public void UpdatePosition(Vector2 mousePosition)
        {
            _worldPosition = GetTerrainPosition(mousePosition);

            if (_worldPosition.HasValue && ActiveGenerator != null)
            {
                ActiveGenerator.UpdatePosition(_worldPosition.Value);
            }
        }

        public void OnMove()
        {
            if (!_worldPosition.HasValue)
            {
                return;
            }

            var order = Order.CreateMoveOrder(Game.Scene3D.GetPlayerIndex(Game.Scene3D.LocalPlayer), _worldPosition.Value);
            Game.NetworkMessageBuffer.AddLocalOrder(order);
        }

        public void OnActivate()
        {
            if (!_worldPosition.HasValue || ActiveGenerator == null)
            {
                return;
            }

            var result = ActiveGenerator.TryActivate(Game.Scene3D);

            switch (result)
            {
                case OrderGeneratorResult.Success success:
                {
                    // TODO: Wrong place, wrong behavior.
                    Game.Audio.PlayAudioEvent("DozerUSAVoiceBuild");

                    foreach (var order in success.Orders)
                    {
                        Game.NetworkMessageBuffer.AddLocalOrder(order);
                    }

                    if (success.Exit)
                    {
                        ActiveGenerator = null;
                    }

                    break;
                }
                case OrderGeneratorResult.FailureResult _:
                    // TODO: Wrong place, wrong behavior.
                    Game.Audio.PlayAudioEvent("DozerUSAVoiceBuildNot");
                    // TODO: Show error message in HUD
                    break;
            }
        }

        private Vector3? GetTerrainPosition(Vector2 mousePosition)
        {
            var ray = Game.Scene3D.Camera.ScreenPointToRay(mousePosition);
            return Game.Scene3D.Terrain.Intersect(ray);
        }

        public void StartConstructBuilding(ObjectDefinition buildingDefinition)
        {
            if (!buildingDefinition.KindOf.Get(ObjectKinds.Structure))
            {
                throw new ArgumentException("Building must have the STRUCTURE kind.", nameof(buildingDefinition));
            }

            // TODO: Handle ONLY_BY_AI
            // TODO: Copy default settings from DefaultThingTemplate
            /*if (buildingDefinition.Buildable != ObjectBuildableType.Yes)
            {
                throw new ArgumentException("Building must be buildable.", nameof(buildingDefinition));
            }*/

            // TODO: Check that the builder can build that building.
            // TODO: Check that the building has been unlocked.
            // TODO: Check that the builder isn't building something else right now?

            var gameData = Game.ContentManager.IniDataContext.GameData;
            var definitionIndex = Game.ContentManager.IniDataContext.Objects.IndexOf(buildingDefinition) + 1;

            ActiveGenerator = new ConstructBuildingOrderGenerator(buildingDefinition, definitionIndex, gameData);
        }

        public void StartConstructUnit(ObjectDefinition unitDefinition)
        {
            // TODO: Handle ONLY_BY_AI
            // TODO: Copy default settings from DefaultThingTemplate

            // TODO: Check that the building can build that unit.
            // TODO: Check that the unit has been unlocked.
            // TODO: Check that the building isn't building something else right now?

            var gameData = Game.ContentManager.IniDataContext.GameData;
            var definitionIndex = Game.ContentManager.IniDataContext.Objects.IndexOf(unitDefinition) + 1;

            ActiveGenerator = new TrainUnitOrderGenerator(unitDefinition, definitionIndex, gameData);
        }
    }
}
