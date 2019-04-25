using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Utils;

namespace Planner
{

    public class ProgressionPlanner
    {

        private List<HSPPredicate> initState;
        private List<HSPPredicate> goalState;
        private List<HSPAction> groundedActions;

        private PriorityQueue<HSPNode> frontierQ = new PriorityQueue<HSPNode>();
        
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
            goalState = _goal;
            groundedActions = _actions;

            List<HSPNode> plan = solve(_state, _goal, _actions, 1, 1);
            plan.Reverse();

            foreach(HSPNode node in plan) {
                string act = node.GetAction().GetString();
                Debug.Log(act);
            }


        }

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

        public List<HSPNode> solve(List<HSPPredicate> _state, List<HSPPredicate> _goal, List<HSPAction> _actions, int _H, int _W)
        {

            int attempts = 0;

            HashSet<string> explored = new HashSet<string>();
            HashSet<string> prioritised = new HashSet<string>();

            Dictionary<string, int> g_cost = new Dictionary<string, int>();

            HSPState init = new HSPState(GroundPredicatesToState(_state));
            HSPState goal = new HSPState(GroundPredicatesToState(_goal));
            HSPNode start = new HSPNode((HSPState)init.Clone(), null, null, 0, 1);

            frontierQ.Enqueue(start);
            prioritised.Add(start.GetString());

            while (frontierQ.Count() > 0 && attempts < 10000) {

                HSPNode node = frontierQ.Dequeue();

                HSPState state = (HSPState)node.GetState().Clone();//deep copy here on get state?

                explored.Add(state.GetString());//State or Node?

                if (ArrivedAtGoal(goal, state)) {
                    List<HSPNode> plan = node.getPath();
                    return plan;
                }

                List<HSPAction> applicables = getApplicables(state);

                foreach(HSPAction action in applicables) {

                    HashSet<string> negEffects = GroundPredicatesToState(action._negEffects);
                    HashSet<string> posEffects = GroundPredicatesToState(action._posEffects);

                    HashSet<string> newGrounds = new HashSet<string>();
                    foreach(var item in state.GroundedState()) {
                        newGrounds.Add(item);
                    }
                    newGrounds.ExceptWith(negEffects);
                    newGrounds.UnionWith(posEffects);
                    
                    HSPState newState = new HSPState(newGrounds);

                    //#if node already explored, don't bother 
                    bool repeatExplore = explored.Contains(newState.GetString());

                    if (!repeatExplore) {
                        int cost = _W;
                        
                        HSPNode new_node = new HSPNode((HSPState)newState.Clone(), action, node, node.GetG() + 1, cost);

                        if (!g_cost.ContainsKey(new_node.GetString())) {
                            g_cost.Add(new_node.GetString(), new_node.GetG());
                        }
                        
                        int old_g = g_cost[new_node.GetString()];
                        bool repeatEnqueue = prioritised.Contains(new_node.GetString());

                        if (!repeatEnqueue || new_node.GetG() < old_g) {
                            frontierQ.Enqueue(new_node);
                            prioritised.Add(new_node.GetString());
                        }

                    }

                }

                attempts++;

            }

            return null;

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

        private bool ArrivedAtGoal(HSPState _goal, HSPState _state) {
            return _goal.GroundedState().IsSubsetOf(_state.GroundedState());
            //return _goal <= _state;
        }

        public int f(HSPNode node)
        {
            return node.GetG() + node.GetH();
        }

    }
    
}