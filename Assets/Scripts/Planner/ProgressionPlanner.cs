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
        
        public ProgressionPlanner()
        {
        }

        public async Task<List<HSPNode>> PerformPlan(List<HSPPredicate> _init, List<HSPPredicate> _goal, List<HSPAction> _actions) {

            return await Task.Run(() => solve() );
            
            List<HSPNode> solve()
            {
                
                List<HSPNode> plan = new List<HSPNode>();

                HashSet<string> explored = new HashSet<string>();
                Dictionary<string, int> g_cost = new Dictionary<string, int>();

                HashSet<string> goal = groundPredicates(_goal);
                HSPNode start = new HSPNode(groundPredicates(_init), null, null, 0, 0);

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

                    foreach(HSPAction action in _actions) {

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

        private List<HSPAction> getApplicableActions(HashSet<string> state, List<HSPAction> actions)    {
            List<HSPAction> _applicables = new List<HSPAction>();
            foreach(HSPAction action in actions) {
                if (action.isApplicableIn(state)) {
                    _applicables.Add(action);
                }
            }
            return _applicables;
        }

        private HashSet<string> groundPredicates(List<HSPPredicate> _predicates) {
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