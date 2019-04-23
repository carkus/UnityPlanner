using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

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

            solve(_state, _goal, _actions, 1, 1);


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

            HashSet<HSPState> explored = new HashSet<HSPState>();
            HashSet<HSPNode> prioritised = new HashSet<HSPNode>();

            Dictionary<HSPState, int> g_cost = new Dictionary<HSPState, int>();

            HSPState init = new HSPState(GroundPredicatesToState(_state));
            HSPState goal = new HSPState(GroundPredicatesToState(_goal));

            HSPNode start = new HSPNode(init, null, null, 0, 1);

            frontierQ.Enqueue(start);

            while (frontierQ.Count() > 0) {

                HSPNode node = frontierQ.Dequeue();
                HSPState state = node.getState();

                explored.Add(state);

                if (ArrivedAtGoal(goal, state)) {
                    List<HSPNode> plan = node.getPath();
                    return plan;
                }

                List<HSPAction> applicables = getApplicables(state);

                foreach(HSPAction action in applicables) {

                    HashSet<string> negEffects = GroundPredicatesToState(action._negEffects);
                    HashSet<string> posEffects = GroundPredicatesToState(action._posEffects);

                    HSPState new_state = state.applyAction(negEffects, posEffects);

                    //#if node already explored, don't bother 
                    bool repeatExplore = explored.Contains(new_state);

                    if (!repeatExplore) {
                        int cost = _W;
                        
                        HSPNode new_node = new HSPNode(new_state, action, node, node.GetG() + 1, cost);

                        bool repeatEnqueue = prioritised.Contains(new_node);

                        if (!repeatEnqueue || new_node.GetG() < g_cost[new_state]) {
                            g_cost[new_state] = new_node.GetG();
                            frontierQ.Enqueue(new_node);
                            prioritised.Add(new_node);
                        }

                    }

                }

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
            return _state.GroundedState().IsSubsetOf(_goal.GroundedState());
            //return _goal <= _state;
        }

        public int f(HSPNode node)
        {
            return node.GetG() + node.GetH();
        }

        /*def solve(self, W, heuristics, init, goal, actions):
            '''
            Implement best-first graph-search WA*. It receives `W` the weight of WA*
            and `heuristics` a function that receives a state s and the planning object (ie., self)
            and returns h(s). Check heuristics.py for more information.

            If problem has solution, return a triple (plan, num_explored, num_generated) where:
             - `plan` is a sequence of actions;
             - `num_explored` is the number of states explored; and
             - `num_generated` is the nubmer of states generated.
            Otherwise, it should return None.

            OBSERVATION: a state is 'explored' when it is removed from the frontier and
            a state is 'generated' when it is the successor state generated by an action
            regardless whether or not it is in the explored set or already in the frontier.
            '''
            self._grounded_actions = actions
            self._goal = goal

            def f(node):
                return node._g + node._h

            def return_plan(self, node, time_taken):
                actions, states = node.path()
                plandata = {}
                plandata['explored'] = len(self._explored)
                plandata['generated'] = self._generated
                plandata['time_taken'] = time_taken
                plandata['goal'] = self._goal
                return actions, states, plandata

            self._explored = set()
            self._generated = 0

            g_cost = {}

            #Start clock
            start_time = time.time()

            #start Planning
            start = State(init)
            start = Node(start, None, None, 0, W*heuristics(start, self))
            frontier = Frontier(f)
            frontier.push(start)

            while not frontier.is_empty():

                node = frontier.pop()
                state = node._state

                self._explored.add(state)

                if self.is_goal(state, goal):
                    end_time = time.time()
                    time_taken = "%.2f" % float(end_time - start_time)
                    return return_plan(self, node, time_taken)                               

                for action in self.applicable(state):
                    #calculate the result of applying the action to state
                    new_state = self.successor(state, action)

                    self._generated += 1

                    #if node already explored, don't bother 
                    if new_state in self._explored:
                       continue

                    #Grab new node
                    if self._use_geom_heuristic:
                        e = self._model.get_entity(action.label)
                        #cost = W*(len(e._pose)*self._geomweight)
                        #dont affect cost of cosr of 'discrete' actions
                        if e._rule == "transition":
                            cost = W*(len(e._pose)*self._geomweight)       
                        else :
                            cost = W
                    else:
                        cost = W

                    h_cost = cost*heuristics(new_state, self)
                    new_node = Node(new_state, action, node, node._g + 1, cost*heuristics(new_state, self))                   

                    '''
                    #Grab new node
                    node_label = '{0} {1}'.format(action.label, new_state)
                    if node_label not in generated:
                        #modify heuristic cost based Entity pose, distance, etc.
                        if self.use_geom_heuristic:
                            e = self._model.get_entity(action.label)
                            MOD = 0.5
                            cost = W*(len(e._pose)*MOD)
                        else:
                            cost = W
                        new_node = Node(new_state, action, node, node._g + 1, cost*heuristics(new_state, self))
                        generated.update({node_label:new_node})
                    else:
                        new_node = generated[node_label]                
                    '''

                    if new_node not in frontier or new_node._g < g_cost[new_state]:
                        #print node._g,' :: ',action.label,' : ',h_cost
                        #self.states_checked += 1
                        g_cost[new_state] = new_node._g
                        frontier.push(new_node)

            return None

        def applicable(self, state):
            ''' Return a list of applicable actions in a given `state`. '''
            a_actions = []
            for a in self.grounded_actions:

                #Two sets are equal iff every element of each set is contained in the other (each is a subset of the other).
                #A set is less than another set if and only if the first set is a proper subset of the second set (is a subset, but is not equal). 
                #A set is greater than another set if and only if the first set is a proper superset of the second set (is a superset, but is not equal).

                #check if preconditions of action are subset of state
                #i.e. the actions may be performed in current state
                if a.precond <= state:
                    a_actions.append(a)
            return a_actions*/

    }
    
}