using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
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

        private List<HSPNode> plan = new List<HSPNode>();

        private Thread planningThread;
        
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
            
            plan = await Task.Run(() => solve(initState, goalState, groundedActions, 1, 1) );


            List<HSPNode> solve(List<HSPPredicate> _state, List<HSPPredicate> _goal, List<HSPAction> _actions, int _H, int _W)
            {

                HashSet<string> explored = new HashSet<string>();
                Dictionary<string, int> g_cost = new Dictionary<string, int>();

                HSPState init = new HSPState(GroundPredicatesToState(_state));
                HSPState goal = new HSPState(GroundPredicatesToState(_goal));

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

            foreach(HSPNode node in plan) {
                string act = node.GetAction().GetString();
                Debug.Log(act + " : " + node.GetG());
            }

        }

        //-------------------------------------------------
        //UTILS
        //-------------------------------------------------

        private HashSet<string> GroundPredicatesToState(List<HSPPredicate> predicates) {
            HashSet<string> grounds = new HashSet<string>();

            string getGroundString(HSPPredicate predicate, HSPTerm arg) {
                if (arg.IsTyped()) {
                    return $"{predicate.GetName()}({arg.GetValue()}, {arg.GetTermType()})"; 
                }
                return $"{predicate.GetName()}({arg.GetValue()})";        
            }

            foreach(HSPPredicate pred in predicates) {
                foreach(HSPTerm arg in pred.GetArgs()) {
                    grounds.Add(getGroundString(pred, arg));
                }
            }

            return grounds;
        }        

        private HSPState DeriveNewStateFromAction(HSPAction action, HSPState state) {
            //Derive New State From Action
            HashSet<string> negEffects = GroundPredicatesToState(action._negEffects);
            HashSet<string> posEffects = GroundPredicatesToState(action._posEffects);

            HashSet<string> newGrounds = new HashSet<string>();
            foreach(var item in state.GroundedState()) {
                newGrounds.Add(item);
            }
            newGrounds.ExceptWith(negEffects);
            newGrounds.UnionWith(posEffects);
            return new HSPState(newGrounds);
        }

        private List<HSPAction> getApplicables(HSPState state) {
            List<HSPAction> applicables = new List<HSPAction>();
            HashSet<string> groundedState = state.GroundedState();
            //i.e. the actions may be performed in state
            //e.g. if a.precond <= state:
            //check if preconditions of action are subset of state
            foreach(HSPAction action in groundedActions) {
                HashSet<string> precons = GroundPredicatesToState(action._preconditions);
                if (precons.IsSubsetOf(groundedState)) {
                    applicables.Add(action);
                }
            }
            return applicables;
        }

        private void Enqueue(HSPNode node, string stateString) {
            frontierQ.Enqueue(node);
            enQueued.Add(stateString);
        }

        private bool ArrivedAtGoal(HSPState _goal, HSPState _state) {
            return _goal.GroundedState().IsSubsetOf(_state.GroundedState());
        }

    }
    
}