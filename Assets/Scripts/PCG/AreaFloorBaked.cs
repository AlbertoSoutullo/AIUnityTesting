using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PCG
{
    public class AreaFloorBaked: MonoBehaviour
    {
        [SerializeField] private NavMeshSurface _surface;
        [SerializeField] private Player _player;
        [SerializeField] private float _updateRate = 0.1f;
        [SerializeField] private float _movementThreshold = 3;
        [SerializeField] private Vector3 NavMeshSize = new Vector3(20,20,20);

        private Vector3 WorldAnchor;
        private NavMeshData _navMeshData;
        private List<NavMeshBuildSource> _sources = new List<NavMeshBuildSource>();

        private void Start()
        {
            _navMeshData = new NavMeshData();
            NavMesh.AddNavMeshData(_navMeshData);
            BuildNavMesh(false);
            StartCoroutine(CheckPlayerMovement());
        }

        private IEnumerator CheckPlayerMovement()
        {
            WaitForSeconds wait = new WaitForSeconds(_updateRate);

            while (true)
            {
                if (Vector3.Distance(WorldAnchor, _player.transform.position) > _movementThreshold)
                {
                    BuildNavMesh(true);
                    WorldAnchor = _player.transform.position;
                }

                yield return wait;
            }
        }
        
        private void BuildNavMesh(bool Async)
        {
            Bounds navMeshBounds = new Bounds(_player.transform.position, NavMeshSize);

            List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();

            List<NavMeshModifier> modifiers;

            if (_surface.collectObjects == CollectObjects.Children)
            {
                //modifiers = new List<NavMeshModifier>(_surface.GetComponentInChildren<NavMeshModifier>());
                modifiers = new List<NavMeshModifier>();
            }
            else
            {
                modifiers = NavMeshModifier.activeModifiers;
            }

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (((_surface.layerMask & (1 << modifiers[i].gameObject.layer)) == 1)
                    && modifiers[i].AffectsAgentType(_surface.agentTypeID))
                {
                    markups.Add(new NavMeshBuildMarkup()
                    {
                        root = modifiers[i].transform,
                        overrideArea = modifiers[i].overrideArea,
                        area = modifiers[i].area,
                        ignoreFromBuild = modifiers[i].ignoreFromBuild
                    });

                }
            }

            if (_surface.collectObjects == CollectObjects.Children)
            {
                NavMeshBuilder.CollectSources(_surface.transform, _surface.layerMask, _surface.useGeometry,
                    _surface.defaultArea, markups, _sources);
                
            }
            else
            {
                NavMeshBuilder.CollectSources(navMeshBounds, _surface.layerMask, _surface.useGeometry,
                    _surface.defaultArea, markups, _sources);
            }

            _sources.RemoveAll(_sources => _sources.component != null &&
                                           _sources.component.GetComponent<NavMeshAgent>() != null);

            if (Async)
            {
                NavMeshBuilder.UpdateNavMeshDataAsync(_navMeshData, _surface.GetBuildSettings(), _sources,
                    new Bounds(_player.transform.position, NavMeshSize));
            }
        }
    }
}