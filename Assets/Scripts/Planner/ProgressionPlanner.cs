using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

            List<HSPNode> solve(List<HSPPredicate> _initpreds, List<HSPPredicate> _goalpreds, List<HSPAction> _actions, int _H, int _W)
            {

                HashSet<string> explored = new HashSet<string>();
                Dictionary<string, int> g_cost = new Dictionary<string, int>();

                HashSet<string> goal = AddGroundPredicate(_goalpreds);
                HSPNode start = new HSPNode(AddGroundPredicate(_initpreds), null, null, 0, 0);

                Enqueue(start, start.getStateString());

                while (frontierQ.Count() > 0) {

                    HSPNode node = frontierQ.Dequeue();
                    HashSet<string> state = node.GetState();

                    explored.Add(node.getStateString());

                    if (ArrivedAtGoal(goal, state)) {
                        plan = node.getPath();
                        plan.Reverse(); 
                        return plan;
                    }

                    foreach(HSPAction action in groundedActions) {

                        if (action.isApplicableIn(state)) {

                            HashSet<string> newState = DeriveNewStateFromAction(action, state);

                            //#if node already explored, don't bother 
                            string stateString = createStateString(newState);
                        
                            if (!explored.Contains(stateString)) {

                                HSPNode new_node = new HSPNode(newState, action, node, (node.GetG()+1), 0);
                                
                                if (!g_cost.ContainsKey(stateString)) {
                                    g_cost.Add(stateString, new_node.GetG());
                                }
                                
                                if (!enQueued.Contains(stateString) || new_node.GetG() < g_cost[stateString]) {
                                    Enqueue(new_node, stateString);
                                }
                                
                            }                            

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

        private string createStateString(HashSet<string> state) {
            string sep = ", ";
            string[] output = new string[state.Count];
            output = state.ToArray();
            Array.Sort(output);
            return "( " + String.Join( sep, output ) + " )";
        }          

        private HashSet<string> AddGroundPredicate(List<HSPPredicate> _predicates) {
            Stopwatch watch = new Stopwatch();
            watch.Start();            
            
            HashSet<string> grounds = new HashSet<string>();
            
            foreach(var predicate in _predicates) {
                grounds.UnionWith(predicate.GetGroundString());
            }
            
            watch.Stop();
            timeGrounding += watch.Elapsed.TotalSeconds;
            
            return grounds;
        }

        private HashSet<string> DeriveNewStateFromAction(HSPAction action, HashSet<string> groundedState) {

            Stopwatch watch = new Stopwatch();
            watch.Start(); 

            //Derive New State From Action
            HashSet<string> newGrounds = new HashSet<string>();
            foreach(var item in groundedState) {
                newGrounds.Add(item);
            }
            newGrounds.ExceptWith(action.getNegEffects());
            newGrounds.UnionWith(action.getPosEffects());

            watch.Stop();
            timeDeriveState += watch.Elapsed.TotalSeconds;

            return newGrounds;
        }

        private void Enqueue(HSPNode node, string stateString) {
            frontierQ.Enqueue(node);
            enQueued.Add(stateString);
        }

        private bool ArrivedAtGoal(HashSet<string> _goal, HashSet<string> _state) {
            return _goal.IsSubsetOf(_state);
        }

    }
    
}