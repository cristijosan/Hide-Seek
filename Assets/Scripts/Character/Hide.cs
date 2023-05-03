using System;
using System.Collections;
using System.Collections.Generic;
using Character.Movement;
using Pathfinding;
using UnityEngine;

namespace Character
{
    public class Hide : CharacterMovement
    {
        [Header("AI CONFIGURATION: ")]
        public Transform hidePoints;
        public float nextWaypointDistance = 0.75f;

        private readonly List<Transform> _wayPoints = new List<Transform>();
        private int _targetIndex;
        private int _lastTargetIndex;
        private Vector3 _target;

        private Seeker _seeker;
        private Path _path;
        private int _currentWaypoint = 0;

        private bool _isHide = false;

        private void Start()
        {
            if (isMine) return;
            _seeker = GetComponent<Seeker>();
            
            foreach (Transform child in hidePoints)
            {
                _wayPoints.Add(child);
            }

            UpdateDestination();
        }

        public override void ControlAi()
        {
            if (_path == null) return;
            if (_isHide) return;

            if (_currentWaypoint >= _path.vectorPath.Count)
            {
                // On end patch
                _isHide = true;
                MovementDirection = Vector3.zero;
                characterAnimator.SetFloat(Speed, 0);
                return;
            }
            
            var dir = (_path.vectorPath[_currentWaypoint] - transform.position).normalized;
            MovementDirection = new Vector3(dir.x, 0, dir.z).normalized;
            
            if (Vector3.Distance (transform.position,_path.vectorPath[_currentWaypoint]) < nextWaypointDistance)
                _currentWaypoint++;
        }
        
        private void UpdateDestination()
        {
            _target = _wayPoints[_targetIndex].position;
            _seeker.StartPath (transform.position,_target, OnPathComplete);
        }

        private void IterateWaypointIndex()
        {
            // TODO Write AI logic here
            // while (_targetIndex == _lastTargetIndex)
            // {
            //     _targetIndex = Random.Range(0, _wayPoints.Count - 1);
            // }
            //
            // _lastTargetIndex = _targetIndex;
        }
        
        private void OnPathComplete (Path p)
        {
            if (p.error) return;
            
            _path = p;
            _currentWaypoint = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Seeker"))
                Debug.Log(1);
        }

        public void Caught()
        {
            isCanMove = false;
            characterAnimator.SetBool(IsCaught, true);
            starStunned.gameObject.SetActive(true);
            starStunned.Play();
        }
    }
}
