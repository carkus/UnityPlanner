using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;

using System.Threading.Tasks;
using System.Threading;


namespace Planner
{

    public class ProgressionPlanner
    {

        private List<HSPPredicate> initState;
        private List<HSPPredicate> goalState;
        private List<HSPAction> groundedActions;

        private PriorityQueue<HSPNode> frontierQ = new PriorityQueue<HSPNode>();
        private HashSet<string> enQueued = new HashSet<string>();

        //private List<HSPNode> plan = new List<HSPNode>();

        private Thread planningThread;

        private double timeGrounding = 0;
        private double timeApplicables = 0;
        private double timeDeriveState = 0;
        private double timeQueueing = 0;
        
        //Singleton
        private static ProgressionPlanner instance;
        private ProgressionPlanner()
        {
        }

        public static ProgressionPlanner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProgressionPlanner();
                }
                return instance;
            }
        }

        public void buildPlan(List<HSPPredicate> _state, List<HSPPredicate> _goal, List<HSPAction> _actions) {

            initState = _state;
            //initState = RemoveRedundanciesFromState(_state, _goal);
            goalState = _goal;
            groundedActions = _actions;
            PerformPlan();

        }

        private async void PerformPlan() {

            List<HSPNode> plan = new List<HSPNode>();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            plan = await Task.Run(() => solve(initState, goalState, groundedActions, 1, 1) );
            watch.Stop();

            UnityEngine.Debug.Log("Time taken: " + watch.Elapsed.TotalSeconds.ToString());
            UnityEngine.Debug.Log("Time taken timeGrounding: " + timeGrounding);
            UnityEngine.Debug.Log("Time taken timeApplicables: " + timeApplicables);
            UnityEngine.Debug.Log("Time taken timeDeriveState: " + timeDeriveState);
            UnityEngine.Debug.Log("Time taken timeQueueing: " + timeQueueing);
            
            printPlanActions(plan);

            List<HSPNode> solve(List<HSPPredicate> _state, List<HSPPredicate> _goal, List<HSPAction> _actions, int _H, int _W)
            {

                HashSet<string> explored = new HashSet<string>();
                Dictionary<string, int> g_cost = new Dictionary<string, int>();

                HSPState init = new HSPState(AddGroundPredicate(_state));
                HSPState goal = new HSPState(AddGroundPredicate(_goal));

                HSPNode start = new HSPNode((HSPState)init.Clone(), null, null, 0, 0);

                Enqueue(start, start.GetStateString());

                while (frontierQ.Count() > 0) {

                    HSPNode node = frontierQ.Dequeue();
                    HSPState state = (HSPState)node.GetState().Clone();

                    explored.Add(state.ToString());//State or Node?

                    if (ArrivedAtGoal(goal, state)) {
                        plan = node.getPath();
                        plan.Reverse(); 
                        return plan;
                    }

                    List<HSPAction> applicables = getApplicables(state);

                    foreach(HSPAction action in applicables) {
                        
                        HSPState newState = DeriveNewStateFromAction(action, state);

                        //#if node already explored, don't bother 
                        string stateString = newState.ToString();
                    
                        if (explored.Contains(stateString)) {
                            continue;
                        }
                        
                        HSPNode new_node = new HSPNode((HSPState)newState.Clone(), action, node, (node.GetG()+1), 0);
    
                        if (!g_cost.ContainsKey(stateString)) {
                            g_cost.Add(stateString, new_node.GetG());
                        }
                        
                        if (!enQueued.Contains(new_node.GetStateString()) || new_node.GetG() < g_cost[stateString]) {
                            Enqueue(new_node, stateString);
                        }

                    }

                }

                return null;

            }

        }

        //-------------------------------------------------
        //UTILS
        //-------------------------------------------------

        private void printPlanActions(List<HSPNode> plan) {            
            foreach(HSPNode node in plan) {
                string act = node.GetAction().GetString();
                UnityEngine.Debug.Log(act + " : " + node.GetG());
            }
        }

        private HashSet<string> AddGroundPredicate(List<HSPPredicate> preds) {
            Stopwatch watch = new Stopwatch();
            watch.Start();            
            
            HashSet<string> grounds = new HashSet<string>();
            
            foreach(var item in preds) {
                grounds.UnionWith(item.GetGroundString());
            }
            
            watch.Stop();
            timeGrounding += watch.Elapsed.TotalSeconds;
            
            return grounds;
        }

        private HSPState DeriveNewStateFromAction(HSPAction action, HSPState state) {

            //Stopwatch watch = new Stopwatch();
            //watch.Start(); 

            //Derive New State From Action
            HashSet<string> negEffects = AddGroundPredicate(action._negEffects);
            HashSet<string> posEffects = AddGroundPredicate(action._posEffects);

            HashSet<string> newGrounds = new HashSet<string>();
            foreach(var item in state.GroundState()) {
                newGrounds.Add(item);
            }
            newGrounds.ExceptWith(negEffects);
            newGrounds.UnionWith(posEffects);

            //watch.Stop();
            //timeDeriveState += watch.Elapsed.TotalSeconds;

            return new HSPState(newGrounds);
        }

        private List<HSPAction> getApplicables(HSPState state) {
            
            //Stopwatch watch = new Stopwatch();
            //watch.Start(); 

            List<HSPAction> applicables = new List<HSPAction>();
            HashSet<string> groundedState = state.GroundState();
            //i.e. the actions may be performed in state
            //e.g. if a.precond <= state:
            //check if preconditions of action are subset of state
            foreach(HSPAction action in groundedActions) {
                HashSet<string> precons = AddGroundPredicate(action._preconditions);
                if (precons.IsSubsetOf(groundedState)) {
                    applicables.Add(action);
                }
            }

            //watch.Stop();
            //timeApplicables += watch.Elapsed.TotalSeconds;

            return applicables;
        }

        private void Enqueue(HSPNode node, string stateString) {

            //Stopwatch watch = new Stopwatch();
            //watch.Start(); 

            frontierQ.Enqueue(node);
            enQueued.Add(stateString);

            //watch.Stop();
            //timeQueueing += watch.Elapsed.TotalSeconds;            
        }

        private bool ArrivedAtGoal(HSPState _goal, HSPState _state) {
            return _goal.GroundState().IsSubsetOf(_state.GroundState());
        }

    }
    
}