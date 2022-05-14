// Unity Imports
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

// Project Imports
using Player.Scripts;

namespace PCG
{
    public class AreaFloorBaked: MonoBehaviour
    {
        [SerializeField] public NavMeshSurface surface;
        [SerializeField] public PlayerScript player;
        [SerializeField] public float updateRate = 2f;
        [SerializeField] public float movementThreshold = 3;
        [SerializeField] public Vector3 navMeshSize = new Vector3(20,20,20);

        private Vector3 _worldAnchor;
        private NavMeshData _navMeshData;
        private readonly List<NavMeshBuildSource> _sources = new List<NavMeshBuildSource>();

        private void Start()
        {
            _navMeshData = new NavMeshData();
            NavMesh.AddNavMeshData(_navMeshData);
            BuildNavMesh(false);
            StartCoroutine(CheckPlayerMovement());
        }

        private IEnumerator CheckPlayerMovement()
        {
            WaitForSeconds wait = new WaitForSeconds(updateRate);

            while (true)
            {
                if (NeedToUpdateNavMeshByDistance())
                {
                    BuildNavMesh(true);
                    _worldAnchor = player.transform.position;
                }
                yield return wait;
            }
        }
        
        private void BuildNavMesh(bool async)
        {
            Bounds navMeshBounds = new Bounds(player.transform.position, navMeshSize);

            List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
            List<NavMeshModifier> modifiers;

            if (surface.collectObjects == CollectObjects.Children)
                modifiers = new List<NavMeshModifier>();
            else
                modifiers = NavMeshModifier.activeModifiers;

            AddMarkupsToModifiers(modifiers, markups);
            CollectSources(markups, navMeshBounds);

            _sources.RemoveAll(sources => sources.component != null &&
                                           sources.component.GetComponent<NavMeshAgent>() != null);
            
            if (async)
                NavMeshBuilder.UpdateNavMeshDataAsync(_navMeshData, surface.GetBuildSettings(), _sources,
                    new Bounds(player.transform.position, navMeshSize));
        }

        private void CollectSources(List<NavMeshBuildMarkup> markups, Bounds navMeshBounds)
        {
            if (surface.collectObjects == CollectObjects.Children)
                NavMeshBuilder.CollectSources(surface.transform, surface.layerMask, surface.useGeometry,
                    surface.defaultArea, markups, _sources);
            else
                NavMeshBuilder.CollectSources(navMeshBounds, surface.layerMask, surface.useGeometry,
                    surface.defaultArea, markups, _sources);
        }

        private void AddMarkupsToModifiers(List<NavMeshModifier> modifiers, List<NavMeshBuildMarkup> markups)
        {
            foreach (var modifier in modifiers)
            {
                if (((surface.layerMask & (1 << modifier.gameObject.layer)) == 1)
                    && modifier.AffectsAgentType(surface.agentTypeID))
                {
                    markups.Add(new NavMeshBuildMarkup()
                    {
                        root = modifier.transform,
                        overrideArea = modifier.overrideArea,
                        area = modifier.area,
                        ignoreFromBuild = modifier.ignoreFromBuild
                    });
                }
            }
        }

        private bool NeedToUpdateNavMeshByDistance()
        {
            return (Vector3.Distance(_worldAnchor, player.transform.position) > movementThreshold);
        }
    }
}