using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

//using Utils;

namespace Planner
{

    public class ProgressionPlanner
    {

        private List<HSPPredicate> initState;
        private List<HSPPredicate> goalState;
        private List<HSPAction> groundedActions;

        private PriorityQueue<HSPNode> frontierQ = new PriorityQueue<HSPNode>();
        private HashSet<string> enQueued = new HashSet<string>();
        
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

            //initState = RemoveRedundanciesFromState(_state, _goal);

            initState = _state;
            goalState = _goal;
            groundedActions = _actions;

            //List<HSPNode> plan = new List<HSPNode>();
            List<List<HSPNode>> planStack = new List<List<HSPNode>>();
            try
            {
                //plan = solve(_state, _goal, _actions, 1, 1);
                planStack = solve(_state, _goal, _actions, 1, 1);
            }
            catch(Exception e)
            {
                Debug.Log("Planner failed: " + e);
            }
            finally 
            {

                foreach(List<HSPNode> plan in planStack) {
                    if (plan != null) {
                        plan.Reverse();
                        foreach(HSPNode node in plan) {
                            string act = node.GetAction().GetString();
                            Debug.Log(act + " : " + node.GetG());
                        }
                    }
                    else {
                        Debug.Log("Planner failed.");
                    }
                }

            }

        }

        /*private List<HSPPredicate> RemoveRedundanciesFromState(List<HSPPredicate> _state, List<HSPPredicate> _goal) {

        }*/



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

        public List<List<HSPNode>> solve(List<HSPPredicate> _state, List<HSPPredicate> _goal, List<HSPAction> _actions, int _H, int _W)
        {

            List<List<HSPNode>> planStack = new List<List<HSPNode>>();
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
                    List<HSPNode> plan = node.getPath();
                    planStack.Add(plan);
                    //This will return the first plan only.
                    //Comment out to return multiple plans
                    return planStack;
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

            return planStack;

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

        public void Enqueue(HSPNode node, string stateString) {
            frontierQ.Enqueue(node);
            enQueued.Add(stateString);
        }

        private bool ArrivedAtGoal(HSPState _goal, HSPState _state) {
            return _goal.GroundedState().IsSubsetOf(_state.GroundedState());
            //return _goal <= _state;
        }

    }
    
}