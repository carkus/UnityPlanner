using System.Collections;

public class HSPNode {

    private HSPState _state;
    private HSPAction _action;
    private HSPNode _parent;
    private int _f;

    public int _g { get; set; }
    public int _h { get; set; }

    public HSPNode (HSPState state, HSPAction action, HSPNode parent = null, int g = 0, int h = 0) {

        _state = state;
        _action = action;
        _parent = parent;
        _g = g;
        _h = h;
        _f = 0;

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