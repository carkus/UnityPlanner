
using System;
using System.Collections;
using System.Collections.Generic;

public class HSPNode : IComparable<HSPNode> {

    private HSPState _state;
    private HSPAction _action ;
    private HSPNode _parent;
    
    private int _f;
    private int _g;
    private int _h;

    public int CompareTo(HSPNode other) {
        if (this._f < other._f) {
            return 1;
        }
        return 0;
    }

    public HSPNode (HSPState state, HSPAction action, HSPNode parent = null, int g = 0, int h = 0) {

        _state = state;
        _action = action;
        _parent = parent;
        _g = g;
        _h = h;
        _f = 0;

    }

    public HSPState getState() {
        return _state;
    }

    public List<HSPNode> getPath() {
        
        List<HSPNode> path = new List<HSPNode>();
        HSPNode node = this;//new HSPNode();
        //node = this;

        while (node._parent != null) {
            path.Add(node);
            node = node._parent;
        }
        
        return path;
    }

    public int GetF() {
        return _f;
    }

    public int GetG() {
        return _g;
    }

    public int GetH() {
        return _h;
    }

    /*def __lt__(self, other):
        """
        nodes are sorted by f value (see a_star.py)

        :param other: compare Node
        :return:
        """
        return self._f < other._f

    def __eq__(self, other):
        ''' Return true if two nodes refer to the same state. '''
        return str(self._state) == str(other._state)

    def __str__(self):
        ''' Return the node string as the representation of the state it refers to. '''
        return str(self._state)

    def path(self):
        '''
        Return a list of actions and states in the path from initial state
        to the current state referred to by the node.
        '''
        actions = []
        states = []
        node = self
        while node._parent is not None:
            actions = [node._action] + actions
            states = [node._state] + states
            node = node._parent
        return [actions, states]    
    */


}