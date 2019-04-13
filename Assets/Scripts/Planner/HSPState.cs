
using System;
using System.Collections;
using System.Collections.Generic;


public class HSPState {

    
    //HashSet<string> self; //hash of string/HSPTerms??

    public List<HSPPredicate> predicates { get; set; }


    public HSPState (List<HSPPredicate> _state) {

        predicates = _state;


        //self = new HashSet<string>();

    }
    
    private void ground() {}//?? an idea - need?
    
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