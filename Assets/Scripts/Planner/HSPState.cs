using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


public class HSPState : IComparable<HSPState> {

    private HashSet<string> _grounds = new HashSet<string>();

    public HSPState (HashSet<string> state) {
        _grounds = state;
    }

    public HSPState applyAction(HashSet<string> negEffects, HashSet<string> posEffects) {
        HashSet<string> new_state = new HashSet<string>();
        new_state = _grounds;        
        new_state.ExceptWith(negEffects);
        new_state.UnionWith(posEffects);
        return new HSPState(new_state);
    }

    public HashSet<string> GroundedState() {
        return _grounds;
    }
    
    public int CompareTo(HSPState other) {
        if (this._grounds.Equals(other._grounds)) {
            return 0;
        }
        if (this._grounds.IsSubsetOf(other._grounds)) {
            return -1;
        } 
        if (this._grounds.IsSupersetOf(other._grounds)) {
            return 1;
        }                
        return 0;
    }

    // Define the is greater than operator.
    public static bool operator >  (HSPState operand1, HSPState operand2)
    {
       return operand1.CompareTo(operand2) == 1;
    }
    
    // Define the is less than operator.
    public static bool operator <  (HSPState operand1, HSPState operand2)
    {
       return operand1.CompareTo(operand2) == -1;
    }

    // Define the is greater than or equal to operator.
    public static bool operator >=  (HSPState operand1, HSPState operand2)
    {
       return operand1.CompareTo(operand2) >= 0;
    }
    
    // Define the is less than or equal to operator.
    public static bool operator <=  (HSPState operand1, HSPState operand2)
    {
       return operand1.CompareTo(operand2) <= 0;
    }




    /*private List<HSPPredicate> applyPositiveEffects(List<HSPPredicate> state, List<HSPPredicate> effects) {
        foreach (HSPPredicate effect in effects) {
            state.Add(effect);
        }
        return state;
    }

    private List<HSPPredicate> applyNegativeEffects(List<HSPPredicate> state, List<HSPPredicate> effects) {
        List<HSPPredicate> new_state = new List<HSPPredicate>();
        
        foreach (HSPPredicate effect in effects) {

            foreach (HSPPredicate pred in state) {

                bool found = false;

                if (pred.GetName() == effect.GetName()) {

                    List<HSPTerm> args = pred.GetArgs();
                    List<HSPTerm> effectargs = effect.GetArgs();

                    HashSet<string> thisArgs = new HashSet<string>();
                    HashSet<string> otherArgs = new HashSet<string>();

                    for(int i=0; i<args.Count; i++) {
                        thisArgs.Add(args[i].GetValue());
                    }

                    for(int i=0; i<effectargs.Count; i++) {
                        otherArgs.Add(effectargs[i].GetValue());
                    }

                    if (thisArgs.SetEquals(otherArgs)) {
                        found = true;
                    }

                }

                if (!found) {
                    new_state.Add(pred);
                }
            }
        }

        return new_state;
    } */

    /*public List<HSPPredicate> GetPredicates() {
        return _predicates;
    }*/
    
    /*
    '''
    State representation as a set of ground atoms.
    It makes the CWA (Closed World Assumption).
    '''

    def __str__(self):
        return str(sorted(map(str, self)))

    def __hash__(self):
        return hash(str(self))

    def union(self, predicates):
        ''' Return a new state with ground `predicates` added to the set. '''
        return State(self | predicates)

    def intersect(self, predicates):
        ''' Return a new state with the ground `predicates` in intersection of sets. '''
        return State(self & predicates)

    def difference(self, predicates):
        ''' Return a new state with ground `predicates` in the difference of sets. '''
        return State(self - predicates)
    */


}